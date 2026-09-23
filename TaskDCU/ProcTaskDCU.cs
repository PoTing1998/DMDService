using ASI.Lib.Comm.Socket.Admin;
using ASI.Lib.Config;
using ASI.Lib.DB;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Lib.UC;
using ASI.Wanda.DMD.ProcMsg;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ASI.Wanda.DMD.TaskDCU
{
    public class ProcTaskDCU : ProcBase
    {
        #region construct
        private ASI.Wanda.DMD.DMD_API mDMD_API = null;
        public string mDMDServerConnStr = "";

        public bool mIsConnectedToDCU = false;
        /// <summary>
        /// 當前車站 ID
        /// </summary>
        private string mCurrentStationID = "";
        /// <summary>
        /// 儲存已連接的客戶端
        /// </summary>
        private List<string> connectedClients = new List<string>();
        /// <summary>
        /// 車站代碼 -> 已連線的 DCU 客戶端 (IP:port)，來源為 station_conf.dcu_ip 反查
        /// </summary>
        private Dictionary<string, string> mStationClients =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// connectedClients / mStationClients 的同步鎖。
        /// 這兩個集合會被 socket 事件緒與訊息處理緒同時存取，
        /// 鎖內只做記憶體操作，DB 查詢與 Send 一律留在鎖外。
        /// </summary>
        private readonly object mClientsLock = new object();

        /// <summary>
        /// 記錄連入 DCU Socket Server 的 Client，供 UITest「Socket 連線監控」查詢
        /// </summary>
        private readonly SocketClientTracker mSocketTracker = new SocketClientTracker("DCU");

        /// <summary>
        /// 本機管理通道 (預設 127.0.0.1:18001)
        /// </summary>
        private SocketAdminServer mSocketAdmin = null;
        public class DeviceInfo
        {
            public string StationID { get; set; }
            public string AreaID { get; set; }
            public string DeviceID { get; set; }
        }
        #endregion


        #region 啟動Task
        /// <summary>
        /// 處理DCU模組執行程序所收到之訊息
        /// </summary>
        /// <param name="pLabel"></param>
        /// <param name="pBody"></param>
        /// <returns></returns>
        public override int ProcEvent(string pLabel, string pBody)
        {
            
            LogFile.Display(pBody);

            if (pLabel == MSGFinish.Label)
            {
                return 0;
            }
            else if (pLabel == MSGFromTaskCMFT.Label)
            {

                return ProMsgFromCMFT(pBody);
            }
            else if (pLabel == MSGFromTaskOCS.Label)
            {
                return ProMsgFromOCS(pBody);
            }
            return base.ProcEvent(pLabel, pBody);
        }
        public override int ProcTimerEvent(string pMessage) // handle timer message
        {
            //定時回報TaskMain
            if (base.ProcTimerEvent(pMessage) <= 0)
            {
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// 啟始處理DCU模組執行程序
        /// </summary>
        /// <param name="pComputer"></param>
        /// <param name="pProcName"></param>
        /// <returns></returns>  
        public override int StartTask(string pComputer, string pProcName)
        {
            mTimerTick = 30;
            _mProcName = "TaskDCU";

            // 讀取本機站所代碼。
            // OCC / BOCC 為全線集中式主機，服務所有車站，沒有「單一車站」的概念，
            // 未設定時一律視同 OCC (不對 target_du 做車站篩選)，不再擋住啟動。
            mCurrentStationID = ConfigApp.Instance.GetConfigSetting("STATION_ID");
            if (string.IsNullOrEmpty(mCurrentStationID))
            {
                mCurrentStationID = DCUHelper.CentralStationID;
                ASI.Lib.Log.DebugLog.Log(_mProcName,
                    $"未設定 STATION_ID，視同集中式主機 [{mCurrentStationID}]，不對 target_du 做車站篩選");
            }
            else
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName,
                    DCUHelper.IsCentralStation(mCurrentStationID)
                        ? $"本機站所: {mCurrentStationID} (集中式主機，不做車站篩選)"
                        : $"本機車站: {mCurrentStationID} (僅處理該站的 target_du)");
            }

            // DMD Database Configuration
            string sDMD_DBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            string sDMD_DBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            string sDMD_DBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");

            // CMFT Database Configuration
            string sCMFT_DBIP = ConfigApp.Instance.GetConfigSetting("CMFT_DB_IP");
            string sCMFT_DBPort = ConfigApp.Instance.GetConfigSetting("CMFT_DB_Port");
            string sCMFT_DBName = ConfigApp.Instance.GetConfigSetting("CMFT_DB_Name");
            // 帳密由 Config 讀取，未設定時沿用 postgres/postgres
            string sDMD_DBUserID = ConfigApp.Instance.GetConfigSetting("DMD_DB_userID", "postgres");
            string sDMD_DBPassword = ConfigApp.Instance.GetConfigSetting("DMD_DB_Passward", "postgres");
            string sCMFT_DBUserID = ConfigApp.Instance.GetConfigSetting("CMFT_DB_userID", "postgres");
            string sCMFT_DBPassword = ConfigApp.Instance.GetConfigSetting("CMFT_DB_Passward", "postgres");
            string sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");
            try
            {
                //"Server='localhost'; Port='5432'; Database='DMDDB'; User Id='postgres'; Password='postgres'";
                // 嘗試初始化 DMD 資料庫連線
                if (!ASI.Wanda.DMD.DB.Manager.Initializer(sDMD_DBIP, sDMD_DBPort, sDMD_DBName, sDMD_DBUserID, sDMD_DBPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"DMD資料庫連線失敗!{sDMD_DBIP}:{sDMD_DBPort};userid={sDMD_DBUserID}");
                    return -1; // 返回錯誤代碼
                }
                // 嘗試初始化 CMFT 資料庫連線
                if (!ASI.Wanda.CMFT.DB.Manager.Initializer(sCMFT_DBIP, sCMFT_DBPort, sCMFT_DBName, sCMFT_DBUserID, sCMFT_DBPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"CMFT資料庫連線失敗!{sCMFT_DBIP}:{sCMFT_DBPort};userid={sCMFT_DBUserID}");
                    return -1; // 返回錯誤代碼
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"資料庫連線失敗! Exception: {ex.Message}");
                return -1; // 返回錯誤代碼
            }
            ConnectToDCUServer();
            StartSocketAdmin();
            return base.StartTask(pComputer, pProcName);
        }
        /// <summary>
        /// 結束處理DMD模組執行程序
        /// </summary>
        public override void StopTask()
        {
            ASI.Lib.Log.DebugLog.Log(_mProcName, "正在嘗試停止 TaskDCU...");
            if (mSocketAdmin != null)
            {
                mSocketAdmin.Stop();
                mSocketAdmin = null;
            }
            if (mDMD_API != null)
            {
                try
                {
                    // 停止 DMD API
                    mDMD_API.Dispose();
                    mDMD_API = null;
                    DisconnectExistingDMDAPI();
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "DMD API 已停止並釋放資源。");
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"停止 DMD API 時發生錯誤: {ex}");
                }
            }
            base.StopTask();
        }

        #endregion

        private void DMD_API_DisconnectedEvent(string clientInfo)
        {
            mSocketTracker.OnDisconnected(clientInfo);
            List<string> staleStations;

            lock (mClientsLock)
            {
                // 從列表中移除已斷開的客戶端
                connectedClients.Remove(clientInfo);

                // 一併移除車站對照，避免送到已斷線的目標
                staleStations = mStationClients
                                .Where(x => string.Equals(x.Value, clientInfo, StringComparison.OrdinalIgnoreCase))
                                .Select(x => x.Key)
                                .ToList();
                foreach (var stationID in staleStations)
                {
                    mStationClients.Remove(stationID);
                }
            }

            // 寫檔的 log 放到鎖外
            foreach (var stationID in staleStations)
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"車站 [{stationID}] 的連線已移除");
            }

            ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端已斷開連接: {clientInfo}");

        }

        /// <summary>
        /// 取得指定車站目前的連線目標 (IP:port)，供 DMD_API.Send(message, target) 使用。
        /// 先查連線時建立的對照，查不到再以 station_conf.dcu_ip 比對已連線清單。
        /// </summary>
        /// <param name="stationID">車站代碼，例如 "LG08A"</param>
        /// <returns>連線目標字串；該站尚未連線時回傳 null</returns>
        private string ResolveClientTarget(string stationID)
        {
            string cached;

            // 第一段：鎖內，純記憶體查對照
            lock (mClientsLock)
            {
                if (mStationClients.TryGetValue(stationID, out cached))
                    return cached;
            }

            // 第二段：鎖外，查 DB（慢動作，鎖住會卡到 socket 事件緒）
            var dcuIP = ASI.Wanda.DMD.DB.Tables.Train.stationConf.GetDcuIP(stationID);
            if (string.IsNullOrEmpty(dcuIP))
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"車站 [{stationID}] 在 station_conf 查無 dcu_ip");
                return null;
            }

            // 第三段：鎖內，比對已連線清單並寫回對照
            string match;
            lock (mClientsLock)
            {
                // 查 DB 這段期間，ConnectedEvent 可能已經建立對照
                if (mStationClients.TryGetValue(stationID, out cached))
                    return cached;

                // Server 模式下客戶端的來源 port 是動態的，只能以 IP 前綴比對
                match = connectedClients.FirstOrDefault(
                            c => !string.IsNullOrEmpty(c) &&
                                 string.Equals(c.Split(':')[0].Trim(), dcuIP, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                    mStationClients[stationID] = match;
            }

            if (match == null)
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"車站 [{stationID}] 的 DCU ({dcuIP}) 目前未連線");

            return match;
        }

        /// <summary>
        /// 將依車站分群的訊息分別送往各站的 DCU。
        /// </summary>
        /// <param name="messagesByStation">車站代碼 -&gt; 訊息物件</param>
        /// <param name="messageTypeName">訊息類型名稱，僅供記錄使用</param>
        /// <returns>成功送出的車站數</returns>
        private int SendByStation(Dictionary<string, ASI.Wanda.DMD.Message.Message> messagesByStation, string messageTypeName)
        {
            var successCount = 0;

            if (messagesByStation == null || messagesByStation.Count == 0)
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"{messageTypeName} 沒有可分送的車站目標");
                return 0;
            }

            foreach (var item in messagesByStation)
            {
                var target = ResolveClientTarget(item.Key);
                if (string.IsNullOrEmpty(target))
                {
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"{messageTypeName} 略過車站 [{item.Key}]：找不到可用連線");
                    continue;
                }

                var result = mDMD_API.Send(item.Value, target);
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"{messageTypeName} 送往車站 [{item.Key}] ({target}) 結果: {result}");
                if (result == 0)
                    successCount++;
            }

            ASI.Lib.Log.DebugLog.Log(_mProcName, $"{messageTypeName} 分送完成，成功 {successCount}/{messagesByStation.Count} 站");
            return successCount;
        }

        /// <summary>
        /// 從DCU 接收訊號
        /// </summary>
        /// <param name="DCUServerMessage"></param>
        private void DMD_API_ReceivedEvent(ASI.Wanda.DMD.Message.Message DCUServerMessage)
        {
            string sLog = "";
            try
            {
                string sRcvTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
                string sByteArray = ASI.Lib.Text.Parsing.String.BytesToHexString(DCUServerMessage.CompleteContent, "");
                string sJsonData = DCUServerMessage.JsonContent;
                string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "JsonObjectName");
                int iMsgID = DCUServerMessage.MessageID;

                if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Ack)
                {
                    ///Ack
                    sLog = string.Format("Ack，訊息識別碼:[{0}]", DCUServerMessage.MessageID);
                }
                else if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Command)
                {
                    ///Change/Command
                    sLog = $"從DCU Server收到:{sByteArray}；訊息類別碼:{DCUServerMessage.MessageType}；識別碼:{iMsgID}；長度:{DCUServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
                }
                else if (DCUServerMessage.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Response)
                {
                    sLog = $"從DCU Server收到:{sByteArray}；訊息類別碼:{DCUServerMessage.MessageType}；識別碼:{iMsgID}；長度:{DCUServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
                    ///DCU Server to DMD
                    if (sJsonObjectName == "ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage")
                    {
                        var oJsonObject = (ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(DCUServerMessage.JsonContent);
                        //失敗看板 
                        var stationsDuDictionary = new Dictionary<string, List<string>>();

                        foreach (var Targetdu in oJsonObject.failed_target)
                        {
                            var station = Targetdu.Split('_')[0];
                            if (!stationsDuDictionary.ContainsKey(station))
                            {
                                stationsDuDictionary[station] = new List<string>();
                            }
                            stationsDuDictionary[station].Add(Targetdu);
                        }
                        //要傳送的目標看板 車站 看板 
                        var deviceInfoList = oJsonObject.failed_target
                            .Where(Targetdu => Targetdu.Split('_').Length >= 3)
                            .Select(Targetdu =>
                            {
                                var parts = Targetdu.Split('_');
                                return new DeviceInfo
                                {
                                    StationID = parts[0],
                                    AreaID = parts[1],
                                    DeviceID = parts[2]
                                };
                            })
                            .ToList();
                        //將失敗的的看板從 dmd_playlist中刪除  
                        foreach (var deviceInfo in deviceInfoList)
                        {
                            ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.DeletePlayingItem(deviceInfo.StationID, deviceInfo.AreaID, deviceInfo.DeviceID);
                        }
                        //接下來回傳給Task CMFT 

                    }
                    else if (sJsonObjectName == "ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendInstantMessage")
                    {
                        var oJsonObject = (ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendInstantMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(DCUServerMessage.JsonContent);
                        //失敗看板
                        var stationDudictionary = new Dictionary<string, List<string>>();

                        foreach (var Targetdu in oJsonObject.failed_target)
                        {
                            var station = Targetdu.Split('_')[0];
                            if (!stationDudictionary.ContainsKey(station))
                            {
                                stationDudictionary[station] = new List<string>();
                            }
                            stationDudictionary[station].Add(Targetdu);
                        }
                        var deviceInfoList = oJsonObject.failed_target
                             .Where(Targetdu => Targetdu.Split('_').Length >= 3)
                             .Select(Targetdu =>
                             {
                                 var parts = Targetdu.Split('_');
                                 return new DeviceInfo 
                                 {
                                     StationID = parts[0],
                                     AreaID = parts[1],
                                     DeviceID = parts[2]
                                 };
                             })
                             .ToList();
                        //將失敗的的看板從 dmd_playlist中刪除 
                        foreach (var deviceInfo in deviceInfoList)
                        {
                            ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.DeletePlayingItem(deviceInfo.StationID, deviceInfo.AreaID, deviceInfo.DeviceID);
                        }
                        //接下來回傳給Task CMFT
                    }
                }

            }
            catch (System.Exception ex)
            {

                ASI.Lib.Log.ErrorLog.Log("TaskDCU", ex);
            }
        }

        /// <summary>
        /// 處理TaskCMFT的訊息
        /// </summary>
        private int ProMsgFromCMFT(string pMessage)
        {
            try
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskCMFT mSGFromTaskCMFT = new ProcMsg.MSGFromTaskCMFT(new MSGFrameBase(""));

                if (mSGFromTaskCMFT.UnPack(pMessage) > 0)
                {
                    string sJsonData = mSGFromTaskCMFT.JsonData;
                    string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskCMFT.JsonData, "JsonObjectName");
                    string sStationID = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskCMFT.JsonData, "StationID");
                    string sSeatID = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "SeatID");
                    int iMsgID = mSGFromTaskCMFT.MessageID;
                    ASI.Lib.Log.DebugLog.Log(_mProcName + " fromTaskCMFT", $"收到來自TaskCMFT的訊息，SeatID:{sSeatID}；MsgID:{iMsgID}；JsonObjectName:{sJsonObjectName}");

                    // 建立 Helper 並傳入當前車站 ID，用於篩選 target_du
                    var Helper = new DCUHelper(mCurrentStationID);
                    var message = new object();
                    int result;
                    //回應Ack給CMFT
                    switch (sJsonObjectName)
                    {
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPreRecordMsg: //預錄訊息 
                            // 依 target_du 的車站分群，各站以 station_conf.dcu_ip 對應的連線分別送出
                            result = SendByStation(Helper.SendPreRecordMSGToDCUByStation(mSGFromTaskCMFT), "預錄訊息");
                            ASI.Lib.Log.DebugLog.Log("預錄訊息 傳送結果", $"成功車站數:{result}");
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendInstantMsg:  //即時訊息 
                            result = SendByStation(Helper.SendInstantMSGToDCUByStation(mSGFromTaskCMFT), "即時訊息");
                            ASI.Lib.Log.DebugLog.Log("即時訊息 傳送結果", $"成功車站數:{result}");
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPreRecordMessageSetting: //預錄訊息設定  
                            message = Helper.SendPreRecordMessageSetting(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("預錄訊息設定 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendGroupSetting:      //群組設定
                            message = Helper.SendGroupSettingToDCU(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("群組設定 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendScheduleSetting:   //排程設定 
                            message = Helper.SendScheduleSettingToDCU(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("排成設定 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendTrainMessageSetting: // 列車訊息設定  
                            message = Helper.SendTrainMessageSetting(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("列車訊息設定 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPowerTimeSetting:  //電力設定   
                            message = Helper.SendPowerSettingToDCU(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("電力設定 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendParameterSetting:  //參數設定 
                            message = Helper.SendParameterSetting(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("電力設定 傳送結果", result.ToString());
                            break;
                        default:
                            break;
                    }
                }

            } 
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }

            return -1;
        }

        private int ProMsgFromOCS(string pMessage)
        {
            try
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskOCS mSGFromTaskOCS = new ProcMsg.MSGFromTaskOCS(new MSGFrameBase(""));

                if (mSGFromTaskOCS.UnPack(pMessage) > 0)
                {
                    string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskOCS.JsonData, "JsonObjectName");
                    ASI.Lib.Log.DebugLog.Log(_mProcName + " fromTaskOCS", $"收到來自TaskOCS的訊息，JsonObjectName:{sJsonObjectName}");
                    // 建立 Helper 並傳入當前車站 ID
                    var Helper = new DCUHelper(mCurrentStationID);
                    var message = new object();
                    int result;
                    message = Helper.SendOCSMSGMSGToDCU(mSGFromTaskOCS);
                    result = mDMD_API.Send((Message.Message)message);
                    ASI.Lib.Log.DebugLog.Log("來自OCS的資料 傳送結果", result.ToString());
                }

            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }

            return -1;
        }
        /// <summary>
        /// 開啟與DCU伺服器的Socket連線
        /// </summary>
        private void ConnectToDCUServer()
        {
            try
            {
                DisconnectExistingDMDAPI(); 
                mDMD_API = new ASI.Wanda.DMD.DMD_API();
                mDMD_API.ConnectedEvent += DMD_API_ConnectedEvent;
                mDMD_API.ReceivedEvent += DMD_API_ReceivedEvent;
                mDMD_API.DisconnectedEvent += DMD_API_DisconnectedEvent;
                mDMD_API.ErrorEvent += DMD_API_ErrorEvent;
                mDMD_API.ClientDataReceivedEvent += DMD_API_ClientDataReceivedEvent;
                mDMDServerConnStr = ConfigApp.Instance.GetConfigSetting("DCU_Server");

                int iResult = mDMD_API.Initial(mDMDServerConnStr);
                if (iResult == 0)
                {
                    mIsConnectedToDCU = true;
                    mSocketTracker.AddEvent("Info", null, $"DCU Socket 開啟成功 {mDMDServerConnStr}");
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "與DCU Server的Socket開啟成功");
                }
                else
                {
                    mIsConnectedToDCU = false;
                    mSocketTracker.AddEvent("Error", null, $"DCU Socket 開啟失敗 {mDMDServerConnStr}，回傳碼 {iResult}");
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"與DCU Server的Socket開啟失敗，DMD_Server: {mDMDServerConnStr}");
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"Exception in ConnToDCUServer: {ex}");
            }
        }

        private void DMD_API_ErrorEvent(Exception source)
        {
            ASI.Lib.Log.ErrorLog.Log(_mProcName, source);
        }

        private void DMD_API_ConnectedEvent(string clientInfo)
        {
            // 添加已連接的客戶端到列表
            mSocketTracker.OnConnected(clientInfo);
            lock (mClientsLock)
            {
                connectedClients.Add(clientInfo);
            }
            ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端連接成功: {clientInfo}");

            // 以來源 IP 反查 station_conf.dcu_ip，建立車站與連線的對照
            try
            {
                // DB 查詢在鎖外
                var stationID = ASI.Wanda.DMD.DB.Tables.Train.stationConf.GetStationIdByDcuIp(clientInfo);
                if (string.IsNullOrEmpty(stationID))
                {
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端 {clientInfo} 在 station_conf 中查無對應車站，將無法依車站分送");
                    return;
                }

                lock (mClientsLock)
                {
                    mStationClients[stationID] = clientInfo;
                }
                mSocketTracker.SetNote(clientInfo, stationID);
                ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端 {clientInfo} 對應車站 [{stationID}]");
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }
        }

        private void DisconnectExistingDMDAPI()  //斷線   
        {
            if (mDMD_API != null)
            {
                mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                mDMD_API.DisconnectedEvent -= DMD_API_DisconnectedEvent;
                mDMD_API.ErrorEvent -= DMD_API_ErrorEvent;
                mDMD_API.ConnectedEvent -= DMD_API_ConnectedEvent;
                mDMD_API.ClientDataReceivedEvent -= DMD_API_ClientDataReceivedEvent;
                try
                {
                    mDMD_API.Dispose();
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "Existing DMD_API disconnected and disposed.");
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"Exception in DisconnectExistingDMDAPI: {ex}");
                }
                finally
                {
                    mDMD_API = null;
                    mSocketTracker.ClearClients("DCU Socket 關閉");
                }
            }
        }

        #region Socket 連線監控 / 管理通道

        private void DMD_API_ClientDataReceivedEvent(string source, int length)
        {
            mSocketTracker.OnReceived(source, length);
        }

        /// <summary>
        /// 開啟本機管理通道，設定：Socket_Admin_IP (預設 127.0.0.1)、Socket_Admin_DCU_Port (預設 18001)
        /// </summary>
        private void StartSocketAdmin()
        {
            try
            {
                string sIP = ConfigApp.Instance.GetConfigSetting("Socket_Admin_IP");
                int iPort;
                if (!int.TryParse(ConfigApp.Instance.GetConfigSetting("Socket_Admin_DCU_Port"), out iPort) || iPort <= 0)
                {
                    iPort = SocketAdminServer.DefaultDcuPort;
                }

                mSocketAdmin = new SocketAdminServer(_mProcName, mSocketTracker, new DcuSocketAdminHandler(this));
                mSocketAdmin.Start(sIP, iPort);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }
        }

        /// <summary>
        /// 管理通道對 DMD_API (DCU Socket Server) 的實際操作
        /// </summary>
        private class DcuSocketAdminHandler : ISocketAdminHandler
        {
            private readonly ProcTaskDCU mOwner;

            public DcuSocketAdminHandler(ProcTaskDCU owner)
            {
                mOwner = owner;
            }

            public bool IsServerOpen
            {
                get
                {
                    var api = mOwner.mDMD_API;
                    return api != null && api.IsConnect;
                }
            }

            public string ListenConnStr
            {
                get { return mOwner.mDMDServerConnStr; }
            }

            public IList<string> GetSocketEndpoints()
            {
                var api = mOwner.mDMD_API;
                return api == null ? new List<string>() : api.GetClientEndpoints();
            }

            public int Kick(string endpoint)
            {
                var api = mOwner.mDMD_API;
                // 斷線後會觸發 DMD_API_DisconnectedEvent，一併清掉車站對照
                return api == null ? -4 : api.DisconnectClient(endpoint);
            }

            public int SendTo(string endpoint, int messageType, int messageId, string jsonContent)
            {
                var api = mOwner.mDMD_API;
                if (api == null) return -3;
                if (!api.GetClientEndpoints().Contains(endpoint, StringComparer.OrdinalIgnoreCase)) return -5; // 找不到該 Client

                var msg = new ASI.Wanda.DMD.Message.Message(
                    (ASI.Wanda.DMD.Message.Message.eMessageType)messageType,
                    messageId,
                    string.IsNullOrEmpty(jsonContent) ? null : jsonContent);
                return api.Send(msg, endpoint);
            }

            private List<SocketStationDef> mStationCache;
            private DateTime mStationCacheTime = DateTime.MinValue;

            /// <summary>
            /// 應連入的 DCU 車站 = station_conf 中啟用且有 dcu_ip 的車站 (萬大線 LG01 ~ LG08A 共 9 站)。
            /// 管理介面每幾秒查一次，這裡快取 30 秒避免一直打 DB。
            /// </summary>
            public IList<SocketStationDef> GetStationDefinitions()
            {
                if (mStationCache != null && (DateTime.Now - mStationCacheTime).TotalSeconds < 30)
                    return mStationCache;

                mStationCache = ASI.Wanda.DMD.DB.Tables.Train.stationConf.GetDcuStations()
                    .Select(x => new SocketStationDef
                    {
                        StationId = x.station_id,
                        Name = x.chinese,
                        Ip = x.dcu_ip.Trim()
                    })
                    .ToList();
                mStationCacheTime = DateTime.Now;
                return mStationCache;
            }
        }

        #endregion
    }
}
