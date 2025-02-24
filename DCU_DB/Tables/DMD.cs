using ASI.Wanda.DCU.DB.Models;
using ASI.Wanda.DCU.DB.Models.DMD;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DCU.DB.Tables.DMD
{
    public class dmdGroup : ASI.Wanda.DCU.DB.Tables.Table<dmd_group>
    {
        #region Methods
        static public dmd_group SelectGroup(Guid groupID)
        {
            string whereString = string.Format("where group_id = '{0}'", groupID);
            var group = SelectWhere(whereString);
            return group.Count > 0 ? group[0] : null;
        }
        static public void InsertGroup(Guid groupID, string groupName, string groupDescription)
        {
            Insert(groupID, groupName, groupDescription);
        }
        static public void UpdateGroup(Guid groupID, string groupName, string groupDescription)
        {
            Update(groupID, groupName, groupDescription);
        }
        static public void DeleteGroup(Guid groupID)
        {
            Delete(groupID);
        }
        #endregion
    }
    public class dmdGroupTarget : ASI.Wanda.DCU.DB.Tables.Table<dmd_group_target>
    {
        #region Methods
        static public List<dmd_group_target> SelectGroupTargets(Guid groupID)
        {
            var whereString = string.Format("where group_id = '{0}'", groupID);
            var devices = SelectWhere(whereString);
            return devices;
        }
        static public void InsertGroupTarget(Guid groupID, string stationID, string AreaID, string DeviceID)
        {
            Insert(groupID, stationID, AreaID, DeviceID);
        }
        static public void DeleteGroupTarget(Guid groupID, string stationID, string AreaID, string DeviceID)
        {
            Delete(groupID, stationID, AreaID, DeviceID);
        }
        static public void DeleteGroupTargets(Guid groupID)
        {
            string whereString = string.Format("where group_id = '{0}'", groupID);
            DeleteWhere(whereString);
        }


        #endregion
    }
    public class dmdInstantMessage : ASI.Wanda.DCU.DB.Tables.Table<dmd_instant_message>
    {
        #region Methods
        public static dmd_instant_message SelectMessage(Guid messageID)
        {
            return Select(messageID);
        }
        public static void InsertInstantMessages(Guid messageID, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Insert(messageID, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void UpdateInstantMessages(Guid messageID, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Update(messageID, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void DeleteInstantMessages(Guid messageID)
        {
            Delete(messageID);
        }
        #endregion
    }
    public class dmdPlayList : ASI.Wanda.DCU.DB.Tables.Table<dmd_playlist>
    {
        #region Methods
        static public dmd_playlist SelectPlayingItem(string stationID, string deviceID, Guid messageID)
        {
            dynamic playingItem = Select(stationID, deviceID, messageID);
            return playingItem;
        }
        static public void InsertPlayingItem(Guid playlist_id, string stationID, string area_id, string deviceID, Guid messageID, int message_type, string send_time)
        {
            Insert(
                  playlist_id
                 , stationID
                 , area_id
                 , deviceID
                 , messageID
                 , message_type
                 , send_time
                 );
        }
        static public void UpdatePlayingItem()
        {

        }
        static public void DeletePlayingItem(string stationID, string area_id, string deviceID)
        {
            string whereString = string.Format("where station_id = '{0}' AND area_id = '{1}' AND  device_id = '{2}' ", stationID, area_id, deviceID);
            DeleteWhere(whereString);
        }
        #endregion
    }
    public class dmdPowerSetting : ASI.Wanda.DCU.DB.Tables.Table<dmd_power_setting>
    {
        #region Methods
        static public dmd_power_setting SelectPowerSetting(string stationID)
        {
            return Select(stationID);
        }
        static public void UpdatePowerSetting(string stationID, string ecoMode, int ecoTime, string notEcoDay, string autoPlayTime, string autoEcoTime)
        {
            Update(
                  stationID
                , ecoMode
                , ecoTime
                , notEcoDay
                , autoPlayTime
                , autoEcoTime);
        }
        #endregion
    }
    public class dmdPreRecordMessage : ASI.Wanda.DCU.DB.Tables.Table<dmd_pre_record_message>
    {
        #region Methods
        public static dmd_pre_record_message SelectMessage(Guid messageID) 
        {
            return Select(messageID);
        }
        public static void InsertPreRecordMessage(Guid messageID, string messageName, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Insert(messageID, messageName, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void UpdatePreRecordMessage(Guid messageID, string messageName, int messageType, int messagePriority, int moveMode, int moveSpeed, string interval, string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN, string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Update(messageID, messageName, messageType, messagePriority, moveMode, moveSpeed, interval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }
        public static void DeletePreRecordMessage(Guid messageID)
        {
            Delete(messageID);
        }
        /// <summary>
        /// 獲取訊息的參數
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public static List<string> SelectMSGSetting(Guid messageID)
        {
            var content = new List<string>();
            var msg = SelectWhere(string.Format("where message_id = '{0}'", messageID)).SingleOrDefault();

            if (msg != null)
            {
                content.Add(msg.message_type.ToString());
                content.Add(msg.message_priority.ToString());
                content.Add(msg.move_mode.ToString());
                content.Add(msg.move_speed.ToString());
                content.Add(msg.Interval);
                content.Add(msg.message_content);
                content.Add(msg.font_type);
                content.Add(msg.font_size.ToString());
                content.Add(msg.font_color);
                content.Add(msg.message_content_en);
                content.Add(msg.font_type_en);
                content.Add(msg.font_size_en.ToString());
                content.Add(msg.font_color_en);
            }

            return content;
        }
        /// <summary>
        /// 取得訊息的優先權
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        static public int Priority(Guid messageID)
        {
            var msg = SelectWhere(string.Format("where message_id = '{0}'", messageID)).SingleOrDefault();

            return msg.message_priority;
        }
        #endregion
    }
    public class dmdSchedule : ASI.Wanda.DCU.DB.Tables.Table<dmd_schedule> 
    {
        #region Methods
        static public dmd_schedule SelectSchedule(Guid scheduleID)
        {
            return Select(scheduleID);
        }
        static public void InsertSchedule(Guid scheduleID, string scheduleName, bool isEnable, DateTime startDate, DateTime endDate)
        {
            Insert(
                      scheduleID 
                    , scheduleName
                    , isEnable 
                    , startDate
                    , endDate
                    );
        }
        static public void UpdateSchedule(Guid scheduleID, string scheduleName, bool isEnable, DateTime startDate, DateTime endDate)
        {
            Update(
                      scheduleID
                    , scheduleName
                    , isEnable
                    , startDate
                    , endDate
                    );
        }
        static public void DeleteSchedule(Guid scheduleID)
        {
            Delete(scheduleID);
        }
        #endregion
    }
    public class dmdSchedulePlayList : ASI.Wanda.DCU.DB.Tables.Table<dmd_schedule_playlist>
    {
        #region Methods
        static public void InsertSchedulePlayListItem(Guid scheduleID, Guid messageID, string stationID, string deviceID)
        {
            Insert(
                      scheduleID
                    , messageID
                    , stationID
                    , deviceID
                    , new DateTime()
                    );
        }
        static public void DeleteSchedulePlayListItems(Guid scheduleID)
        {
            string whereString = string.Format("where schedule_id = '{0}'", scheduleID);
            DeleteWhere(whereString);
        }
        #endregion
    }
    public class dmdTarget : ASI.Wanda.DCU.DB.Tables.Table<dmd_target>
    {
        #region Methods
        public static List<dmd_target> SelectPanelStatusError( List<string> Panel)
        {
            var SeatList = SelectWhere(string.Format("where device_id = 1" )).ToList();

            foreach (var seat in SeatList)
            { 
                Panel.Add(seat.station_id);
                Panel.Add(seat.area_id);
                Panel.Add(seat.device_id);
            }
            return SeatList;
        }
        #endregion
    }
    public class dmdTrainMessage : ASI.Wanda.DCU.DB.Tables.Table<dmd_train_message>
    {
        #region Methods
        public static void UpdateTrainMessages(
              string messageID, int messageType, int messageSubType, int messagePriority
            , int moveMode, int moveSpeed, int displayTimes, int countDownDisplayInterval
            , string messageContentCHN, string fontTypeCHN, int fontSizeCHN, string fontColorCHN
            , string messageContentENG, string fontTypeENG, int fontSizeENG, string fontColorENG)
        {
            Update(
                  messageID, messageType, messageSubType, messagePriority
                , moveMode, moveSpeed, displayTimes, countDownDisplayInterval
                , messageContentCHN, fontTypeCHN, fontSizeCHN, fontColorCHN
                , messageContentENG, fontTypeENG, fontSizeENG, fontColorENG);
        }


        /// <summary>
        /// 取得訊息的優先權
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        static public int Priority(Guid messageID)
        {
            var msg = SelectWhere(string.Format("where message_id = '{0}'", messageID)).SingleOrDefault();

            return msg.message_priority;
        }
        #endregion
    }


}
