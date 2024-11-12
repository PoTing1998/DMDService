using ASI.Lib.Process;
using ASI.Wanda.CMFT;
using System;
using System.Collections.Generic;
using System.Linq;



namespace ASI.Wanda.DMD.TaskCMFT
{
    /// <summary>
    /// 判別傳送過來的JsonName 
    /// </summary>
    public static class Constants
    {
        public const string SendPreRecordMsg                = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage";
        public const string SendInstantMsg                  = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendInstantMessage";
        public const string SendScheduleSetting             = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ScheduleSetting";
        public const string SendPreRecordMessageSetting     = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PreRecordMessageSetting";
        public const string SendTrainMessageSetting         = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.TrainMessageSetting";
        public const string SendPowerTimeSetting            = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.PowerTimeSetting";
        public const string SendGroupSetting                = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.GroupSetting";
        public const string SendParameterSetting            = "ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.ParameterSetting";
    }
    public class CMFTHelper<T> where T : class
    {
        private Action<T, ASI.Wanda.CMFT.Message.Message> sendAction;

        private T API;

        public CMFTHelper(T api, Action<T, ASI.Wanda.CMFT.Message.Message> sendAction)
        {
            API = api;
            this.sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));
        }
        public void HandleAckMessage(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
            var sLog = $"Ack，訊息識別碼:[{CMFTServerMessage.MessageID}]";
            var MSG = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Ack, CMFTServerMessage.MessageID, null);
            ASI.Lib.Log.DebugLog.Log("FromCMFTService", sLog);
             //利用委派的方式傳送 
             sendAction?.Invoke(API, MSG);
        }
        public void SendPreRecordMSGToDCU(ASI.Wanda.CMFT.Message.Message CMFTServerMessage)
        {
            //收到封包
            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);
            //組封包 
            var sendPreRecordMessage = new JsonObject.DCU.FromDMD.SendPreRecordMessage(Enum.Station.OCC);  
            sendPreRecordMessage.seatID = oJsonObject.SeatID; 
            sendPreRecordMessage.msg_id = oJsonObject.msg_id;
            sendPreRecordMessage.target_du = oJsonObject.target_du;
            //組成給DCU的封包
            var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));
            //傳送不同的格式
            SendToTaskDCU(2, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(sendPreRecordMessage));
            
            ASI.Lib.Log.DebugLog.Log("SendPreRecordMSGToDCU", MSG.JsonContent);
        }
      
        ///傳送到內部MSG   
        private void SendToTaskDCU(int msgType, int msgID, string jsonData)
        {
            try 
            {
                var MSGFromTaskCMFT = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskCMFT(new MSGFrameBase("TaskCMFT", "dmdserverTaskDCU"));
                //組相對應的封包
                
                MSGFromTaskCMFT.MessageType = msgType;
                MSGFromTaskCMFT.MessageID = msgID;  
                MSGFromTaskCMFT.JsonData = jsonData;
                ASI.Lib.Process.ProcMsg.SendMessage(MSGFromTaskCMFT);
                ASI.Lib.Log.DebugLog.Log("SendToTaskDCU", jsonData);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("FromTaskCMFT", ex);
            }
        }
        ///收到DCU回傳的資料後 傳給CMFT   
        public void SendResponsePreRecordMSGToCMFT(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string stationId, bool isSuccess, List<string> failedTargets)
        {
            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.SendPreRecordMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);
            var res_SendPreRecordMessage = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendPreRecordMessage(CMFT.Enum.COMDevice.OCC_DMD_server);
            res_SendPreRecordMessage.SeatID = oJsonObject.SeatID;
            res_SendPreRecordMessage.msg_id = oJsonObject.msg_id;
            ///組成 失敗的訊息內容 
            res_SendPreRecordMessage.station_id = stationId;
            res_SendPreRecordMessage.is_success = isSuccess;
            res_SendPreRecordMessage.failed_target = isSuccess ? null : failedTargets;
            ///組成 CMFT要的訊息內容
            var MSG = new ASI.Wanda.CMFT.Message.Message(CMFT.Message.Message.eMessageType.Response, CMFTServerMessage.MessageID, ASI.Lib.Text.Parsing.Json.SerializeObject(res_SendPreRecordMessage));
            sendAction?.Invoke(API, MSG);
            ASI.Lib.Log.DebugLog.Log("ResponPreRecord", MSG.JsonContent); 
        }
        ///收到DCU回傳的資料後 傳給CMFT  
        public void sendResponInstantMSGToCMFT(ASI.Wanda.CMFT.Message.Message CMFTServerMessage, string stationId, bool isSuccess, List<string> failedTargets)
        {
            var oJsonObject = (ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT.Res_SendInstantMessage)ASI.Wanda.CMFT.Message.Helper.GetJsonObject(CMFTServerMessage.JsonContent);
            var res_SendInstantMessage = new ASI.Wanda.CMFT.JsonObject.DMD.FromDMD.Res_SendInstantMessage(ASI.Wanda.CMFT.Enum.COMDevice.OCC_DMD_server);
            res_SendInstantMessage.SeatID = oJsonObject.SeatID;
            res_SendInstantMessage.msg_id = oJsonObject.msg_id; 
            ///失敗的看板內容 
            res_SendInstantMessage.station_id = stationId;
            res_SendInstantMessage.is_success = isSuccess; 
            res_SendInstantMessage.failed_target = isSuccess ? null : failedTargets;
            ///組成 CMFT要的訊息內容 並依造需求 發送出去   
            var MSG = new ASI.Wanda.CMFT.Message.Message(ASI.Wanda.CMFT.Message.Message.eMessageType.Response, CMFTServerMessage.MessageID, Lib.Text.Parsing.Json.SerializeObject(res_SendInstantMessage));
            
            sendAction?.Invoke(API, MSG);
            ASI.Lib.Log.DebugLog.Log("ResponInstant", MSG.JsonContent);
        }

        #region 資料庫的操作
        /// <summary>
        /// 更新dmd_playlist的資料庫 
        /// </summary>  
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList> UpdateDMDPlayList()
        {
            try
            {
                ///抓取CMFT的資料表
                var tempList = CMFT.DB.Tables.DMD.dmdPlayList.SelectAll();
                ///轉換過程 
                var convertedList = tempList
                    .Select(item => new DB.Models.dmd_playlist
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
                    })
                    .ToList();
                ///刪除原本的資料
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.DeletePlayingItem(
                        item.station_id,item.area_id,item.device_id);
                });

                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList.InsertPlayingItem(
                        item.playlist_id,
                        item.station_id,
                        item.area_id,
                        item.device_id,
                        item.message_id, 
                        item.message_type, 
                        item.send_time
                    );
                }
                ///選擇並分類同一車站的數據 
                return convertedList.Cast<DB.Tables.DMD.dmdPlayList>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況
                ASI.Lib.Log.ErrorLog.Log("Error updating dmd_playlist", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdPlayList>();
            }

        }   
        /// <summary>
        /// 更新DMDPreRecordMessage資料表  
        /// </summary>
        /// <returns></returns>    
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage> UpdataDMDPreRecordMessage()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPreRecordMessage.SelectAll();
                ///轉換過程  
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_pre_record_message
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
                    })
                    .ToList(); 
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.DeletePreRecordMessage(
                       item.message_id 
                    );
                }); 
                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage.InsertPreRecordMessage(
                        item.message_id,
                        item.message_name,
                        item.message_type,
                        item.message_priority,
                        item.move_mode,
                        item.move_speed,
                        item.Interval,
                        item.message_content,
                        item.font_type,
                        item.font_size,
                        item.font_color,
                        item.message_content_en,
                        item.font_type_en,
                        item.font_size_en,
                        item.font_color_en
                    );
                }

                return convertedList.Cast<DMD.DB.Tables.DMD.dmdPreRecordMessage>(); 
            }
            catch (Exception updateException) 
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdPreRecordMessage", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdPreRecordMessage>();
            }  
        }
        /// <summary>
        /// 從CMFT更新Config的表 拿到相對色碼顏色 
        /// </summary>
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.System.sysConfig> UpdataConfig()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.System.sysConfig.SelectAll();
                ///轉換過程 
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.System.sys_config
                    {
                        config_name = item.config_name,
                        config_value = item.config_value, 
                        config_description = item.config_description, 
                        remark = item.remark,
                        system_id = item.system_id,
                        ins_user = item.ins_user,  
                        ins_time = item.ins_time,
                        upd_user = item.upd_user,
                        upd_time = item.upd_time,
                    })  
                    .ToList();
                ///遍歷轉換後的列表，進行更新操作 
                foreach (var item in convertedList)
                {   
                    DB.Tables.System.sysConfig.UpdataSystemConfig(  
                       item.config_name, 
                       item.config_value, 
                       item.config_description ,
                       item.system_id,
                       item.remark 
                    );
                }
                
                return convertedList.Cast<DMD.DB.Tables.System.sysConfig>(); 
            }
            catch (Exception updateException) 
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating sysConfig", updateException);
                return Enumerable.Empty<DB.Tables.System.sysConfig>();
            }
        }

        /// <summary>
        /// 更新dmd_schedule資料表  
        /// </summary>
        /// <returns></returns>    
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedule> UpSchedule()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdSchedule.SelectAll();
                ///轉換過程  
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_schedule
                    {
                        schedule_id= item.schedule_id,
                        schedule_name= item.schedule_name,
                        is_enable = item.is_enable,
                        start_date= item.start_date,
                        end_date= item.end_date,
                        ins_user = item.ins_user,
                        ins_time = item.ins_time,
                        upd_user = item.upd_user,
                        upd_time = item.upd_time,
                    })
                    .ToList();
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedule.DeleteSchedule(
                       item.schedule_id
                    );
                });
                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedule.InsertSchedule(
                       item.schedule_id,
                       item.schedule_name,
                       item.is_enable,
                       item.start_date,
                       item.end_date
                    );
                }

                return convertedList.Cast<DMD.DB.Tables.DMD.dmdSchedule>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdSchedule", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedule>();
            }
        }

        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedulePlayList> UpDMDSchedulePlaylist()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdSchedulePlayList.SelectAll();
                ///轉換過程  
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_schedule_playlist
                    {
                        schedule_id = item.schedule_id,
                        message_id = item.message_id,
                        station_id = item.station_id,
                        device_id = item.device_id,
                        sned_time = item.sned_time,
                        ins_user = item.ins_user,
                        ins_time = item.ins_time,
                        upd_user = item.upd_user,
                        upd_time = item.upd_time,
                    })
                    .ToList();
                convertedList.ForEach(item =>
                {
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedulePlayList.DeleteSchedulePlayListItems(
                       item.schedule_id
                    );
                });
                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedulePlayList.InsertSchedulePlayListItem(
                       item.schedule_id,
                       item.message_id,
                       item.station_id,
                       item.device_id
                    );
                }

                return convertedList.Cast<DMD.DB.Tables.DMD.dmdSchedulePlayList>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdSchedulePlayList", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdSchedulePlayList>();
            }
        }
        public IEnumerable<ASI.Wanda.DMD.DB.Tables.DMD.dmdPowerSetting> UpDateDMDPowerSetting()
        {
            try
            {
                var tempList = ASI.Wanda.CMFT.DB.Tables.DMD.dmdPowerSetting.SelectAll();
                ///轉換過程  
                var convertedList = tempList
                    .Select(item => new ASI.Wanda.DMD.DB.Models.dmd_power_setting
                    {
                        station_id = item.station_id,
                        eco_mode = item.eco_mode,
                        eco_time = item.eco_time,
                        not_eco_day = item.not_eco_day,
                        auto_play_time = item.auto_play_time,
                        auto_eco_time= item.auto_eco_time,
                        ins_user = item.ins_user,
                        ins_time = item.ins_time,
                        upd_user = item.upd_user,
                        upd_time = item.upd_time,
                    })
                    .ToList();
               
                ///遍歷轉換後的列表，進行更新操作
                foreach (var item in convertedList)
                {
                    ///MSGtype  0 =預錄  1= 及時 
                    ASI.Wanda.DMD.DB.Tables.DMD.dmdPowerSetting.UpdatePowerSetting(
                       item.station_id,
                       item.eco_mode,
                       item.eco_time,
                       item.not_eco_day,
                       item.auto_play_time,
                       item.auto_eco_time
                    );
                }

                return convertedList.Cast<DMD.DB.Tables.DMD.dmdPowerSetting>();
            }
            catch (Exception updateException)
            {
                ///記錄例外狀況 
                ASI.Lib.Log.ErrorLog.Log("Error updating dmdPowerSetting", updateException);
                return Enumerable.Empty<ASI.Wanda.DMD.DB.Tables.DMD.dmdPowerSetting>();
            }
        }



        #endregion
    }
}
