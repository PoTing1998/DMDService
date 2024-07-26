using ASI.Lib.Config;
using ASI.Lib.DB;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Wanda.DMD.ProcMsg;

using System;

namespace ASI.Wanda.DMD.TaskCMFT
{
    /// <summary>
    /// 處理CMFT模組執行程序
    /// </summary>
    public class ProcTaskCMFT : ProcBase
    {
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
                return  ProMsgFromDCU(pBody);
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

                    if (mIsConnectedToCMFT)
                    { 
                        //若原本為連線，則檢查目前連線狀態
                        //超過60秒未收到DMD傳送的訊息則判定為離線
                     //   string sStatusType = "default";
                        string sStatusValue = true.ToString();
                        if (System.DateTime.Now.Subtract(LastHeartbeatTime).TotalSeconds > 60)
                        {
                            sStatusValue = false.ToString();
                        }
                    }
                    else
                    {
                        //嘗試重新連線 
                     //   ConnToCMFTServer();
                    }
                    //string sCMFTServerIP = ASI.Lib.Config.ConfigApp.Instance.GetIPFromConnStr(this.mCMFTServerConnStr);

                    //bool bStatus = ASI.Lib.Comm.Network.NetworkLib.TryPing(sCMFTServerIP, 300, 4);
                    //ASI.Lib.Log.DebugLog.Log(mProcName, $"嘗試ping CMFT Server IP = {sCMFTServerIP}，連線狀態:{bStatus}");
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                }
            }

            //檢查排程設定是否需進行排程訊息播放 todo
            //讀取資料庫
            //送出訊息播放命令給CMFT Server

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
            mProcName = "TaskCMFT";
            
            string sDBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            string sDBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            string sDBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");
            string sUserID = "postgres";
            string sPassword = "postgres"; 
            string sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");

            try
            {
                //"Server='localhost'; Port='5432'; Database='DMDDB'; User Id='postgres'; Password='postgres'";
                if (!ASI.Wanda.DMD.DB.Manager.Initializer(sDBIP, sDBPort, sDBName, sUserID, sPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID}");
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID};ex={ex}");
            }

            ConnToCMFTServer(); 
            
            return base.StartTask(pComputer, pProcName);   
        }
              
        private void CMFT_API_DisconnectedEvent(string source)
        {
            //string sStatusType = "default"; 
            //string sStatusValue = false.ToString();
        } 

        /// <summary>
        /// 從CMFT 接收訊息   
        /// </summary>
        /// <param name="DMDServerMessage"></param> 
        private void CMFT_API_ReceivedEvent(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
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
                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Ack:

                        HandleAckMessage(CMFTServerMessage, CMFTHelper);
                        var MSG = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Ack, CMFTServerMessage.MessageID, null);
                        mCMFT_API.Send(MSG);
                        break;

                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Command:
                        var MSG2 = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Ack, CMFTServerMessage.MessageID, null);
                        mCMFT_API.Send(MSG2);
                        HandleCommandMessage(CMFTServerMessage, sByteArray, sJsonObjectName, iMsgID, sJsonData, CMFTHelper);
                        break;

                    case ASI.Wanda.CMFT.Message.Message.eMessageType.Response:
                        ASI.Lib.Log.ErrorLog.Log(mProcName, $"從HMI來的訊息不應有Response，MessageType:{CMFTServerMessage.MessageType}");
                        break;

                    default:
                        ASI.Lib.Log.ErrorLog.Log(mProcName, $"無此種訊息類別:[{CMFTServerMessage.MessageType}]");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("TaskDMD", ex);
            }
        }

        /// <summary>
        /// 處理Ack訊息
        /// </summary>
        private void HandleAckMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            string sLog = $"Ack，訊息識別碼:[{CMFTServerMessage.MessageID}]";
            ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");
            CMFTHelper.HandleAckMessage(CMFTServerMessage);
        }
        /// <summary>
        /// 處理Command訊息
        /// </summary>
        private void HandleCommandMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string sByteArray, string sJsonObjectName, int iMsgID, string sJsonData, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            string sLog = $"從CMFT Server收到:{sByteArray}；訊息類別碼:{CMFTServerMessage.MessageType}；識別碼:{iMsgID}；長度:{CMFTServerMessage.MessageLength}；內容:{sJsonData}；JsonObjectName:{sJsonObjectName}";
            ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");

            switch (sJsonObjectName)
            {
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPreRecordMsg:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendInstantMsg:
                    HandlePreRecordOrInstantMsg(CMFTServerMessage, CMFTHelper);
                    break;

                case ASI.Wanda.DMD.TaskCMFT.Constants.SendScheduleSetting:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPreRecordMessageSetting:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendTrainMessageSetting:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendPowerTimeSetting:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendGroupSetting:
                case ASI.Wanda.DMD.TaskCMFT.Constants.SendParameterSetting:
                    CMFTHelper.HandleAckMessage(CMFTServerMessage);
                    break;
            }
        }

        /// <summary>
        /// 處理預錄或即時訊息
        /// </summary>
        private void HandlePreRecordOrInstantMsg(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, CMFTHelper<ASI.Wanda.CMFT.CMFT_API> CMFTHelper)
        {
            CMFTHelper.UpdateDMDPlayList();
            CMFTHelper.UpdataDMDPreRecordMessage();
          //  CMFTHelper.UpdataConfig();
            CMFTHelper.SendPreRecordMSGToDCU(CMFTServerMessage);
        }
        /// <summary>
        /// 結束處理DMD模組執行程序
        /// </summary>
        public override void StopTask()
        {
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
            DataBase oDB = null;
            string sLog = "";
            try
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU mSGFromTaskDCU = new MSGFromTaskDCU(new MSGFrameBase(""));
                if (mSGFromTaskDCU.UnPack(pMessage) >0)
                {
                    if (mSGFromTaskDCU.MessageType == 1)
                    {
                        //DMD內部通訊定義:Ack  
                        //從TaskDCU過來不應該有Ack
                        ASI.Lib.Log.ErrorLog.Log(mProcName, $"從TaskDCU來的訊息不應有DMD內部通訊定義:Ack，MessageType:{mSGFromTaskDCU.MessageType}"); ;
                    }
                    else if (mSGFromTaskDCU.MessageType == 2)  
                    {
                        //DMD內部通訊定義:Change/Command  
                        string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskDCU.JsonData, "JsonObjectName");
                        sLog = $"sJsonObjectName = {sJsonObjectName}"; 
                        ASI.Lib.Log.DebugLog.Log(mProcName, sLog);
                    } 
                    else if (mSGFromTaskDCU.MessageType == 3) 
                    {
                        //DMD內部通訊定義:Response 
                        string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskDCU.JsonData, "JsonObjectName");
                        sLog = $"sJsonObjectName = {sJsonObjectName}";
                        ASI.Lib.Log.DebugLog.Log(mProcName, sLog);
                        //將訊息傳給CMFT 
                        var oJsonObject = (ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(mSGFromTaskDCU.JsonData);

                        //組封包  
                        var Res_SendPreRecordMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDCU.Res_SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
                        Res_SendPreRecordMessage.seatID = oJsonObject.seatID;
                        Res_SendPreRecordMessage.msg_id = oJsonObject.msg_id; 
                        Res_SendPreRecordMessage.failed_target = oJsonObject.failed_target;
                        
                        //組成給DCU的封包  
                        var MSG = new CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Response, mSGFromTaskDCU.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(Res_SendPreRecordMessage));
                        mCMFT_API.Send(MSG);
                        ASI.Lib.Log.DebugLog.Log("RES_SendPreRecordMSGToDCU", MSG.JsonContent);
                    }
                    else
                    {
                        //無此種訊息類別 
                        ASI.Lib.Log.ErrorLog.Log(mProcName, $"無此種訊息類別，MessageType:{mSGFromTaskDCU.MessageType}");
                    }
                }   
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
            }
            finally
            {
                if (oDB != null)
                {
                    oDB.Close();
                }
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
                    mCMFT_API.Dispose();
                }

                mCMFT_API = new ASI.Wanda.CMFT.CMFT_API();
                mCMFT_API.ReceivedEvent += CMFT_API_ReceivedEvent;
                mCMFT_API.DisconnectedEvent += CMFT_API_DisconnectedEvent;
                mCMFTServerConnStr = ConfigApp.Instance.GetConfigSetting("CMFT_Server");
                int iResult = mCMFT_API.Initial(mCMFTServerConnStr, "CMFT");
                if (iResult == 0)
                {
                    mIsConnectedToCMFT = true;
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"與CMFT Server連線成功"); 
                    //連線成功時，重新計算最後一次收到DMD的時間  
                    LastHeartbeatTime = System.DateTime.Now;
                }
                else
                {
                    mIsConnectedToCMFT = false;
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"與CMFT Server連線失敗，DMD_Server:{mCMFTServerConnStr}");
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
            }
        }   

    }
}
