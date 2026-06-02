using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using DMDService.Services.Interfaces;
using DMDService.Services.Models;

namespace DMDService.Services.Services
{
    public class CmftMessageService : ICmftMessageService
    {
        private readonly ICmftConnectionService _connectionService;
        private readonly Random _random = new Random();
        private Timer _heartbeatTimer;
        private Timer _blackListTimer;
        private List<string> _failedDeviceList = new List<string>();

        public event Action<string> LogConnectionState;
        public event Action<string> LogSendData;
        public event Action<string> LogResponseData;
        public event Action<string> LogReceiveData;
        public event Action<string> LogBlackList;
        public event Action<string> LogBlackListSeparator;

        public bool IsConnected => _connectionService.IsConnected;

        public CmftMessageService(ICmftConnectionService connectionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _connectionService.MessageReceived += OnMessageReceived;
        }

        #region Connection

        public int Connect(string ip, string port, string type)
        {
            return _connectionService.Connect(ip, port, type);
        }

        public void Disconnect()
        {
            _connectionService.Disconnect();
        }

        #endregion

        #region Database

        public bool InitializeDatabases(string dmdIp, string dmdPort, string dmdDb, string dmdUser, string dmdPass,
                                         string cmftIp, string cmftPort, string cmftDb, string cmftUser, string cmftPass)
        {
            try
            {
                bool dmdResult = ASI.Wanda.DMD.DB.Manager.Initializer(dmdIp, dmdPort, dmdDb, dmdUser, dmdPass, "DMDServer");
                bool cmftResult = ASI.Wanda.CMFT.DB.Manager.Initializer(cmftIp, cmftPort, cmftDb, cmftUser, cmftPass, "admin");
                return dmdResult && cmftResult;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("CmftMessageService", ex);
                return false;
            }
        }

        public void InitializeCmftDatabase(string ip, string port, string db, string user, string pass)
        {
            ASI.Wanda.CMFT.DB.Manager.Initializer(ip, port, db, user, pass, "admin");
        }

        public List<EquipStatusInfo> GetEquipStatuses()
        {
            try
            {
                var statuses = ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.SelectAll();
                return statuses?.Select(s => new EquipStatusInfo
                {
                    EquipId = s.equip_id,
                    EquipStatus = s.equip_status
                }).ToList() ?? new List<EquipStatusInfo>();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("CmftMessageService", ex);
                return new List<EquipStatusInfo>();
            }
        }

        public List<TargetPanelInfo> GetTargetPanels()
        {
            try
            {
                var targets = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectAll();
                return targets?.Select(t => new TargetPanelInfo
                {
                    StationId = t.station_id,
                    AreaId = t.area_id,
                    DeviceId = t.device_id,
                    DeviceStatus = t.device_status
                }).ToList() ?? new List<TargetPanelInfo>();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("CmftMessageService", ex);
                return new List<TargetPanelInfo>();
            }
        }

        #endregion

        #region Timers

        public void StartTimers()
        {
            // Heartbeat timer (7 sec)
            _heartbeatTimer = new Timer(7000);
            _heartbeatTimer.Elapsed += OnHeartbeatTimerElapsed;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Enabled = true;

            // BlackList timer (6 sec)
            _blackListTimer = new Timer(6000);
            _blackListTimer.Elapsed += OnBlackListTimerElapsed;
            _blackListTimer.AutoReset = true;
            _blackListTimer.Start();
        }

        public void StopTimers()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Dispose();
                _heartbeatTimer = null;
            }
            if (_blackListTimer != null)
            {
                _blackListTimer.Stop();
                _blackListTimer.Dispose();
                _blackListTimer = null;
            }
        }

        private void OnHeartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var msg = new ASI.Wanda.CMFT.Message.Message(
                    ASI.Wanda.CMFT.Message.Message.eMessageType.Heartbeat, 0, "連線中");
                _connectionService.Send(msg);
                string time = DateTime.Now.ToString("HH:mm:ss.fff");
                LogConnectionState?.Invoke($"發送訊息與CMFT連線 HeartBeat{time}");
                ASI.Lib.Log.DebugLog.Log("conetState", msg.ToString());
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("CmftMessageService.Heartbeat", ex);
            }
        }

        private void OnBlackListTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateBlackListFromDatabase();
        }

        private void UpdateBlackListFromDatabase()
        {
            _failedDeviceList.Clear();
            ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.SelectBlackList(false, "DMD", _failedDeviceList);

            for (int i = 0; i < _failedDeviceList.Count; i++)
            {
                LogBlackList?.Invoke(_failedDeviceList[i]);
            }
            LogBlackListSeparator?.Invoke("================================");
        }

        #endregion

        #region Message Received

        private void OnMessageReceived(ASI.Wanda.CMFT.Message.Message cmftMessage)
        {
            try
            {
                string sRcvTime = DateTime.Now.ToString("HH:mm:ss.fff");
                string sByteArray = ASI.Lib.Text.Parsing.String.BytesToHexString(cmftMessage.CompleteContent, "");

                if (cmftMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Heartbeat)
                {
                    var temp = FormatJsonObject(cmftMessage);
                    LogReceiveData?.Invoke($"{temp} \r\n");
                }
                else if (cmftMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Ack)
                {
                    string sLog = string.Format("Ack，訊息識別碼:[{0}]", cmftMessage.MessageID);
                    LogReceiveData?.Invoke($"{sRcvTime} {sLog} \r\n");
                }
                else if (cmftMessage.MessageType == ASI.Wanda.CMFT.Message.Message.eMessageType.Command)
                {
                    HandleCommandMessage(cmftMessage, sByteArray);
                }
                else
                {
                    ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"無此種訊息類別:[{cmftMessage.MessageType}]");
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("TaskDMD", ex);
            }
        }

        private void HandleCommandMessage(ASI.Wanda.CMFT.Message.Message cmftMessage, string byteArray)
        {
            string jsonData = cmftMessage.JsonContent;
            string jsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(jsonData, "JsonObjectName");
            int msgId = cmftMessage.MessageID;

            string sLog = $"從CMFT Server收到:{byteArray}；訊息類別碼:{cmftMessage.MessageType}；識別碼:{msgId}；長度:{cmftMessage.MessageLength}；內容:{jsonData}；JsonObjectName:{jsonObjectName}";
            ASI.Lib.Log.DebugLog.Log("FromCMFTDate", $"{sLog}\r\n");

            var oObject = ASI.Wanda.CMFT.Message.Helper.GetJsonObject(cmftMessage.JsonContent);

            // (2) 預錄訊息命令
            if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage")
            {
                HandlePreRecordMessage(cmftMessage);
            }
            // (3) 即時訊息命令
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage")
            {
                HandleInstantMessage(cmftMessage);
            }
            // (4)~(8) 其他命令 — 記錄日誌
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"排程ID={obj.sched_id}；命令種類={obj.command}；DB1={obj.dbName1}；DB2={obj.dbName2}");
            }
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"預錄訊息ID={obj.msg_id}；命令種類={obj.command}；DB1={obj.dbName1}");
            }
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"列車訊息ID={obj.msg_id}；命令種類={obj.command}；DB1={obj.dbName1}");
            }
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"命令種類={obj.command}；DB1={obj.dbName1}");
            }
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"群組ID={obj.group_id}；命令種類={obj.command}；DB1={obj.dbName1}");
            }
            else if (jsonObjectName == "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting")
            {
                var obj = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting)oObject;
                ASI.Lib.Log.DebugLog.Log("CmftMessageService", $"命令種類={obj.command}；DB1={obj.dbName1}");
            }

            var formatted = FormatJsonObject(cmftMessage);
            LogReceiveData?.Invoke($"{formatted} \r\n");
        }

        private void HandlePreRecordMessage(ASI.Wanda.CMFT.Message.Message cmftMessage)
        {
            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage)
                ASI.Wanda.CMFT.Message.Helper.GetJsonObject(cmftMessage.JsonContent);

            var failedList = new List<string>();
            var stationsDuDictionary = GroupTargetsByStation(oJsonObject.target_du);
            var deviceInfoList = ParseDeviceInfoList(oJsonObject.target_du);

            var panelError = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectPanelStatusError();

            foreach (var deviceInfo in deviceInfoList)
            {
                string deviceInfoString = $"{deviceInfo.StationId}_{deviceInfo.AreaId}_{deviceInfo.DeviceId}";
                bool isDuplicate = panelError.Any(t =>
                    t.station_id == deviceInfo.StationId &&
                    t.area_id == deviceInfo.AreaId &&
                    t.device_id == deviceInfo.DeviceId);

                if (isDuplicate)
                    failedList.Add(deviceInfoString);
            }

            foreach (var station in stationsDuDictionary)
            {
                if (panelError.Any(t => t.station_id == station.Key))
                {
                    SendResponsePreRecordMessage(cmftMessage, oJsonObject.SeatID, oJsonObject.msg_id,
                        station.Key, false,
                        _failedDeviceList.Contains(station.Key) ? station.Value : failedList);
                }
                else
                {
                    SendResponsePreRecordMessage(cmftMessage, oJsonObject.SeatID, oJsonObject.msg_id,
                        station.Key, true,
                        _failedDeviceList.Contains(station.Key) ? station.Value : new List<string>());
                }
            }
        }

        private void HandleInstantMessage(ASI.Wanda.CMFT.Message.Message cmftMessage)
        {
            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage)
                ASI.Wanda.CMFT.Message.Helper.GetJsonObject(cmftMessage.JsonContent);

            var failedList = new List<string>();
            var stationsDuDictionary = GroupTargetsByStation(oJsonObject.target_du);
            var deviceInfoList = ParseDeviceInfoList(oJsonObject.target_du);

            var panelError = ASI.Wanda.DMD.DB.Tables.DMD.dmdTarget.SelectPanelStatusError();

            foreach (var deviceInfo in deviceInfoList)
            {
                string deviceInfoString = $"{deviceInfo.StationId}_{deviceInfo.AreaId}_{deviceInfo.DeviceId}";
                bool isDuplicate = panelError.Any(t =>
                    t.station_id == deviceInfo.StationId &&
                    t.area_id == deviceInfo.AreaId &&
                    t.device_id == deviceInfo.DeviceId);

                if (isDuplicate)
                    failedList.Add(deviceInfoString);
            }

            foreach (var station in stationsDuDictionary)
            {
                if (panelError.Any(t => t.station_id == station.Key))
                {
                    SendResponseInstantMessage(cmftMessage, oJsonObject.SeatID, oJsonObject.msg_id,
                        station.Key, false,
                        _failedDeviceList.Contains(station.Key) ? station.Value : failedList);
                }
                else
                {
                    SendResponseInstantMessage(cmftMessage, oJsonObject.SeatID, oJsonObject.msg_id,
                        station.Key, true,
                        _failedDeviceList.Contains(station.Key) ? station.Value : new List<string>());
                }
            }
        }

        #endregion

        #region Response Methods

        private void SendResponsePreRecordMessage(ASI.Wanda.CMFT.Message.Message cmftMessage,
            string seatId, List<string> msgId, string stationId, bool isSuccess, List<string> failedTargets)
        {
            var oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = cmftMessage.MessageID;
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Response;

            var res = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendPreRecordMessage(
                ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
            res.SeatID = seatId;
            res.msg_id = msgId;
            res.station_id = stationId;
            res.is_success = isSuccess;
            res.failed_target = isSuccess ? null : failedTargets;

            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(res);
            _connectionService.Send(oMessage);

            var formatted = FormatJsonObject(oMessage);
            LogResponseData?.Invoke(formatted);
            ASI.Lib.Log.DebugLog.Log("ResponPreRecord", oMessage.JsonContent);
        }

        private void SendResponseInstantMessage(ASI.Wanda.CMFT.Message.Message cmftMessage,
            string seatId, string msgId, string stationId, bool isSuccess, List<string> failedTargets)
        {
            var oMessage = new ASI.Wanda.CMFT.Message.Message();
            oMessage.MessageID = cmftMessage.MessageID;
            oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Response;

            var res = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendInstantMessage(
                ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
            res.SeatID = seatId;
            res.msg_id = msgId;
            res.station_id = stationId;
            res.is_success = isSuccess;
            res.failed_target = isSuccess ? null : failedTargets;

            oMessage.JsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(res);
            _connectionService.Send(oMessage);

            var formatted = FormatJsonObject(oMessage);
            LogResponseData?.Invoke(formatted);
            ASI.Lib.Log.DebugLog.Log("ResponInstant", oMessage.JsonContent);
        }

        #endregion

        #region Equipment Status

        public void SendEquipStatus(string equipId, bool equipStatus)
        {
            try
            {
                var equipStatusObj = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.EquipStatus(
                    ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
                equipStatusObj.SeatID = "DMD";
                equipStatusObj.equip_id = equipId;
                equipStatusObj.status = equipStatus;
                equipStatusObj.dbName1 = "sys_equip_status";

                string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(equipStatusObj);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    var oMessage = new ASI.Wanda.CMFT.Message.Message();
                    oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;
                    oMessage.MessageID = _random.Next(1, 100000);
                    oMessage.JsonContent = jsonContent;

                    ASI.Wanda.DMD.DB.Tables.System.sysEquipStatus.UpdateEquipStatus(equipId, equipStatus);

                    int sendResult = _connectionService.Send(oMessage);
                    if (sendResult == 0)
                    {
                        var temp = $"{DateTime.Now:HH:mm:ss.fff} {oMessage.JsonContent}\r\n";
                        temp += FormatByteArray(oMessage.Content);
                        LogSendData?.Invoke(temp);
                        ASI.Lib.Log.DebugLog.Log("SendToCMFTEquip", $"傳送封包內容 : {jsonContent}\r\n");
                    }
                    else
                    {
                        LogSendData?.Invoke($"發送失敗，錯誤碼: {sendResult}");
                    }
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("CmftMessageService", ex);
            }
        }

        public void SendOperationMode(string mode, string station)
        {
            var operationMode = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.OperationMode(
                ASI.Wanda.CMFT.Enum.COMDevice.BOCC_DMD_server);
            operationMode.SeatID = "DMD";
            operationMode.station_id = station;
            operationMode.mode = mode;

            string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(operationMode);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                var oMessage = new ASI.Wanda.CMFT.Message.Message();
                oMessage.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Command;
                oMessage.MessageID = _random.Next(1, 100000);

                int sendResult = _connectionService.Send(oMessage);
                if (sendResult == 0)
                {
                    LogSendData?.Invoke($"{DateTime.Now:HH:mm:ss.fff} {oMessage.JsonContent}\r\n");
                }
                else
                {
                    LogSendData?.Invoke($"發送失敗，錯誤碼: {sendResult}");
                }
            }
        }

        #endregion

        #region DB Sync

        public void SyncPlayList()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPlayList.SelectAll();
                var convertedList = tempList.Select(item => new ASI.Wanda.DMD.DB.Models.dmd_playlist
                {
                    playlist_id = item.playlist_id,
                    station_id = item.station_id,
                    area_id = item.area_id,
                    device_id = item.device_id,
                    message_id = item.message_id,
                    message_type = item.message_type,
                    ins_time = item.ins_time,
                    ins_user = item.ins_user,
                    send_time = item.send_time,
                    upd_time = item.upd_time,
                    upd_user = item.upd_user,
                }).ToList();

                convertedList.ForEach(item =>
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.DeletePlayingItem(
                        item.station_id, item.area_id, item.device_id));

                foreach (var item in convertedList)
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.InsertPlayingItem(
                        item.playlist_id, item.station_id, item.area_id,
                        item.device_id, item.message_id, item.message_type, item.send_time);
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("Error updating dmd_playlist", ex);
            }
        }

        public void SyncPreRecordMessages()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPreRecordMessage.SelectAll();
                var convertedList = tempList.Select(item => new ASI.Wanda.DMD.DB.Models.dmd_pre_record_message
                {
                    message_id = item.message_id,
                    message_name = item.message_name,
                    message_type = item.message_type,
                    message_priority = item.message_priority,
                    move_mode = item.move_mode,
                    move_speed = item.move_speed,
                    Interval = item.Interval,
                    message_content = item.message_content,
                    font_type = item.font_type,
                    font_size = item.font_size,
                    font_color = item.font_color,
                    message_content_en = item.message_content_en,
                    font_type_en = item.font_type_en,
                    font_size_en = item.font_size_en,
                    font_color_en = item.font_color_en,
                    ins_user = item.ins_user,
                    ins_time = item.ins_time,
                    upd_user = item.upd_user,
                    upd_time = item.upd_time,
                }).ToList();

                convertedList.ForEach(item =>
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.DeletePreRecordMessage(item.message_id));

                foreach (var item in convertedList)
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.InsertPreRecordMessage(
                        item.message_id, item.message_name, item.message_type,
                        item.message_priority, item.move_mode, item.move_speed,
                        item.Interval, item.message_content, item.font_type,
                        item.font_size, item.font_color, item.message_content_en,
                        item.font_type_en, item.font_size_en, item.font_color_en);
                }
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdPreRecordMessage", ex);
            }
        }

        #endregion

        #region Helper Methods

        private Dictionary<string, List<string>> GroupTargetsByStation(List<string> targetDuList)
        {
            var dict = new Dictionary<string, List<string>>();
            foreach (var targetDu in targetDuList)
            {
                var station = targetDu.Split('_')[0];
                if (!dict.ContainsKey(station))
                    dict[station] = new List<string>();
                dict[station].Add(targetDu);
            }
            return dict;
        }

        private List<TargetDevice> ParseDeviceInfoList(List<string> targetDuList)
        {
            return targetDuList
                .Where(t => t.Split('_').Length >= 3)
                .Select(t =>
                {
                    var parts = t.Split('_');
                    return new TargetDevice
                    {
                        StationId = parts[0],
                        AreaId = parts[1],
                        DeviceId = parts[2]
                    };
                })
                .ToList();
        }

        private string FormatJsonObject(ASI.Wanda.CMFT.Message.Message cmftMessage)
        {
            string jsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(cmftMessage.JsonContent, "JsonObjectName");
            var jsonObject = ASI.Wanda.CMFT.Message.Helper.GetJsonObject(cmftMessage.JsonContent);

            var sb = new StringBuilder();
            Type t = jsonObject.GetType();
            sb.AppendFormat("MessageID:{0}", cmftMessage.MessageID).AppendLine();
            sb.AppendLine("==========================================================");
            sb.AppendFormat("時間: {0}", DateTime.Now).AppendLine();
            sb.AppendFormat("類別: {0}", t.FullName).AppendLine();
            sb.AppendLine("==========================================================");

            PropertyInfo[] props = t.GetProperties();
            sb.AppendFormat("屬性({0}個):", props.Length).AppendLine();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var typeName = prop.PropertyType.Name;
                var value = prop.GetValue(jsonObject);

                sb.AppendLine("-------------------------------------------------------------------------------------------");

                if (value is IList listValues)
                {
                    sb.AppendFormat("　　屬性：{0} ({1}) 數量{2}:", name, typeName, listValues.Count).AppendLine();
                    foreach (var listValue in listValues)
                    {
                        sb.AppendFormat("　　　值({0})：{1}", listValue.GetType().Name, listValue).AppendLine();
                    }
                }
                else
                {
                    sb.AppendFormat("　　屬性：{0}:", name).AppendLine();
                    sb.AppendFormat("　　　值({0})：{1}", typeName, value).AppendLine();
                }
            }

            sb.AppendLine("-------------------------------------------------------------------------------------------");
            return sb.ToString();
        }

        private string FormatByteArray(byte[] data)
        {
            var sb = new StringBuilder();
            sb.Append("封包內容：");
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        #endregion
    }
}
