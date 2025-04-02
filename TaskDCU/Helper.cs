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
        public object SendPreRecordMSGToDCU(MSGFromTaskCMFT DMDServerMessage)
        {
            //收到DMD SERVER內部的訊息 
            var sJsonData = DMDServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);
            //組成要傳給DCU的message
            var sendPreRecordMessage = new JsonObject.DCU.FromDMD.SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
            sendPreRecordMessage.seatID = oJOFromCMFT.seatID;
            sendPreRecordMessage.msg_id = oJOFromCMFT.msg_id;
            sendPreRecordMessage.target_du = oJOFromCMFT.target_du;
            var Message = new Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, DMDServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCU", Message.JsonContent);
            return Message;
        }
        /// <summary>
        /// 將即時訊送給DCUSERVER 
        /// </summary>
        /// <param name="DMDServerMessage"></param>
        public object SendInstantMSGToDCU(MSGFromTaskCMFT DMDServerMessage)
        {  //收到DMD SERVER內部的訊息 
            var sJsonData = DMDServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);
            //組成要傳給DCU的message
            var SendInstantMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(ASI.Wanda.DMD.Enum.Station.OCC);
            SendInstantMessage.seatID = oJOFromCMFT.seatID;
            SendInstantMessage.msg_id = oJOFromCMFT.msg_id;
            SendInstantMessage.target_du = oJOFromCMFT.target_du;  
            
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, DMDServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendInstantMessage));

            ASI.Lib.Log.DebugLog.Log("SendInstantMessageToDCU", MSG.JsonContent);
            return MSG;
        }
        /// <summary>
        /// 將節能模式送給DCUSERVER 
        /// </summary>
        /// <param name="CMFTServerMessage"></param>
        /// <returns></returns>
        public object SendPowerSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PowerTimeSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);
            //組封包 
            var sendPowerTimeSetting = new JsonObject.DCU.FromDMD.PowerTimeSetting(Enum.Station.OCC);
            sendPowerTimeSetting.seatID = oJOFromCMFT.seatID;
            sendPowerTimeSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            sendPowerTimeSetting.dbName1 = oJOFromCMFT.dbName1;
            //組成給DCU的封包
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(sendPowerTimeSetting));
            ASI.Lib.Log.DebugLog.Log("SendPowerTimeSettingToDCU", MSG.JsonContent);
            return MSG;
        }


        public object SendScheduleSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendScheduleSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);
            //組封包 
            var SendScheduleSetting = new JsonObject.DCU.FromDMD.SendScheduleSetting(Enum.Station.OCC);
            SendScheduleSetting.seatID = oJOFromCMFT.seatID;
            SendScheduleSetting.sched_id = oJOFromCMFT.sched_id;
            SendScheduleSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendScheduleSetting.dbName1 = oJOFromCMFT.dbName1;
            SendScheduleSetting.dbName2 = oJOFromCMFT.dbName2;
            //組成給DCU的封包
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendScheduleSetting));
            ASI.Lib.Log.DebugLog.Log("SendPowerTimeSettingToDCU", MSG.JsonContent);
            return MSG;
        }

        public object SendGroupSettingToDCU(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.GroupSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            //組封包
            var SendGroupSetting = new JsonObject.DCU.FromDMD.GroupSetting(Enum.Station.OCC);
             SendGroupSetting.seatID= oJOFromCMFT.seatID;
            SendGroupSetting.group_id = oJOFromCMFT.group_id;
            SendGroupSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendGroupSetting.dbName1 = oJOFromCMFT.dbName1;
            SendGroupSetting.dbName2 = oJOFromCMFT.dbName2;

            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendGroupSetting));
            ASI.Lib.Log.DebugLog.Log("SendGroupSettingToDCU", MSG.JsonContent);
            return MSG; 
        }


        public object SendPreRecordMessageSetting(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.PreRecordMessageSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            //組封包
            var SendPreRecordMessageSetting = new JsonObject.DCU.FromDMD.PreRecordMessageSetting(Enum.Station.OCC);
            SendPreRecordMessageSetting.seatID = oJOFromCMFT.seatID;
            SendPreRecordMessageSetting.msg_id = oJOFromCMFT.msg_id;
            SendPreRecordMessageSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendPreRecordMessageSetting.dbName1 = oJOFromCMFT.dbName1;

            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendPreRecordMessageSetting));
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMessageSetting", MSG.JsonContent);
            return MSG;
        }
        public object SendTrainMessageSetting(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMessageSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            //組封包
            var SendTrainMessageSetting = new JsonObject.DCU.FromDMD.PreRecordMessageSetting(Enum.Station.OCC);
            SendTrainMessageSetting.seatID = oJOFromCMFT.seatID;
            SendTrainMessageSetting.msg_id = oJOFromCMFT.msg_id;
            SendTrainMessageSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendTrainMessageSetting.dbName1 = oJOFromCMFT.dbName1;

            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendTrainMessageSetting));
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMessageSetting", MSG.JsonContent);
            return MSG;
        }
        public object SendParameterSetting(MSGFromTaskCMFT CMFTServerMessage)
        {
            //收到封包
            var sJsonData = CMFTServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.ParameterSetting)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            //組封包
            var SendParameterSetting = new JsonObject.DCU.FromDMD.PreRecordMessageSetting(Enum.Station.OCC);
            SendParameterSetting.seatID = oJOFromCMFT.seatID;
            SendParameterSetting.SqlCommand = oJOFromCMFT.SqlCommand;
            SendParameterSetting.dbName1 = oJOFromCMFT.dbName1;

            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(SendParameterSetting));
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMessageSetting", MSG.JsonContent);
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

