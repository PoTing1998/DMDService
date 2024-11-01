using ASI.Lib.Process;
using ASI.Wanda.DMD.ProcMsg;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ASI.Wanda.DMD.TaskDCU
{
    public static class Constants
    {
        public const string SendPreRecordMsg            = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage";
        public const string SendInstantMsg              = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage";
        public const string SendScheduleSetting         = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.ScheduleSetting";
        public const string SendPreRecordMessageSetting = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PreRecordMessageSetting";
        public const string SendTrainMessageSetting     = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMessageSetting";
        public const string SendPowerTimeSetting        = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting";
        public const string SendGroupSetting            = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.GroupSetting";
        public const string SendParameterSetting        = "ASI.Wanda.DMD.JsonObject.DCU.FromDMD.ParameterSetting";
    }

   
    public class DCUHelper
    {
        public DCUHelper()
        {
        } 
        public ASI.Wanda.DMD.Message.Message Message { get; set; }
        public List <string> Stations { get; set; }
        public object HandleAckMessage(ASI.Wanda.DMD.Message.Message DMDServerMessage)
        {
            var sLog = $"Ack，訊息識別碼:[{DMDServerMessage.MessageID}]";
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Ack, DMDServerMessage.MessageID, null);
            ASI.Lib.Log.DebugLog.Log("FromDMDService", sLog);
            return MSG;
        }

        /// <summary>
        /// 將預錄訊息傳送給 DCU 伺服器
        /// </summary>
        /// <param name="DMDServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendPreRecordMSGToDCU(MSGFromTaskCMFT DMDServerMessage)
        {
            // 取得 DMDServerMessage 中的 JSON 資料
            string sJsonData = DMDServerMessage.JsonData;

            // 將 JSON 資料反序列化成預錄訊息物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新預錄訊息物件
            var sendPreRecordMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, msg_id, target_du 等屬性賦值給新預錄訊息物件
            sendPreRecordMessage.seatID = oJOFromCMFT.seatID;
            sendPreRecordMessage.msg_id = oJOFromCMFT.msg_id;
            sendPreRecordMessage.target_du = oJOFromCMFT.target_du;  

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容 
            var Message = new ASI.Wanda.DMD.Message.Message(
                              ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                              DMDServerMessage.MessageID,
                              ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));

            // 紀錄將傳送的預錄訊息內容到日誌中   
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCU", Message.JsonContent);

            // 傳回已建立的訊息物件
            return Message;
        }
        
        /// <summary>
        /// 將即時訊息傳送給 DCU 伺服器
        /// </summary>
        /// <param name="DMDServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendInstantMSGToDCU(MSGFromTaskCMFT DMDServerMessage)
        {
            // 取得 DMDServerMessage 中的 JSON 資料
            string sJsonData = DMDServerMessage.JsonData;

            // 將 JSON 資料反序列化成 DCU 訊息物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新訊息物件
            var SendInstantMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, msg_id, target_du 等屬性賦值給新訊息物件
            SendInstantMessage.seatID = oJOFromCMFT.seatID;
            SendInstantMessage.msg_id = oJOFromCMFT.msg_id;
            SendInstantMessage.target_du = oJOFromCMFT.target_du;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          DMDServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendInstantMessage));

            // 紀錄將傳送的訊息內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendInstantMessageToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件
            return MSG;
        }
        /// <summary>
        /// 將排程設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="CMFTServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendScheduleSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            // 取得 CMFTServerMessage 中的 JSON 資料
            string sJsonData = CMFTServerMessage.JsonData;

            // 將 JSON 資料反序列化成排程設定物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendScheduleSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新排程設定物件
            var SendScheduleSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendScheduleSetting(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, sched_id, SqlCommand, dbName1, dbName2 等屬性賦值給新的排程設定物件
            SendScheduleSetting.seatID = oJOFromCMFT.seatID;
            SendScheduleSetting.sched_id = oJOFromCMFT.sched_id;
            SendScheduleSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendScheduleSetting.dbName1 = oJOFromCMFT.dbName1;
            SendScheduleSetting.dbName2 = oJOFromCMFT.dbName2;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          CMFTServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendScheduleSetting));

            // 紀錄將傳送的排程設定內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendPowerTimeSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件
            return MSG;
        }
        /// <summary>
        /// 將預錄訊息設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="DMDServerMessage"></param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendPreRecordMessageSetting(MSGFromTaskCMFT DMDServerMessage)
        {
            //收到DMD SERVER內部的訊息 
            string sJsonData = DMDServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PreRecordMessageSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新訊息物件
            var SendRecordMessageSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PreRecordMessageSetting(ASI.Wanda.DMD.Enum.Station.OCC);
            SendRecordMessageSetting.seatID = oJOFromCMFT.seatID;
            SendRecordMessageSetting.msg_id= oJOFromCMFT.msg_id;
            SendRecordMessageSetting.SqlCommand = oJOFromCMFT.SqlCommand;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          DMDServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendRecordMessageSetting));

            // 紀錄將傳送的訊息內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendRecordMessageSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件 
            return MSG;
        }
        /// <summary>
        /// 將列車訊息設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="DMDServerMessage"></param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendTrainMessageSetting(MSGFromTaskCMFT DMDServerMessage)
        {
            //收到DMD SERVER內部的訊息 
            string sJsonData = DMDServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMessageSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新訊息物件
            var SendTrainMessageSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMessageSetting(ASI.Wanda.DMD.Enum.Station.OCC);
            SendTrainMessageSetting.seatID = oJOFromCMFT.seatID;
            SendTrainMessageSetting.msg_id = oJOFromCMFT.msg_id;
            SendTrainMessageSetting.SqlCommand = oJOFromCMFT.SqlCommand;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          DMDServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendTrainMessageSetting));

            // 紀錄將傳送的訊息內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendTrainMessageSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件 
            return MSG;
        }
       
        /// <summary>
        /// 將節能模式設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="CMFTServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendPowerSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            // 取得 CMFTServerMessage 中的 JSON 資料
            string sJsonData = CMFTServerMessage.JsonData;

            // 將 JSON 資料反序列化成節能模式設定物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的節能模式設定物件
            var sendPowerTimeSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, SqlCommand, dbName1 等屬性賦值給新的節能模式設定物件
            sendPowerTimeSetting.seatID = oJOFromCMFT.seatID;
            sendPowerTimeSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            sendPowerTimeSetting.dbName1 = oJOFromCMFT.dbName1;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          CMFTServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(sendPowerTimeSetting));

            // 紀錄將傳送的節能模式設定內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendPowerTimeSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件
            return MSG;
        }

      


        /// <summary>
        /// 將群組設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="CMFTServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendGroupSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            // 取得 CMFTServerMessage 中的 JSON 資料
            string sJsonData = CMFTServerMessage.JsonData;

            // 將 JSON 資料反序列化成排程設定物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.GroupSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新排程設定物件
            var SendGroupSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.GroupSetting(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, group_id, SqlCommand, dbName1, dbName2 等屬性賦值給新的排程設定物件
            SendGroupSetting.seatID = oJOFromCMFT.seatID;
            SendGroupSetting.group_id = oJOFromCMFT.group_id;
            SendGroupSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendGroupSetting.dbName1 = oJOFromCMFT.dbName1;
            SendGroupSetting.dbName2 = oJOFromCMFT.dbName2;

            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          CMFTServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendGroupSetting));

            // 紀錄將傳送的排程設定內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendGroupSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件
            return MSG;
        }

        /// <summary>
        /// 將群組設定傳送給 DCU 伺服器
        /// </summary>
        /// <param name="CMFTServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendParameterSetting(MSGFromTaskCMFT CMFTServerMessage)
        {
            // 取得 CMFTServerMessage 中的 JSON 資料
            string sJsonData = CMFTServerMessage.JsonData;

            // 將 JSON 資料反序列化成排程設定物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.ParameterSetting)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新排程設定物件
            var SendParameterSetting = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.ParameterSetting(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, SqlCommand, dbName1, dbName2 等屬性賦值給新的排程設定物件
            SendParameterSetting.seatID = oJOFromCMFT.seatID;
            SendParameterSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendParameterSetting.dbName1 = oJOFromCMFT.dbName1;


            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容
            var MSG = new ASI.Wanda.DMD.Message.Message(
                          ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                          CMFTServerMessage.MessageID,
                          ASI.Lib.Text.Parsing.Json.SerializeObject(SendParameterSetting));

            // 紀錄將傳送的排程設定內容到日誌中
            ASI.Lib.Log.DebugLog.Log("SendParameterSettingToDCU", MSG.JsonContent);

            // 傳回已建立的訊息物件
            return MSG;
        }
        ///傳送到內部的Server   
        public void SendToTaskCMFT(int msgType, int msgID, string jsonData)
        {
            try 
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU MSGFromTaskDCU = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU(new MSGFrameBase("TaskDCU", "TaskCMFT"));

                MSGFromTaskDCU.MessageType = msgType;
                MSGFromTaskDCU.MessageID = msgID;
                MSGFromTaskDCU.JsonData = jsonData;

                ASI.Lib.Process.ProcMsg.SendMessage(MSGFromTaskDCU);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("TaskCMFT", ex);
            }
        }
    }
}

