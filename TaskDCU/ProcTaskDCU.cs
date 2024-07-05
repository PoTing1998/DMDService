using ASI.Lib.Config;
using ASI.Lib.DB;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Wanda.DMD.ProcMsg;

using System;
using System.Collections.Generic;
using System.Linq;



namespace ASI.Wanda.DMD.TaskDCU
{
    public class ProcTaskDCU : ProcBase
    {
        private ASI.Wanda.DMD.DMD_API mDMD_API = null;

        private System.DateTime LastHeartbeatTime = System.DateTime.Now;

        public string mDMDServerConnStr = "";


        public bool mIsConnectedToDCU = false;


        static private Dictionary<string, int> mServerIP = null;

        public class DeviceInfo 
        {
            public string StationID { get; set; }
            public string AreaID { get; set; }
            public string DeviceID { get; set; }
        }
        DMD.Enum.Station Station = new Enum.Station();
        
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


        /// <summary>
        /// 啟始處理DCU模組執行程序
        /// </summary>
        /// <param name="pComputer"></param>
        /// <param name="pProcName"></param>
        /// <returns></returns>  
        public override int StartTask(string pComputer, string pProcName)
        {
            mTimerTick = 30;
            mProcName = "TaskDCU";
            mServerIP = new Dictionary<string, int>();
            string sDBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            string sDBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            string sDBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");
            string sUserID = "postgres";
            string sPassword = "postgres";
            string sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");

            try
            {
                //"Server='localhost'; Port='5432'; Database='DCUDB'; User Id='postgres'; Password='postgres'"; 
                if (!ASI.Wanda.DMD.DB.Manager.Initializer(sDBIP, sDBPort, sDBName, sUserID, sPassword, sCurrentUserID))
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID}");
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID};ex={ex}");
            }
            return base.StartTask(pComputer, pProcName);
        }
        public void SendMessage()
        {
            try
            {
                var MSGFromTaskCMFT = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU(new MSGFrameBase("TaskDCU", "TaskCMFT")); 
                //組相對應的封包
                MSGFromTaskCMFT.MessageType = 1;
                MSGFromTaskCMFT.MessageID = 1;
                MSGFromTaskCMFT.JsonData = "";
                ASI.Lib.Process.ProcMsg.SendMessage(MSGFromTaskCMFT);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DMD_API_DisconnectedEvent(string source)
        {
            //string sStatusType = "default";
            //string sStatusValue = false.ToString();
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
                    ///Response

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
            DataBase oDB = null;
            try
            {
                //建立CMFTHelper並將 CMFT_API的send 委派
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskCMFT mSGFromTaskCMFT = new ProcMsg.MSGFromTaskCMFT(new MSGFrameBase(""));

                if (mSGFromTaskCMFT.UnPack(pMessage) > 0) 
                {
                    string sJsonData = mSGFromTaskCMFT.JsonData;
                    string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskCMFT.JsonData, "JsonObjectName"); 
                    string sStationID = ASI.Lib.Text.Parsing.Json.GetValue(mSGFromTaskCMFT.JsonData, "StationID");
                    string sSeatID = ASI.Lib.Text.Parsing.Json.GetValue(sJsonData, "SeatID");
                    int iMsgID = mSGFromTaskCMFT.MessageID;
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"收到來自TaskCMFT的訊息，SeatID:{sSeatID}；MsgID:{iMsgID}；JsonObjectName:{sJsonObjectName}");
                    var Helper = new DCUHelper();
                    var MSG = new object();
                    //回應Ack給CMFT 
                    switch (sJsonObjectName)
                    {
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPreRecordMsg: //預錄訊息 
                            MSG = Helper.SendPreRecordMSGToDCU(mSGFromTaskCMFT);
                            mDMD_API.Send((Message.Message)MSG);
                            //DetermineSendDestination((Message.Message)MSG);
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendInstantMsg:  //即時訊息
                            MSG = Helper.SendInstantMSGToDCU(mSGFromTaskCMFT);
                            mDMD_API.Send((Message.Message)MSG);
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPreRecordMessageSetting: //預錄訊息設定
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendGroupSetting:      //群組設定
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendScheduleSetting:   //排成設定
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendTrainMessageSetting: // 列車訊息設定
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendPowerTimeSetting:  //電力設定
                            break;
                        case ASI.Wanda.DMD.TaskDCU.Constants.SendParameterSetting:  //群組設定
                            break;
                        default:
                            break;
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

        //判別要傳送的目的碼  
        private void DetermineSendDestination(ASI.Wanda.DMD.Message.Message message)
        {
            try
            {
                if (message.JsonContent == null)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, "JsonContent is null. Unable to determine send destination.");
                    return;
                }
                // 解析 JsonContent   
                var jsonObject = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)ASI.Lib.Text.Parsing.Json.DeserializeObject(message.JsonContent, typeof(ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage));
                // 提取 target_du  
                var targetDuList = jsonObject.target_du; 
                //讀取現有連線的clinet
                var Client = mDMD_API.ClientIDList;

                foreach (var clientEntry in Client)
                {
                    string clientIPPort = clientEntry.Value;
                    //  clientIPPort 的格式為 "IP:Port"  
                    string[] clientParts = clientIPPort.Split(':');

                    if (clientParts.Length == 2)
                    {
                        string clientIP = clientParts[0];

                        // 在這裡進行相應的處理，例如取得 stationID 
                        foreach (var targetDu in targetDuList)
                        {
                            // 在這裡進行字串操作，提取 stationID    
                            string[] parts = targetDu.Split('_');
                            if (parts.Length >= 1)
                            {
                                // parts[0] 就是 stationID   
                                string stationID = parts[0];

                                mDMD_API.Send(message, clientIPPort);
                                ASI.Lib.Log.DebugLog.Log("DetermineSendDestination", $"Connected client to the target IP: {clientIP}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("DetermineSendDestination", ex);
                ASI.Lib.Log.DebugLog.Log("DetermineSendDestination", $"Failed to determine send destination. Message: {message.JsonContent}");
            }

        }

        /// <summary>
        /// 與CMFT Server連線
        /// </summary>
        private void ConnToDMDServer()
        {
            try
            {
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= DMD_API_ReceivedEvent;
                    mDMD_API.DisconnectedEvent -= DMD_API_DisconnectedEvent;
                    mDMD_API.Dispose();
                }

                mDMD_API = new ASI.Wanda.DMD.DMD_API();
                mDMD_API.ReceivedEvent += DMD_API_ReceivedEvent;
                mDMD_API.DisconnectedEvent += DMD_API_DisconnectedEvent;
                mDMDServerConnStr = ConfigApp.Instance.GetConfigSetting("DCU_Server");
                int iResult = mDMD_API.Initial(mDMDServerConnStr);
                if (iResult == 0)
                {
                    mIsConnectedToDCU = true;
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"與CMFT Server連線成功");
                    
                    //連線成功時，重新計算最後一次收到DMD的時間
                    LastHeartbeatTime = System.DateTime.Now;
                }
                else 
                {
                    mIsConnectedToDCU = false;
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"與CMFT Server連線失敗，DMD_Server:{mDMDServerConnStr}");
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
            }
        }
        /// <summary>
        /// 結束處理DMD模組執行程序
        /// </summary>
        public override void StopTask()
        {
            if (mDMD_API != null)
            {
                mDMD_API.Dispose();
                mDMD_API = null;
            }

            base.StopTask();
        }
    }
}
