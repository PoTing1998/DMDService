using System;
using System.Collections.Generic;
using ASI.Wanda.CMFT.DB.Models.DMD;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASI.Wanda.CMFT.DB.Tables.DMD
{
    public class dmdGroup : ASI.Wanda.CMFT.DB.Tables.Table<dmd_group>
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
    public class dmdGroupTarget : ASI.Wanda.CMFT.DB.Tables.Table<dmd_group_target>
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
    public class dmdInstantMessage : ASI.Wanda.CMFT.DB.Tables.Table<dmd_instant_message>
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
    public class dmdPlayList : ASI.Wanda.CMFT.DB.Tables.Table<dmd_playlist>
    {
        #region Methods
        static public dmd_playlist SelectPlayingItem(Guid palylistID)
        {
            return Select(palylistID);
        }

        static public void InsertPlayingItem(Guid playlistID, string stationID, string areaID, string deviceID, Guid messageID, int messageType, string sendTime)
        {
            Insert(
                   playlistID
                 , stationID
                 , areaID
                 , deviceID
                 , messageID
                 , messageType
                 , sendTime );
        }
        static public void UpdatePlayingItem(Guid playlistID, string stationID, string areaID, string deviceID, Guid messageID, int messageType, string sendTime)
        {
            Update(
                  playlistID
                , stationID
                , areaID
                , deviceID
                , messageID
                , messageType
                , sendTime);
        }

 
        /// <summary>
        /// 刪除目前播放列表中指定設備的所有資料
        /// </summary>
        /// <param name="stationID">看板所在車站ID</param>
        /// <param name="areaID">看板所區域ID</param>
        /// <param name="deviceID">看板ID</param>
        static public void DeletePlayingItemsByDevice(string stationID, string areaID, string deviceID)
        {
            string whereString = string.Format("where station_id = '{0}' and area_id = '{1}' and device_id = '{2}'", stationID, areaID, deviceID);
            DeleteWhere(whereString); 
        }

        static public void DeletePlayingItem(Guid palylistID)
        {
            Delete(palylistID);
        }
        #endregion
    }
    public class dmdPowerSetting : ASI.Wanda.CMFT.DB.Tables.Table<dmd_power_setting>
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
    public class dmdPreRecordMessage : ASI.Wanda.CMFT.DB.Tables.Table<dmd_pre_record_message>
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
        #endregion
    }
    public class dmdSchedule : ASI.Wanda.CMFT.DB.Tables.Table<dmd_schedule>
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
    public class dmdSchedulePlayList : ASI.Wanda.CMFT.DB.Tables.Table<dmd_schedule_playlist>
    {
        #region Methods
        /// <summary>
        /// 新增一條撥放內容
        /// </summary>
        /// <param name="scheduleID">排程編號</param>
        /// <param name="messageID">訊息編號</param>
        /// <param name="stationID">看板所在車站編號</param>
        /// <param name="deviceID">看板設備編號</param>
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
        /// <summary>
        /// 刪除指定排程所有內容
        /// </summary>
        /// <param name="scheduleID">排程編號</param>
        static public void DeleteSchedulePlayListItems(Guid scheduleID)
        {
            string whereString = string.Format("where schedule_id = '{0}'", scheduleID);
            DeleteWhere(whereString);
        }
        /// <summary>
        /// 取得指定排程的所有預錄訊息
        /// </summary>
        /// <param name="scheduleID">排程編號</param>
        static public List<dmd_pre_record_message> GetSchedulePlayListMessages(Guid scheduleID)
        {
            string whereString = string.Format("where schedule_id = '{0}'", scheduleID);
            var preMsgIDs = SelectWhere(whereString)
                   .Select(y => y.message_id)
                   .Distinct()
                   .ToList();
            var messageList = new List<dmd_pre_record_message>();

            foreach (var preMsgID in preMsgIDs)
            {
               var message = dmdPreRecordMessage.SelectMessage(preMsgID);
                messageList.Add(message);
            }
            return messageList;
        }
        static public List<dmd_target> GetSchedulePlayListTargets(Guid scheduleID)
        {
            string whereString = string.Format("where schedule_id = '{0}'", scheduleID);
            var schedules = SelectWhere(whereString);

            var dbAllTarget = ASI.Wanda.CMFT.DB.Tables.DMD.dmdTarget.SelectAll();
            List<dmd_target> targets = new List<dmd_target>();
            foreach (var schedule in schedules)
            {
                var target = dbAllTarget.FirstOrDefault(x => x.station_id == schedule.station_id && x.device_id == schedule.device_id);
                targets.Add(target);
            }
            return targets;
        }
        #endregion
    }
    public class dmdTarget : ASI.Wanda.CMFT.DB.Tables.Table<dmd_target>
    {
        #region Methods
        static public string GetDeviceName(string stationID, string areaID, string deviceID)
        {
            string whereString  = string.Format("where station_id = '{0}' and area_id = '{1}' and device_id = '{2}'", stationID, areaID, deviceID);
            return SelectWhere(whereString).Single().device_name;
        }


        static public Dictionary<string,string> GetTargetStations()
        {
            var stations = SelectAll()
                   .Select(x => new KeyValuePair<string, string>(x.station_id, ASI.Wanda.CMFT.DB.Tables.System.sysStation.GetStationName(x.station_id)))
                   .Distinct()
                   .ToDictionary(p => p.Key, p => p.Value);
            return stations;   
        }
        #endregion
    }
    public class dmdTrainMessage : ASI.Wanda.CMFT.DB.Tables.Table<dmd_train_message>
    {
        #region Methods
         static public void UpdateTrainMessages(
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
        #endregion
    }

    public class dmdCharBlacklist : ASI.Wanda.CMFT.DB.Tables.Table<dmd_char_blacklist>
    {
        #region Methods
        static bool IsContainBlacklistCharacter(string dmdtext)
        {
            var blacklistCharacters = SelectAll().Select(x=>x.blacklist_char);

            foreach(var blacklistCharacter in blacklistCharacters)
            {
                var isBlacklistCharacter = dmdtext.Contains(blacklistCharacter);

                if(isBlacklistCharacter == true)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
