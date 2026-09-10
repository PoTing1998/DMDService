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
        private string _currentStationID;

        public DCUHelper()
        {
        }

        public DCUHelper(string currentStationID)
        {
            _currentStationID = currentStationID;
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
        /// 篩選屬於當前車站的目標設備
        /// </summary>
        /// <param name="targetDuList">目標設備列表 (格式: StationID_AreaID_DeviceID)</param>
        /// <returns>篩選後屬於當前車站的設備列表</returns>
        private List<string> FilterTargetDuForCurrentStation(List<string> targetDuList)
        {
            if (string.IsNullOrEmpty(_currentStationID))
            {
                // 如果沒有設定當前車站 ID，返回所有目標
                ASI.Lib.Log.DebugLog.Log("DCUHelper", "未設定當前車站 ID，不進行篩選");
                return targetDuList;
            }

            if (targetDuList == null || targetDuList.Count == 0)
            {
                ASI.Lib.Log.DebugLog.Log("DCUHelper", "target_du 列表為空");
                return targetDuList;
            }

            // 篩選出屬於當前車站的設備
            var filteredList = targetDuList
                .Where(target => target.StartsWith(_currentStationID + "_"))
                .ToList();

            ASI.Lib.Log.DebugLog.Log("DCUHelper",
                $"原始 target_du 數量: {targetDuList.Count}，篩選後屬於車站 [{_currentStationID}] 的數量: {filteredList.Count}");

            if (filteredList.Count > 0)
            {
                ASI.Lib.Log.DebugLog.Log("DCUHelper",
                    $"篩選結果: {string.Join(", ", filteredList)}");
            }

            return filteredList;
        }

        /// <summary>
        /// 依 target_du 的車站前綴 (StationID_AreaID_DeviceID) 將目標設備分群。
        /// 一則訊息的 target_du 可能同時包含多站，需分別送往各站的 DCU。
        /// </summary>
        /// <param name="targetDuList">目標設備列表</param>
        /// <returns>車站代碼 -&gt; 該站的目標設備列表</returns>
        private Dictionary<string, List<string>> GroupTargetDuByStation(List<string> targetDuList)
        {
            var groups = new Dictionary<string, List<string>>();

            if (targetDuList == null || targetDuList.Count == 0)
            {
                ASI.Lib.Log.DebugLog.Log("DCUHelper", "target_du 列表為空，無法分群");
                return groups;
            }

            foreach (var targetDu in targetDuList)
            {
                if (string.IsNullOrWhiteSpace(targetDu))
                    continue;

                var parts = targetDu.Split('_');
                if (parts.Length < 3)
                {
                    ASI.Lib.Log.DebugLog.Log("DCUHelper", $"target_du 格式不正確，已略過: [{targetDu}]");
                    continue;
                }

                var stationID = parts[0];
                if (!groups.ContainsKey(stationID))
                    groups.Add(stationID, new List<string>());

                groups[stationID].Add(targetDu);
            }

            ASI.Lib.Log.DebugLog.Log("DCUHelper",
                $"target_du 共 {targetDuList.Count} 筆，分群後涵蓋 {groups.Count} 個車站: {string.Join(", ", groups.Keys)}");

            return groups;
        }

        #region 傳給DCU 的Method

        /// <summary>
        /// 將預錄訊息依車站分群後，產生要送往各站 DCU 的訊息。
        /// 與 <see cref="SendPreRecordMSGToDCU"/> 的差異：不以 STATION_ID 篩選，
        /// 而是把每一站的 target_du 各自包成一則訊息，由呼叫端依 dcu_ip 分送。
        /// </summary>
        /// <param name="DMDServerMessage">來自 TaskCMFT 的訊息物件</param>
        /// <returns>車站代碼 -&gt; 該站的訊息物件</returns>
        public Dictionary<string, ASI.Wanda.DMD.Message.Message> SendPreRecordMSGToDCUByStation(MSGFromTaskCMFT DMDServerMessage)
        {
            var result = new Dictionary<string, ASI.Wanda.DMD.Message.Message>();

            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(DMDServerMessage.JsonData);

            foreach (var group in GroupTargetDuByStation(oJOFromCMFT.target_du))
            {
                // station 欄位為「訊息發送來源」，維持原本的 OCC 不變
                var sendPreRecordMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station.OCC);
                sendPreRecordMessage.seatID = oJOFromCMFT.seatID;
                sendPreRecordMessage.msg_id = oJOFromCMFT.msg_id;
                sendPreRecordMessage.target_du = group.Value;

                var message = new ASI.Wanda.DMD.Message.Message(
                                  ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                                  DMDServerMessage.MessageID,
                                  ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));

                ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCUByStation", $"[{group.Key}] {message.JsonContent}");
                result.Add(group.Key, message);
            }

            return result;
        }

        /// <summary>
        /// 將即時訊息依車站分群後，產生要送往各站 DCU 的訊息。
        /// </summary>
        /// <param name="DMDServerMessage">來自 TaskCMFT 的訊息物件</param>
        /// <returns>車站代碼 -&gt; 該站的訊息物件</returns>
        public Dictionary<string, ASI.Wanda.DMD.Message.Message> SendInstantMSGToDCUByStation(MSGFromTaskCMFT DMDServerMessage)
        {
            var result = new Dictionary<string, ASI.Wanda.DMD.Message.Message>();

            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(DMDServerMessage.JsonData);

            foreach (var group in GroupTargetDuByStation(oJOFromCMFT.target_du))
            {
                // station 欄位為「訊息發送來源」，維持原本的 OCC 不變
                var sendInstantMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(ASI.Wanda.DMD.Enum.Station.OCC);
                sendInstantMessage.seatID = oJOFromCMFT.seatID;
                sendInstantMessage.msg_id = oJOFromCMFT.msg_id;
                sendInstantMessage.target_du = group.Value;

                var message = new ASI.Wanda.DMD.Message.Message(
                                  ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                                  DMDServerMessage.MessageID,
                                  ASI.Lib.Text.Parsing.Json.SerializeObject(sendInstantMessage));

                ASI.Lib.Log.DebugLog.Log("SendInstantMSGToDCUByStation", $"[{group.Key}] {message.JsonContent}");
                result.Add(group.Key, message);
            }

            return result;
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

            // 將 oJOFromCMFT 中的 seatID, msg_id 等屬性賦值給新預錄訊息物件
            sendPreRecordMessage.seatID = oJOFromCMFT.seatID;
            sendPreRecordMessage.msg_id = oJOFromCMFT.msg_id;

            // 篩選屬於當前車站的 target_du
            sendPreRecordMessage.target_du = FilterTargetDuForCurrentStation(oJOFromCMFT.target_du);

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
            var sJsonData = DMDServerMessage.JsonData;

            // 將 JSON 資料反序列化成 DCU 訊息物件
            var oJOFromCMFT = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新訊息物件
            var SendInstantMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, msg_id 等屬性賦值給新訊息物件
            SendInstantMessage.seatID = oJOFromCMFT.seatID;
            SendInstantMessage.msg_id = oJOFromCMFT.msg_id;

            // 篩選屬於當前車站的 target_du
            SendInstantMessage.target_du = FilterTargetDuForCurrentStation(oJOFromCMFT.target_du);

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
            var sJsonData = DMDServerMessage.JsonData;
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



        /// <summary>
        /// 將預錄訊息傳送給 DCU 伺服器
        /// </summary>
        /// <param name="DMDServerMessage">來自 DMD 伺服器的訊息物件</param>
        /// <returns>傳送給 DCU 伺服器的訊息物件</returns>
        public object SendOCSMSGMSGToDCU(MSGFromTaskOCS DMDServerMessage)
        {
            // 取得 DMDServerMessage 中的 JSON 資料
            string sJsonData = DMDServerMessage.JsonData;

            // 將 JSON 資料反序列化成預錄訊息物件
            var oJOFromOCS = (ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMSG)
                              ASI.Wanda.DMD.Message.Helper.GetJsonObject(sJsonData);

            // 組合要傳送給 DCU 的新預錄訊息物件
            var sendOCSMessage = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.TrainMSG(ASI.Wanda.DMD.Enum.Station.OCC);

            // 將 oJOFromCMFT 中的 seatID, msg_id, target_du 等屬性賦值給新預錄訊息物件 
            sendOCSMessage.Type = oJOFromOCS.Type;
            sendOCSMessage.Command = oJOFromOCS.Command;
            sendOCSMessage.Platform_id = oJOFromOCS.Platform_id;
            sendOCSMessage.Arrive_time1 = oJOFromOCS.Arrive_time1;
            sendOCSMessage.Depart_time1 = oJOFromOCS.Depart_time1;
            sendOCSMessage.Destination1 = oJOFromOCS.Destination1;
            sendOCSMessage.Arrive_time2 = oJOFromOCS.Arrive_time2;
            sendOCSMessage.Depart_time2 = oJOFromOCS.Depart_time2;
            sendOCSMessage.Destination2 = oJOFromOCS.Destination2;
  
            // 建立一個新的訊息物件，指定訊息類型、訊息 ID 及序列化的訊息內容 
            var Message = new ASI.Wanda.DMD.Message.Message(
                              ASI.Wanda.DMD.Message.Message.eMessageType.trainMessage,
                              DMDServerMessage.MessageID,
                              ASI.Lib.Text.Parsing.Json.SerializeObject(sendOCSMessage));

            // 紀錄將傳送的預錄訊息內容到日誌中   
            ASI.Lib.Log.DebugLog.Log("SendOCSMSGToDCU", Message.JsonContent);

            // 傳回已建立的訊息物件
            return Message;

        }
        #endregion

        ///傳送到內部的Server   
        public void SendToTaskCMFT(int msgType, int msgID, string jsonData)
        {
            try 
            {
                ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU MSGFromTaskDCU = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskDCU(new MSGFrameBase("Taskdcu", "Taskcmft"));

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

