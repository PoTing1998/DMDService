using ASI.Lib.Process;
using ASI.Wanda.DMD.ProcMsg;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ASI.Wanda.DMD.TaskDCU
{
    public static class Constants
    {
        public const string SendPreRecordMsg            = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage";
        public const string SendInstantMsg              = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage";
        public const string SendScheduleSetting         = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting";
        public const string SendPreRecordMessageSetting = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting";
        public const string SendTrainMessageSetting     = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting";
        public const string SendPowerTimeSetting        = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting";
        public const string SendGroupSetting            = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting";
        public const string SendParameterSetting        = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting";
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
            string sJsonData = DMDServerMessage.JsonData;
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
        {
            string sJsonData = DMDServerMessage.JsonData;
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            var sendPreRecordMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
            sendPreRecordMessage.seatID = oJOFromCMFT.seatID;
            sendPreRecordMessage.msg_id = oJOFromCMFT.msg_id;
            sendPreRecordMessage.target_du = oJOFromCMFT.target_du;
            
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, DMDServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));

            ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCU", MSG.JsonContent);
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

