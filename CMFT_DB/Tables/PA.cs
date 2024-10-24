using System;
using System.Linq;
using System.Collections.Generic;
using ASI.Wanda.CMFT.DB.Models.PA;

namespace ASI.Wanda.CMFT.DB.Tables.PA
{
    public class paDaypartTime : ASI.Wanda.CMFT.DB.Tables.Table<pa_daypart_time>
    {
        #region Methods
        static public pa_daypart_time SelectDaypartTime(Guid daypartTimeID)
        {
            return Select(daypartTimeID);
        }
        static public void InsertDaypartTime(Guid daypartTimeID, string stationID, int weekDay, string startTime, string endTime, int dayPart)
        {
            Insert(daypartTimeID, stationID, weekDay, startTime, endTime, dayPart);
        }
        static public void UpdateDaypartTime(Guid daypartTimeID, string stationID, int weekDay, string startTime, string endTime, int dayPart)
        {
            Update(daypartTimeID, stationID, weekDay, startTime, endTime, dayPart);
        }
        static public void DeleteDaypartTime(Guid daypartTimeID)
        {
            Delete(daypartTimeID);
        }
        static public void DeleteDaypartTime(string stationID, int weekDay)
        {
            string whereString = string.Format("where station_id = '{0}' and week_day = {1} ", stationID, weekDay);
            DeleteWhere(whereString);
        }
        #endregion

        #region Source
        public static Dictionary<int, string> WeekDays
        {
            get
            {
                return new Dictionary<int, string>
                {
                      { 1, "星期一" }
                    , { 2, "星期二" }
                    , { 3, "星期三" }
                    , { 4, "星期四" }
                    , { 5, "星期五" }
                    , { 6, "星期六" }
                    , { 7, "星期日" }
                };
            }
        }
        #endregion
    }
    public class paDaypartVolume : ASI.Wanda.CMFT.DB.Tables.Table<pa_daypart_volume>
    {
        #region Methods
        static public void UpdateDaypartTime(string stationID, int daypart, int volume)
        {
            Update(stationID, daypart, volume);
        }
        #endregion

        #region Source
        public static Dictionary<int, string> Dayparts
        {
            get
            {
                return new Dictionary<int, string>
                {
                      { 1, "尖峰時段" }
                    , { 2, "離峰時段" }
                    , { 3, "夜間時段" }
                };
            }
        }
        #endregion
    }
    public class paGroup : ASI.Wanda.CMFT.DB.Tables.Table<pa_group>
    {
        #region Methods
        static public pa_group SelectGroup(Guid groupID)
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
    public class paGroupTarget : ASI.Wanda.CMFT.DB.Tables.Table<pa_group_target>
    {
        #region Methods
        static public List<pa_group_target> SelectGroupTargets(Guid groupID)
        {
            var whereString = string.Format("where group_id = '{0}'", groupID);
            var devices = SelectWhere(whereString);
            return devices;
        }
        static public void InsertGroupTarget(Guid groupID, string stationID, string AreaID)
        {
            Insert(groupID, stationID, AreaID);
        }
        static public void DeleteGroupTarget(Guid groupID, string stationID, string AreaID)
        {
            Delete(groupID, stationID, AreaID);
        }
        static public void DeleteGroupTargets(Guid groupID)
        {
            string whereString = string.Format("where group_id = '{0}'", groupID);
            DeleteWhere(whereString);
        }
        #endregion
    }
    public class paPreRecordVoice : ASI.Wanda.CMFT.DB.Tables.Table<pa_pre_record_voice>
    {
        #region Methods

        static public pa_pre_record_voice SelectVoice(string folderID)
        {
            return Select(folderID);
        }

        public static int InsertPreRecordVoice(string folderID, int voiceType, int voicePriority, string voiceName, string version, bool status, string fileType, string fileName, string voiceContent, bool includeCHN, bool includeENG, bool includeTWN, bool includeHAKKA)
        {
            return Insert(folderID, voiceType, voicePriority, voiceName, version, status, fileType, fileName, voiceContent, includeCHN, includeENG, includeTWN, includeHAKKA);
        }

        public static int UpdatePreRecordVoice(string folderID, int voiceType, int voicePriority, string voiceName, string version, bool status, string fileType, string fileName, string voiceContent, bool includeCHN, bool includeENG, bool includeTWN, bool includeHAKKA)
        {
            return Update(folderID, voiceType, voicePriority, voiceName, version, status, fileType, fileName, voiceContent, includeCHN, includeENG, includeTWN, includeHAKKA);
        }

        public static int DeletePreRecordVoice(string folderID)
        {
            return Delete(folderID);
        }

        public static int DeleteBeforeTime(DateTime dateTime)
        {
            return DeleteWhere($" where ins_time < '{dateTime.ToString("yyyy/MM/dd HH:mm:ss.fff")}' ");
        }

        #endregion
    }
    public class paSchedule : ASI.Wanda.CMFT.DB.Tables.Table<pa_schedule>
    {
        #region Methods
        static public pa_schedule SelectSchedule(Guid scheduleID)
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
    public class paSchedulePlayList : ASI.Wanda.CMFT.DB.Tables.Table<pa_schedule_playlist>
    {
        #region Methods
        static public void InsertSchedulePlayListItem(Guid scheduleID, string folderID, string stationID, string areaID)
        {
            Insert(
                      scheduleID
                    , folderID
                    , stationID
                    , areaID
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
    public class paTarget : ASI.Wanda.CMFT.DB.Tables.Table<pa_target>
    {
        #region Methods
        static public List<string> SelectAllStationID()
        {
            List<string> allStationID = SelectAll()
                .Select(x => x.station_id)
                .Distinct()
                .ToList();

            return allStationID;
        }

        #endregion
    }
}
