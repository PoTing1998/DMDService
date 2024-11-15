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
        /// 儲存已連接的客戶端
        /// </summary>
        private List<string> connectedClients = new List<string>();
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
            // DMD Database Configuration
            string sDMD_DBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            string sDMD_DBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            string sDMD_DBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");

            // CMFT Database Configuration
            string sCMFT_DBIP = ConfigApp.Instance.GetConfigSetting("CMFT_DB_IP");
            string sCMFT_DBPort = ConfigApp.Instance.GetConfigSetting("CMFT_DB_Port");
            string sCMFT_DBName = ConfigApp.Instance.GetConfigSetting("CMFT_DB_Name");
            string sUserID = "postgres";
            string sPassword = "postgres";
            string sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");
            try
            {
                //"Server='localhost'; Port='5432'; Database='DMDDB'; User Id='postgres'; Password='postgres'";
                // 嘗試初始化 DMD 資料庫連線
                if (!ASI.Wanda.DMD.DB.Manager.Initializer(sDMD_DBIP, sDMD_DBPort, sDMD_DBName, sUserID, sPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"DMD資料庫連線失敗!{sDMD_DBIP}:{sDMD_DBPort};userid={sUserID}");
                    return -1; // 返回錯誤代碼
                }
                // 嘗試初始化 CMFT 資料庫連線
                if (!ASI.Wanda.CMFT.DB.Manager.Initializer(sCMFT_DBIP, sCMFT_DBPort, sCMFT_DBName, sUserID, sPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"CMFT資料庫連線失敗!{sCMFT_DBIP}:{sCMFT_DBPort};userid={sUserID}");
                    return -1; // 返回錯誤代碼
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"資料庫連線失敗! Exception: {ex.Message}");
                return -1; // 返回錯誤代碼
            }
            ConnectToDCUServer();
            return base.StartTask(pComputer, pProcName);
        }
        /// <summary>
        /// 結束處理DMD模組執行程序
        /// </summary>
        public override void StopTask()
        {
            ASI.Lib.Log.DebugLog.Log(_mProcName, "正在嘗試停止 TaskDCU...");
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
            // 從列表中移除已斷開的客戶端
            connectedClients.Remove(clientInfo);
            ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端已斷開連接: {clientInfo}");

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
                    var Helper = new DCUHelper();
                    var message = new object();
                    int result;
                    //回應Ack給CMFT
                    switch (sJsonObjectName)
                    {
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPreRecordMsg: //預錄訊息  
                            message = Helper.SendPreRecordMSGToDCU(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("預錄訊息 傳送結果", result.ToString());
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendInstantMsg:  //即時訊息 
                            message = Helper.SendInstantMSGToDCU(mSGFromTaskCMFT);
                            result = mDMD_API.Send((Message.Message)message);
                            ASI.Lib.Log.DebugLog.Log("即時訊息 傳送結果", result.ToString());
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
                mDMDServerConnStr = ConfigApp.Instance.GetConfigSetting("DCU_Server");

                int iResult = mDMD_API.Initial(mDMDServerConnStr);
                if (iResult == 0)
                {
                    mIsConnectedToDCU = true;
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "與DCU Server的Socket開啟成功");
                }
                else
                {
                    mIsConnectedToDCU = false;
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
            connectedClients.Add(clientInfo);
            ASI.Lib.Log.DebugLog.Log(_mProcName, $"客戶端連接成功: {clientInfo}");
        }

        private void DisconnectExistingDMDAPI()
        {
            if (mDMD_API != null)
            {
                mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                mDMD_API.DisconnectedEvent -= DMD_API_DisconnectedEvent;
                mDMD_API.ErrorEvent -= DMD_API_ErrorEvent;
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
                }
            }
        }
        #region   判斷方式
        //判別要傳送的目的碼  
        //private void DetermineSendDestination(ASI.Wanda.DMD.Message.Message message)
        //{
        //    try
        //    {
        //        if (message.JsonContent == null)
        //        {
        //            ASI.Lib.Log.ErrorLog.Log(_mProcName, "JsonContent is null. Unable to determine send destination.");
        //            return;
        //        }
        //        // 解析 JsonContent   
        //        var jsonObject = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)ASI.Lib.Text.Parsing.Json.DeserializeObject(message.JsonContent, typeof(ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage));
        //        // 提取 target_du  
        //        var targetDuList = jsonObject.target_du; 
        //        //讀取現有連線的clinet
        //        var Client = mDMD_API.ClientIDList;

        //        foreach (var clientEntry in Client)
        //        {
        //            string clientIPPort = clientEntry.Value;
        //            //  clientIPPort 的格式為 "IP:Port"  
        //            string[] clientParts = clientIPPort.Split(':');

        //            if (clientParts.Length == 2)
        //            {
        //                string clientIP = clientParts[0];

        //                // 在這裡進行相應的處理，例如取得 stationID  
        //                foreach (var targetDu in targetDuList)
        //                {
        //                    // 在這裡進行字串操作，提取 stationID    
        //                    string[] parts = targetDu.Split('_');
        //                    if (parts.Length >= 1)
        //                    {
        //                        // parts[0] 就是 stationID   
        //                        string stationID = parts[0];

        //                        mDMD_API.Send(message, clientIPPort);
        //                        ASI.Lib.Log.DebugLog.Log("DetermineSendDestination", $"Connected client to the target IP: {clientIP}");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ASI.Lib.Log.ErrorLog.Log("DetermineSendDestination", ex);
        //        ASI.Lib.Log.DebugLog.Log("DetermineSendDestination", $"Failed to determine send destination. Message: {message.JsonContent}");
        //    }

        //}
        #endregion
    }
}
