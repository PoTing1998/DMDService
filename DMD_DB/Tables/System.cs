using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Claims;

using ASI.Wanda.DMD.DB.Models.System;
using ASI.Wanda.DMD.DB.Tables;

namespace ASI.Wanda.DMD.DB.Tables.System
{
    public class sysAlarm : Table<sys_alarm>
    {
        #region Methods
        public static void UpdateSystemAlarm(Guid alarmID, string alarmTime, string releaseTime, string checkTime, string checkAccountID, string checkAccountName, string equipID, string alarmListID)
        {
            Update(alarmID, alarmTime, releaseTime, checkTime, checkAccountID, checkAccountName, equipID, alarmListID);
        }
        public static void UpdateSystemAlarmCheckState(Guid alarmID, string checkTime, string checkAccountID, string checkAccountName)
        {
            var alarm =
                 SelectWhere(string.Format("where alarm_id = '{0}'", alarmID))
                 .SingleOrDefault();

            Update(
                  alarm.alarm_id
                , alarm.alarm_time
                , alarm.release_time
                , checkTime
                , checkAccountID
                , checkAccountName
                , alarm.equip_id
                , alarm.alarm_list_id);
        }
        #endregion
    }
    public class sysAlarmList : ASI.Wanda.DMD.DB.Tables.Table<sys_alarm_list>
    {
        #region Methods

        #endregion
    }
    public class sysConfig : ASI.Wanda.DMD.DB.Tables.Table<sys_config>
    {
        #region Methods
        public static void InsertSystemConfig(string configName, string configValue, string configDescription, string system_id, string remark)
        {
            Insert(configName, configValue, configDescription, system_id, remark);
        }
        public static void UpdataSystemConfig(string configName, string configValue, string configDescription, string system_id, string remark)
        {
            Update(configName, configValue, configDescription, system_id, remark);
        }
        static public void DeletePlayingItem(string configName)
        {
            string whereString = string.Format("where config_name = '{0}' ", configName);
            DeleteWhere(whereString);
        }
       
        #endregion
    }
    public class sysEquipStatus : ASI.Wanda.DMD.DB.Tables.Table<sys_equip_status>
    {
        #region Methods

        public static void UpdateEquipStatus(string equip_id, bool equip_status)
        {
            var equip =
                 SelectWhere(string.Format("where equip_id = '{0}'", equip_id))
                 .SingleOrDefault();

            Update(
                equip.equip_id
              , equip.region_id
              , equip.region_name
              , equip.place_name
              , equip.area_name
              , equip.system_id
              , equip.system_name
              , equip.equip_type
              , equip_status
              );

        }
        public static void SelectBlackList(bool equip_status, string system_id, List<string> blackLast)
        {
            var equipList =
               SelectWhere(string.Format("where system_id = '{0}' AND equip_status = '{1}'", system_id, equip_status))
               .ToList();
            ;
            foreach (var equip in equipList)
            {
                blackLast.Add(equip.equip_id); // 添加每個 equip.equip_id 到 blackList
            }
        }


  
        #endregion
    }
    public class sysOperLog : ASI.Wanda.DMD.DB.Tables.Table<sys_oper_log>
    {
        #region Methods
        public static void InsertOperationLog(Guid logID, string accountID, string accountName, string operationID, string logContent)
        {
            Insert(logID, accountID, accountName, operationID, logContent);
        }
        #endregion
    }
    public class sysRegion : ASI.Wanda.DMD.DB.Tables.Table<sys_region>
    {
        #region Methods

        #endregion
    }
    public class sysSeat : ASI.Wanda.DMD.DB.Tables.Table<sys_seat>
    {
        #region Methods

        /// <summary>
        /// 依據pa_seat_num取得相對應的seat_id
        /// </summary>
        /// <param name="paSeatNum">PA系統席位定義編號</param>
        /// <returns>若無對應則回傳空值("")</returns>
        public static string GetSeatID(int paSeatNum)
        {
            string sSeatID = "";
            var SeatList = SelectWhere($"where pa_seat_num = '{paSeatNum}'");
            if (SeatList != null && SeatList.Count == 0)
            {
                sSeatID = SeatList[0].seat_id;
            }

            return sSeatID;
        }

        /// <summary>
        /// 依據seat_id取得相對應的pa_seat_num
        /// </summary>
        /// <param name="seatID">席位編號</param>
        /// <returns>若無對應則回傳0</returns>
        public static int GetPASeatNum(string seatID)
        {
            int iPASeatNum = 0;
            var SeatList = SelectWhere($"where seat_id = '{seatID}'");
            if (SeatList != null && SeatList.Count == 0)
            {
                iPASeatNum = SeatList[0].pa_seat_id;
            }

            return iPASeatNum;
        }



        #endregion
    }
    public class sysStation : ASI.Wanda.DMD.DB.Tables.Table<sys_station>
    {
        #region Methods
        static public string GetStationName(string stationID)
        {
            string stationName = SelectAll().First(x => x.station_id == stationID).station_name;
            return stationName;
        }
        #endregion
    }
    public class sysStationArea : ASI.Wanda.DMD.DB.Tables.Table<sys_station_area>
    {
        #region Methods
        #endregion
    }

    public class sysIP : ASI.Wanda.DMD.DB.Tables.Table<sys_ip>
    {
        #region Methods

        static public List<sys_ip> SelectIP(string servername)
        {
            var whereString = string.Format("where group_id = '{0}'", servername);
            var devices = SelectWhere(whereString);
            return devices;
        } 
        #endregion
    }
}
