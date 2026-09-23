using ASI.Lib.Comm.Socket.Admin;
using ASI.Lib.Config;
using ASI.Lib.DB;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Wanda.DMD.ProcMsg;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Wanda.DMD.TaskCMFT
{
    /// <summary>
    /// 處理CMFT模組執行程序
    /// </summary>
    public class ProcTaskCMFT : ProcBase
    {

        #region 與CMFT的狀態設定
        private ASI.Wanda.CMFT.CMFT_API mCMFT_API = null;

        /// <summary>
        /// 最後一次收到CMFT訊息的時間
        /// </summary>
        private System.DateTime LastHeartbeatTime = System.DateTime.Now;

        /// <summary>
        /// 與DMD Server的連線狀態
        /// </summary>
        private bool mIsConnectedToCMFT = false;

        public string mCMFTServerConnStr = "";

        /// <summary>
        /// 記錄連入 CMFT Socket Server 的 Client，供 UITest「Socket 連線監控」查詢
        /// </summary>
        private readonly SocketClientTracker mSocketTracker = new SocketClientTracker("CMFT");

        /// <summary>
        /// 本機管理通道 (預設 127.0.0.1:18000)
        /// </summary>
        private SocketAdminServer mSocketAdmin = null;

        /// <summary>
        /// 定時送 Heartbeat 給 CMFT 並檢查逾時
        /// </summary>
        private System.Threading.Timer mHeartbeatTimer = null;
        private int mHeartbeatBusy = 0;

        /// <summary>Heartbeat 間隔秒數 (Config: CMFT_Heartbeat_Interval，預設 7，0 = 不送)</summary>
        private int mHeartbeatIntervalSec = 7;

        /// <summary>超過此秒數沒收到 CMFT 任何資料視為斷線 (Config: CMFT_Heartbeat_Timeout，預設 60，0 = 不檢查)</summary>
        private int mHeartbeatTimeoutSec = 60;

        /// <summary>預期連入的 CMFT IP (Config: CMFT_Client_IP)，空白 = 不檢查</summary>
        private string mCMFTClientIP = "";
        /// <summary>
        /// 處理CMFT模組執行程序所收到之訊息
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
            else if (pLabel == MSGFromTaskDCU.Label)
            {
                return ProMsgFromDCU(pBody);
            }
            return base.ProcEvent(pLabel, pBody);
        }
        /// <summary>
        /// 處理CMFT模組執行程序所收到之定時訊息  
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
        public override int ProcTimerEvent(string pMessage) // handle timer message
        {
            //定時回報TaskMain 
            if (base.ProcTimerEvent(pMessage) <= 0)
            {
                return -1; 
            }

            //ping CMFT Server 
            if (this.mCMFTServerConnStr != "")
            {
                try
                {
                    // 連線逾時檢查改由 HeartbeatTick 處理 (每個 Heartbeat 週期檢查一次)
                    if (!mIsConnectedToCMFT)
                    {
                        //嘗試重新連線 
                         ConnToCMFTServer();
                    }
                    
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
                }
            }

            //檢查排程設定是否需進行排程訊息播放 todo
            //讀取資料庫
            return 1;
        }

        /// <summary> 
        /// 啟始處理CMFT模組執行程序 
        /// </summary>
        /// <param name="pComputer"></param>
        /// <param name="pProcName"></param>
        /// <returns></returns>
        public override int StartTask(string pComputer, string pProcName)
        {
            mTimerTick = 30;
            _mProcName = "TaskCMFT";
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
            ConnToCMFTServer();
            StartSocketAdmin();
            StartHeartbeat();

            return base.StartTask(pComputer, pProcName);
        }
        /// <summary>
        /// 斷線處理
        /// </summary>
        /// <param name="source"></param>
        private void CMFT_API_DisconnectedEvent(string source)
        {
            System.DateTime time = DateTime.Now;
            ASI.Lib.Log.ErrorLog.Log("CMFT_API", $"斷線 {source} {time}");
            mSocketTracker.OnDisconnected(source);

            // Server 模式下 DisconnectedEvent 代表「某一個 Client 離線」，Server 本身仍在監聽。
            // 若在此設為 false，ProcTimerEvent 會整個重建 Server，把其他 Client 全部斷掉
            // （包含從管理介面踢除單一 Client 時），因此只有 Client 模式才觸發重連。
            if (!IsCMFTServerMode())
            {
                mIsConnectedToCMFT = false; // 更新連線狀態，讓 ProcTimerEvent 觸發重連
            }
        }

        private bool IsCMFTServerMode()
        {
            return mCMFTServerConnStr != null &&
                   mCMFTServerConnStr.IndexOf("Type=Server", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void CMFT_API_ConnectedEvent(string source)
        {
            ASI.Lib.Log.DebugLog.Log(_mProcName, $"CMFT Client 連線: {source}");
            mSocketTracker.OnConnected(source);

            if (!string.IsNullOrEmpty(mCMFTClientIP))
            {
                string sIP = SocketClientTracker.GetIp(source);
                if (string.Equals(sIP, mCMFTClientIP, StringComparison.OrdinalIgnoreCase))
                {
                    mSocketTracker.SetNote(source, "CMFT");
                }
                else
                {
                    // 只記錄警告不拒絕，避免 CMFT 換備援機時被擋掉
                    mSocketTracker.SetNote(source, "非預期來源");
                    mSocketTracker.AddEvent("Error", source, $"連入來源不是設定的 CMFT_Client_IP ({mCMFTClientIP})");
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"CMFT Socket 有非預期來源連入: {source}，預期為 {mCMFTClientIP}");
                }
            }
        }

        private void CMFT_API_ClientDataReceivedEvent(string source, int length)
        {
            mSocketTracker.OnReceived(source, length);
        }

        /// <summary>
        /// 從CMFT 接收訊息
        /// </summary>
        /// <param name="DMDServerMessage"></param> 
        private void CMFT_API_ReceivedEvent(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
            // 收到 CMFT 任何訊息 (含 Heartbeat) 都算連線正常
            LastHeartbeatTime = System.DateTime.Now;
            try
            {
                string sRcvTime = System.DateTime.Now.ToString("HH:mm:ss.fff");
                string sByteArray = ASI.Lib.Text.Parsing.String.BytesToHexString(CMFTServerMessage.CompleteContent, "");
                string sJsonData = CMFTServerMessage.JsonContent;
                string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "JsonObjectName");
                int iMsgID = CMFTServerMessage.MessageID;
                //建立CMFTHelper並將 CMFT_API的send 委派 
                var CMFTHelper = new CMFTHelper<ASI.Wanda.CMFT.CMFT_API>(mCMFT_API, (api, message) => api.Send(message));
                switch (CMFTServerMessage.MessageType)
                {
                    case CMFT.Message.Message.eMessageType.Heartbeat:
                        ASI.Lib.Log.DebugLog.Log(_mProcName, "與CMFT_Server的heartBeat連線" + LastHeartbeatTime.ToString());
                        break;
                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Ack:
                        var MSG = HandleAckMessage(CMFTServerMessage);//回復ACK
                        mCMFT_API.Send(MSG);
                        break;
                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Command:
                        HandleCommandMessage(CMFTServerMessage, sByteArray, sJsonObjectName, iMsgID, sJsonData, CMFTHelper); //處理訊息
                        var Rs_Ack = HandleAckMessage(CMFTServerMessage); // 回復ACK
                        mCMFT_API.Send(Rs_Ack);
                        ASI.Lib.Log.DebugLog.Log(_mProcName, $"已送出 ACK 給 CMFT，MessageID={CMFTServerMessage.MessageID}");
                        break;
                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Response:
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"從CMFT來的訊息不應有Response，MessageType:{CMFTServerMessage.MessageType}");
                        break;
                    default: 
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"無此種訊息類別:[{CMFTServerMessage.MessageType}]");
                        break;
                } 
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("TaskCMFT", ex); 
            }
        }
        #endregion

        #region task設定及訊息的處理

        /// <summary>
        /// 處理Ack訊息
        /// </summary>
        private ASI.Wanda.CMFT.Message.Message HandleAckMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
            string sLog = $"Ack，訊息識別碼:[{CMFTServerMessage.MessageID}]";
            ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");
            return new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Ack, CMFTServerMessage.MessageID, null);
        }
        /// <summary>
        /// 處理Command訊息
        /// </summary>
        private void HandleCommandMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string sByteArray, string sJsonObjectName, int iMsgID, string sJsonData, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            string sLog = $"從CMFT Server收到:{sByteArray}；訊息類別碼:{CMFTServerMessage.MessageType}；識別碼:{iMsgID}；長度:{CMFTServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
            ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");
            // 處理不同的 JSON 物件類型 
            if (string.IsNullOrEmpty(sJsonObjectName)) return; // 基本檢查 

            switch (sJsonObjectName) 
            {
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPreRecordMsg:
                    HandlePreRecord(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendInstantMsg:
                    HandleInstantMsg(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendScheduleSetting:
                    HandleSchedule(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPreRecordMessageSetting:
                    HandlePreRecordMessageSetting(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendTrainMessageSetting:
                    HandleTrainMessageSetting(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPowerTimeSetting:
                    HandlePowerSetting(CMFTServerMessage, CMFTHelper);
                    break; 
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendGroupSetting:
                    HandleGroupSetting(CMFTServerMessage, CMFTHelper);
                    break;
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendParameterSetting: 
                    CMFTHelper.HandleAckMessage(CMFTServerMessage);
                    break;
                default:
                    ASI.Lib.Log.DebugLog.Log("收到未知 UnknownJsonObject", $"無法處理的 jsonObjectName: {sJsonObjectName}");
                    break;
            }
        }
    
        /// <summary>
        /// 結束處理DMD模組執行程序
        /// </summary>
        public override void StopTask()
        {
            if (mHeartbeatTimer != null)
            {
                mHeartbeatTimer.Dispose();
                mHeartbeatTimer = null;
            }
            if (mSocketAdmin != null)
            {
                mSocketAdmin.Stop();
                mSocketAdmin = null;
            }
            if (mCMFT_API != null)
            {
                mCMFT_API.Dispose();
                mCMFT_API = null;
            }
            base.StopTask();
        }
        /// <summary>
        /// 處理TaskDCU的訊息
        /// </summary> 
        private int ProMsgFromDCU(string pMessage)
        {
            string sLog = "";
            try
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU mSGFromTaskDCU = new MSGFromTaskDCU(new MSGFrameBase(""));
                if (mSGFromTaskDCU.UnPack(pMessage) > 0)
                {
                    if (mSGFromTaskDCU.MessageType == 1)
                    {
                        //DMD內部通訊定義:Ack
                        //從TaskDCU過來不應該有Ack
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"從TaskDCU來的訊息不應有DMD內部通訊定義:Ack，MessageType:{mSGFromTaskDCU.MessageType}，JsonData:{mSGFromTaskDCU.JsonData}");
                    }
                    else if (mSGFromTaskDCU.MessageType == 2)
                    {
                        //DMD內部通訊定義:Change/Command
                        string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskDCU.JsonData, "JsonObjectName");
                        sLog = $"收到 TaskDCU Command，JsonObjectName={sJsonObjectName}，JsonData={mSGFromTaskDCU.JsonData}";
                        ASI.Lib.Log.DebugLog.Log(_mProcName, sLog);
                    }
                    else if (mSGFromTaskDCU.MessageType == 3)
                    {
                        //DMD內部通訊定義:Response
                        string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskDCU.JsonData, "JsonObjectName");
                        sLog = $"收到 TaskDCU Response，JsonObjectName={sJsonObjectName}，JsonData={mSGFromTaskDCU.JsonData}";
                        ASI.Lib.Log.DebugLog.Log(_mProcName, sLog);

                        // 依 JsonObjectName 決定如何處理回應，避免強制 cast 造成 InvalidCastException
                        var rawObject = ASI.Wanda.DMD.Message.Helper.GetJsonObject(mSGFromTaskDCU.JsonData);
                        if (rawObject is ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage oPreRecord)
                        {
                            var resMsg = new ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
                            resMsg.seatID = oPreRecord.seatID;
                            resMsg.msg_id = oPreRecord.msg_id;
                            resMsg.failed_target = oPreRecord.failed_target;
                            var MSG = new CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Response, mSGFromTaskDCU.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(resMsg));
                            mCMFT_API.Send(MSG);
                            ASI.Lib.Log.DebugLog.Log(_mProcName, $"已送出 Response 給 CMFT（Res_SendPreRecordMessage），MessageID={mSGFromTaskDCU.MessageID}，JsonContent={MSG.JsonContent}");
                        }
                        else
                        {
                            ASI.Lib.Log.ErrorLog.Log(_mProcName, $"無法處理的 Response 類型，JsonObjectName={sJsonObjectName}");
                        }
                    }
                    else
                    {
                        //無此種訊息類別
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"無此種訊息類別，MessageType:{mSGFromTaskDCU.MessageType}");
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
        /// 與CMFT Server連線 
        /// </summary>
        private void ConnToCMFTServer()
        {
            try
            {
                if (mCMFT_API != null)
                {
                    mCMFT_API.ReceivedEvent -= CMFT_API_ReceivedEvent; 
                    mCMFT_API.DisconnectedEvent -= CMFT_API_DisconnectedEvent;
                    mCMFT_API.ConnectedEvent -= CMFT_API_ConnectedEvent;
                    mCMFT_API.ClientDataReceivedEvent -= CMFT_API_ClientDataReceivedEvent;
                    mCMFT_API.Dispose();
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "Existing CMFT_API disconnected and disposed.");
                    mSocketTracker.ClearClients("CMFT Socket 重新建立");
                }

                mCMFT_API = new ASI.Wanda.CMFT.CMFT_API();
                mCMFT_API.ReceivedEvent += CMFT_API_ReceivedEvent;
                mCMFT_API.DisconnectedEvent += CMFT_API_DisconnectedEvent;
                mCMFT_API.ConnectedEvent += CMFT_API_ConnectedEvent;
                mCMFT_API.ClientDataReceivedEvent += CMFT_API_ClientDataReceivedEvent;
                mCMFTServerConnStr = ConfigApp.Instance.GetConfigSetting("CMFT_Server");
                int iResult = mCMFT_API.Initial(mCMFTServerConnStr, "CMFT");
                if (iResult == 0)
                {
                    mIsConnectedToCMFT = true;
                    LastHeartbeatTime = System.DateTime.Now;
                    mSocketTracker.AddEvent("Info", null, $"CMFT Socket 開啟成功 {mCMFTServerConnStr}");
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"與CMFT Server連線成功");
                    //連線成功時，重新計算最後一次收到DMD的時間  
                    LastHeartbeatTime = System.DateTime.Now;
                }
                else
                {
                    mIsConnectedToCMFT = false;
                    mSocketTracker.AddEvent("Error", null, $"CMFT Socket 開啟失敗 {mCMFTServerConnStr}，回傳碼 {iResult}");
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"與CMFT Server連線失敗，DMD_Server:{mCMFTServerConnStr}");
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }
        }

        #endregion

        #region Socket 連線監控 / 管理通道

        /// <summary>
        /// 讀取 Heartbeat 設定並啟動計時器
        /// </summary>
        private void StartHeartbeat()
        {
            int iValue;
            if (int.TryParse(ConfigApp.Instance.GetConfigSetting("CMFT_Heartbeat_Interval"), out iValue) && iValue >= 0)
                mHeartbeatIntervalSec = iValue;
            if (int.TryParse(ConfigApp.Instance.GetConfigSetting("CMFT_Heartbeat_Timeout"), out iValue) && iValue >= 0)
                mHeartbeatTimeoutSec = iValue;
            mCMFTClientIP = ConfigApp.Instance.GetConfigSetting("CMFT_Client_IP", "");

            // 只做逾時檢查時仍需計時器，間隔用 5 秒
            int iPeriodSec = mHeartbeatIntervalSec > 0 ? mHeartbeatIntervalSec : 5;
            if (mHeartbeatIntervalSec == 0 && mHeartbeatTimeoutSec == 0)
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName, "CMFT Heartbeat 與逾時檢查皆已停用");
                return;
            }

            mHeartbeatTimer = new System.Threading.Timer(HeartbeatTick, null, iPeriodSec * 1000, iPeriodSec * 1000);
            ASI.Lib.Log.DebugLog.Log(_mProcName,
                $"CMFT Heartbeat 啟動：間隔 {mHeartbeatIntervalSec} 秒，逾時 {mHeartbeatTimeoutSec} 秒，預期來源 IP [{mCMFTClientIP}]");
        }

        private void HeartbeatTick(object state)
        {
            // 前一次還沒跑完就跳過
            if (System.Threading.Interlocked.Exchange(ref mHeartbeatBusy, 1) == 1) return;
            try
            {
                var api = mCMFT_API;
                if (api == null) return;

                bool bServerMode = IsCMFTServerMode();
                bool bHasPeer = bServerMode ? api.GetClientEndpoints().Count > 0 : api.IsConnect;
                if (!bHasPeer) return;

                // 1. 送 Heartbeat (Server 模式會送給所有連入的 CMFT)
                if (mHeartbeatIntervalSec > 0)
                {
                    var msg = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Heartbeat, 0, "連線中");
                    int iRtn = api.Send(msg);
                    if (iRtn != 0)
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"送出 CMFT Heartbeat 失敗，回傳碼 {iRtn}");
                }

                // 2. 逾時檢查
                if (mHeartbeatTimeoutSec <= 0) return;
                var now = System.DateTime.Now;

                if (bServerMode)
                {
                    // 各連線分別判斷，逾時就斷開，讓 CMFT 重新連入
                    foreach (var client in mSocketTracker.GetClients())
                    {
                        var last = client.LastReceivedAt ?? client.ConnectedAt;
                        if ((now - last).TotalSeconds <= mHeartbeatTimeoutSec) continue;

                        string sLog = $"超過 {mHeartbeatTimeoutSec} 秒未收到 CMFT 資料，自動斷線 (最後收到 {last:HH:mm:ss})";
                        mSocketTracker.AddEvent("Kick", client.Endpoint, sLog);
                        ASI.Lib.Log.ErrorLog.Log(_mProcName, $"{client.Endpoint} {sLog}");
                        api.DisconnectClient(client.Endpoint);
                    }
                }
                else if (mIsConnectedToCMFT && (now - LastHeartbeatTime).TotalSeconds > mHeartbeatTimeoutSec)
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, $"超過 {mHeartbeatTimeoutSec} 秒未收到 CMFT 資料，判定離線，下次定時檢查時重新連線");
                    mIsConnectedToCMFT = false;
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref mHeartbeatBusy, 0);
            }
        }

        /// <summary>
        /// 開啟本機管理通道，設定：Socket_Admin_IP (預設 127.0.0.1)、Socket_Admin_CMFT_Port (預設 18000)
        /// </summary>
        private void StartSocketAdmin()
        {
            try
            {
                string sIP = ConfigApp.Instance.GetConfigSetting("Socket_Admin_IP");
                int iPort;
                if (!int.TryParse(ConfigApp.Instance.GetConfigSetting("Socket_Admin_CMFT_Port"), out iPort) || iPort <= 0)
                {
                    iPort = SocketAdminServer.DefaultCmftPort;
                }

                mSocketAdmin = new SocketAdminServer(_mProcName, mSocketTracker, new CmftSocketAdminHandler(this));
                mSocketAdmin.Start(sIP, iPort);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
            }
        }

        /// <summary>
        /// 管理通道對 CMFT_API 的實際操作
        /// </summary>
        private class CmftSocketAdminHandler : ISocketAdminHandler
        {
            private readonly ProcTaskCMFT mOwner;

            public CmftSocketAdminHandler(ProcTaskCMFT owner)
            {
                mOwner = owner;
            }

            public bool IsServerOpen
            {
                get
                {
                    var api = mOwner.mCMFT_API;
                    return api != null && api.IsConnect;
                }
            }

            public string ListenConnStr
            {
                get { return mOwner.mCMFTServerConnStr; }
            }

            public IList<string> GetSocketEndpoints()
            {
                var api = mOwner.mCMFT_API;
                return api == null ? new List<string>() : api.GetClientEndpoints();
            }

            public int Kick(string endpoint)
            {
                var api = mOwner.mCMFT_API;
                return api == null ? -4 : api.DisconnectClient(endpoint);
            }

            public int SendTo(string endpoint, int messageType, int messageId, string jsonContent)
            {
                var api = mOwner.mCMFT_API;
                if (api == null) return -3;
                if (!api.GetClientEndpoints().Contains(endpoint, StringComparer.OrdinalIgnoreCase)) return -5; // 找不到該 Client

                var msg = new ASI.Wanda.CMFT.Message.Message(
                    (ASI.Wanda.CMFT.Message.Message.eMessageType)messageType,
                    messageId,
                    string.IsNullOrEmpty(jsonContent) ? null : jsonContent);
                return api.Send(msg, endpoint);
            }

            public IList<SocketStationDef> GetStationDefinitions()
            {
                // CMFT 沒有固定的站點清單
                return null;
            }
        }

        #endregion

        #region 資料庫的操作
        /// <summary>
        /// 處理預錄訊息
        /// </summary>
        private void HandlePreRecord(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpdateDMDPlayList();
            CMFTHelper.UpdataDMDPreRecordMessage();
            CMFTHelper.UpdataConfig();
            CMFTHelper.SendPreRecordMSGToDCU(CMFTServerMessage);
        }

        /// <summary>
        /// 處理即時訊息
        /// </summary>
        private void HandleInstantMsg(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpdateDMDPlayList();
            CMFTHelper.UpdataDMDInstantMessage();
            CMFTHelper.UpdataConfig();
            CMFTHelper.SendInstantMSGToDCU(CMFTServerMessage);
        }
        /// <summary>
        /// 處理電源設定
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <param name="CMFTHelper"></param> 
        private void HandlePowerSetting(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpDateDMDPowerSetting();
            CMFTHelper.SendPowerSettingToDCU(CMFTServerMessage);
        }
        /// <summary>
        /// 處理預錄訊息設定
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <param name="CMFTHelper"></param>
        private void HandlePreRecordMessageSetting(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpdataDMDPreRecordMessage();
            CMFTHelper.SendPreRecordMessageSettingToDCU(CMFTServerMessage);
        }

        /// <summary>
        /// 處理預錄訊息的排成
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <param name="CMFTHelper"></param>
        private void HandleSchedule(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpSchedule();
            CMFTHelper.UpDMDSchedulePlaylist();
            CMFTHelper.SendScheduleSettingToDCU(CMFTServerMessage);
        }

        /// <summary>
        /// 處理列車訊息
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <param name="CMFTHelper"></param>
        private void HandleTrainMessageSetting(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpDateDMDTrainMessage();
            CMFTHelper.SendTrainMessageToDCU(CMFTServerMessage);
        }

        /// <summary>
        /// 處理群駔設定
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <param name="CMFTHelper"></param>
        private void HandleGroupSetting(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpDateDMDGroupTarget();
            CMFTHelper.SendGroupSettingToDCU(CMFTServerMessage);
        }
        #endregion
    }
}
