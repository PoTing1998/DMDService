using System;
using System.Collections.Generic;
using ASI.Wanda.CMFT.DB.Models.DMD;
using System.Linq;

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
        static public void InsertPlayingItem(string stationID, string deviceID, Guid messageID)
        {
            Insert(
                   stationID
                 , deviceID
                 , messageID
                 , new DateTime());
        }
        static public void UpdatePlayingItem()
        {

        }
        static public void DeletePlayingItem()
        {

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
