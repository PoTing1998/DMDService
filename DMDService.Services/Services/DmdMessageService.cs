using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Wanda.DMD.DB.Models;
using DMDService.Services.Interfaces;
using DMDService.Services.Models;

namespace DMDService.Services.Services
{
    public class DmdMessageService : IDmdMessageService
    {
        private readonly IDmdConnectionService _connectionService;
        private readonly Random _random = new Random();

        public event Action<string> LogMessage;

        public bool IsConnected => _connectionService.IsConnected;

        public DmdMessageService(IDmdConnectionService connectionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public int Connect(string ip, string port, string type)
        {
            return _connectionService.Connect(ip, port, type);
        }

        public void Disconnect()
        {
            _connectionService.Disconnect();
        }

        public bool InitializeDatabase(string ip, string port, string dbName, string userId, string password)
        {
            try
            {
                bool result = ASI.Wanda.DMD.DB.Manager.Initializer(ip, port, dbName, userId, password, "DMDServer");

                if (result)
                    RaiseLog($"✓ 資料庫連線成功: {ip}:{port}/{dbName}");
                else
                    RaiseLog("✗ 資料庫連線失敗");

                return result;
            }
            catch (Exception ex)
            {
                RaiseLog($"資料庫初始化錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return false;
            }
        }

        public List<dmd_pre_record_message> GetPreRecordMessages()
        {
            try
            {
                var messages = ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.SelectAll();
                return messages ?? new List<dmd_pre_record_message>();
            }
            catch (Exception ex)
            {
                RaiseLog($"載入預錄訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return new List<dmd_pre_record_message>();
            }
        }

        public List<dmd_instant_message> GetInstantMessages()
        {
            try
            {
                var messages = ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.SelectAll();
                return messages ?? new List<dmd_instant_message>();
            }
            catch (Exception ex)
            {
                RaiseLog($"載入即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return new List<dmd_instant_message>();
            }
        }

        public List<TargetDevice> GetTargetDevices(string stationFilter)
        {
            try
            {
                var playlists = ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.SelectAll();
                if (playlists == null || playlists.Count == 0)
                    return new List<TargetDevice>();

                var seen = new HashSet<string>();
                var result = new List<TargetDevice>();

                foreach (var playlist in playlists)
                {
                    if (!string.IsNullOrEmpty(stationFilter))
                    {
                        if (stationFilter == "全部")
                        {
                            if (playlist.station_id == "OCC") continue;
                        }
                        else
                        {
                            if (playlist.station_id != stationFilter) continue;
                        }
                    }

                    string key = $"{playlist.station_id}_{playlist.area_id}_{playlist.device_id}";
                    if (seen.Add(key))
                    {
                        result.Add(new TargetDevice
                        {
                            StationId = playlist.station_id,
                            AreaId = playlist.area_id,
                            DeviceId = playlist.device_id
                        });
                    }
                }

                return result.OrderBy(d => d.StationId).ThenBy(d => d.AreaId).ThenBy(d => d.DeviceId).ToList();
            }
            catch (Exception ex)
            {
                RaiseLog($"載入目標看板錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return new List<TargetDevice>();
            }
        }

        public dmd_instant_message GetInstantMessage(string messageId)
        {
            try
            {
                var messages = ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.SelectAll();
                return messages?.FirstOrDefault(m => m.message_id.ToString() == messageId);
            }
            catch (Exception ex)
            {
                RaiseLog($"載入即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return null;
            }
        }

        public bool SaveInstantMessage(dmd_instant_message msg)
        {
            try
            {
                ASI.Wanda.DMD.DB.Tables.DMD.dmdInstantMessage.UpdateInstantMessages(
                    msg.message_id,
                    msg.message_type,
                    msg.message_priority,
                    msg.move_mode,
                    msg.move_speed,
                    msg.Interval,
                    msg.message_content,
                    msg.font_type,
                    msg.font_size,
                    msg.font_color,
                    msg.message_content_en,
                    msg.font_type_en,
                    msg.font_size_en,
                    msg.font_color_en
                );

                RaiseLog($"✓ 即時訊息已更新 ID: {msg.message_id}");
                return true;
            }
            catch (Exception ex)
            {
                RaiseLog($"保存即時訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return false;
            }
        }

        public SendResult SendPreRecordMessage(string seatId, string station, string messageId,
            List<string> targetDuList, int priority, int moveSpeed, int moveMode)
        {
            if (!_connectionService.IsConnected)
                return SendResult.Fail("尚未連線");

            try
            {
                var stationEnum = GetStationEnum(station);
                var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendPreRecordMessage(stationEnum);

                jsonObject.seatID = seatId;
                jsonObject.msg_id = new List<string> { messageId };
                jsonObject.target_du = targetDuList;
                jsonObject.message_priority = priority;
                jsonObject.move_speed = moveSpeed;
                jsonObject.move_mode = moveMode;

                return SendMessageInternal(jsonObject, "預錄訊息");
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return SendResult.Fail($"發送錯誤: {ex.Message}");
            }
        }

        public SendResult SendInstantMessage(string seatId, string station, string messageId,
            List<string> targetDuList, int priority, int moveSpeed, int moveMode)
        {
            if (!_connectionService.IsConnected)
                return SendResult.Fail("尚未連線");

            try
            {
                var stationEnum = GetStationEnum(station);
                var jsonObject = new ASI.Wanda.DMD.JsonObject.DCU.FromDMD.SendInstantMessage(stationEnum);

                jsonObject.seatID = seatId;
                jsonObject.msg_id = messageId;
                jsonObject.target_du = targetDuList;
                jsonObject.message_priority = priority;
                jsonObject.move_speed = moveSpeed;
                jsonObject.move_mode = moveMode;

                return SendMessageInternal(jsonObject, "即時訊息");
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("DmdMessageService", ex);
                return SendResult.Fail($"發送錯誤: {ex.Message}");
            }
        }

        private SendResult SendMessageInternal(object jsonObject, string messageTypeName)
        {
            string jsonContent = ASI.Lib.Text.Parsing.Json.SerializeObject(jsonObject);
            int messageId = _random.Next(1, 100000);

            var message = new ASI.Wanda.DMD.Message.Message(
                ASI.Wanda.DMD.Message.Message.eMessageType.Command,
                messageId,
                jsonContent
            );

            int result = _connectionService.Send(message);
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

            if (result == 0)
            {
                RaiseLog($"[{timestamp}] ✓ 發送成功: {messageTypeName}");
                RaiseLog($"訊息ID: {messageId}");
                RaiseLog($"內容: {jsonContent}");
                RaiseLog("---");
                ASI.Lib.Log.DebugLog.Log("DmdMessageService", $"{messageTypeName}: {jsonContent}");
                return SendResult.Ok(messageId, jsonContent);
            }
            else
            {
                RaiseLog($"[{timestamp}] ✗ 發送失敗，錯誤碼: {result}");
                return SendResult.Fail($"發送失敗，錯誤碼: {result}", result);
            }
        }

        private ASI.Wanda.DMD.Enum.Station GetStationEnum(string stationText)
        {
            switch (stationText)
            {
                case "OCC": return ASI.Wanda.DMD.Enum.Station.OCC;
                case "LG01": return ASI.Wanda.DMD.Enum.Station.LG01;
                case "LG02": return ASI.Wanda.DMD.Enum.Station.LG02;
                case "LG03": return ASI.Wanda.DMD.Enum.Station.LG03;
                case "LG04": return ASI.Wanda.DMD.Enum.Station.LG04;
                case "LG05": return ASI.Wanda.DMD.Enum.Station.LG05;
                case "LG06": return ASI.Wanda.DMD.Enum.Station.LG06;
                case "LG07": return ASI.Wanda.DMD.Enum.Station.LG07;
                case "LG08": return ASI.Wanda.DMD.Enum.Station.LG08;
                case "LG08A": return ASI.Wanda.DMD.Enum.Station.LG08A;
                default: return ASI.Wanda.DMD.Enum.Station.OCC;
            }
        }

        private void RaiseLog(string message)
        {
            LogMessage?.Invoke(message);
        }
    }
}
