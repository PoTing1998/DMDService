using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Wanda.CMFT.DB.Models.System;

namespace ASI.Wanda.CMFT.DB.Tables.System
{

    public class sysAlarm : Table<sys_alarm>
    {
        #region Methods

        /// <summary>
        /// 取得最近的一筆告警紀錄
        /// </summary>
        /// <param name="equipID">設備代碼</param>
        /// <param name="alarmListID">告警代碼</param>
        /// <returns></returns>
        public static ASI.Wanda.CMFT.DB.Models.System.sys_alarm SelectLastSystemAlarm(string equipID, string alarmListID)
        {
            return SelectWhere($"where equip_id = '{equipID}' and alarm_list_id = '{alarmListID}'", Table<sys_alarm>.eSortWay.Desc).FirstOrDefault();
        }

        /// <summary>
        /// 出現異常 高(1) 未確認
        /// 出現異常 中(2) 未確認
        /// 出現異常 低(3) 未確認
        /// 出現異常 高(1) 已確認
        /// 出現異常 中(2) 已確認
        /// 出現異常 低(3) 已確認
        /// 異常回復 高(1) 未確認 select 
        /// 異常回復 中(2) 未確認 select 
        /// 異常回復 低(3) 未確認 select 
        /// 異常回復 高(1) 已確認 select
        /// 異常回復 中(2) 已確認 select
        /// 異常回復 低(3) 已確認 select
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.System.sys_alarm> SelectUnReleaseedAlarms()
        {
            var temp = SelectAll().Where(x => string.IsNullOrEmpty(x.release_time) == true).ToList();
            return temp;
        }
        /// <summary>
        /// 出現異常 高(1) 未確認 select
        /// 出現異常 中(2) 未確認 select 
        /// 出現異常 低(3) 未確認 select 
        /// 出現異常 高(1) 已確認 select 
        /// 出現異常 中(2) 已確認 select 
        /// 出現異常 低(3) 已確認 select 
        /// 異常回復 高(1) 未確認 select 
        /// 異常回復 中(2) 未確認 select 
        /// 異常回復 低(3) 未確認 select 
        /// 異常回復 高(1) 已確認 
        /// 異常回復 中(2) 已確認 
        /// 異常回復 低(3) 已確認 
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.System.sys_alarm> SelectDisplayAlarm()
        {
            string whereString = string.Format("where (release_time = '{0}' and check_time = '{1}') or (release_time = '{2}') or (check_time = '{3}')"
                , string.Empty
                , string.Empty
                , string.Empty
                , string.Empty);

            var temp = SelectWhere(whereString);

            return temp;
        }

        /// <summary>
        /// 出現異常 高(1) 未確認
        /// 出現異常 中(2) 未確認 
        /// 出現異常 低(3) 未確認 
        /// 出現異常 高(1) 已確認 
        /// 出現異常 中(2) 已確認 
        /// 出現異常 低(3) 已確認  
        /// 異常回復 高(1) 未確認 
        /// 異常回復 中(2) 未確認 
        /// 異常回復 低(3) 未確認 
        /// 異常回復 高(1) 已確認 select 
        /// 異常回復 中(2) 已確認 select 
        /// 異常回復 低(3) 已確認 select 
        /// </summary>
        /// <returns></returns>
        public static List<ASI.Wanda.CMFT.DB.Models.System.sys_alarm> SelectHistoryAlarm()
        {
            var temp = SelectAll()
                .Where(x => string.IsNullOrEmpty(x.release_time) == false && string.IsNullOrEmpty(x.check_account_id) == false)
                .ToList();

            return temp;
        }
        public static int InsertSystemAlarm(string alarmTime, string releaseTime, string checkTime, string checkAccountID, string checkAccountName, string equipID, string alarmListID)
        {
            return Insert(Guid.NewGuid().ToString(), alarmTime, releaseTime, checkTime, checkAccountID, checkAccountName, equipID, alarmListID);
        }

        public static int UpdateSystemAlarm(Guid alarmID, string alarmTime, string releaseTime, string checkTime, string checkAccountID, string checkAccountName, string equipID, string alarmListID)
        {
            return Update(alarmID, alarmTime, releaseTime, checkTime, checkAccountID, checkAccountName, equipID, alarmListID);
        }

        public static void UpdateSystemAlarmCheckState(Guid alarmID, string checkTime, string checkAccountID, string checkAccountName)
        {
            var alarm = SelectWhere(string.Format("where alarm_id = '{0}'", alarmID)).SingleOrDefault();

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

        /// <summary>
        /// 更新release_time欄位，只更新最近新增的一筆
        /// </summary>
        /// <param name="releaseTime">設備狀態恢復正常時間</param>
        /// <param name="equipID">設備編號</param>
        /// <param name="alarmListID">告警列表編號</param>
        /// <returns></returns>
        public static int UpdateReleaseTime(string releaseTime, string equipID, string alarmListID)
        {
            //取得最近新增的一筆
            var alarm = SelectWhere($"where equip_id = '{equipID}' and alarm_list_id = '{alarmListID}'", Table<sys_alarm>.eSortWay.Desc).FirstOrDefault();
            if (alarm != null)
            {
                Dictionary<string, object> columnValues = new Dictionary<string, object>();
                columnValues.Add("release_time", releaseTime);
                return UpdateWhere(columnValues, $"where alarm_id = '{alarm.alarm_id}' ");
            }

            return 0;
        }

        /// <summary>
        /// 更新release_time欄位
        /// </summary>
        /// <param name="alarmID">告警紀錄ID(uuid)</param>
        /// <param name="releaseTime">設備狀態恢復正常時間</param>
        /// <returns></returns>
        public static int UpdateReleaseTime(string alarmID,string releaseTime)
        {            
            Dictionary<string, object> columnValues = new Dictionary<string, object>();
            columnValues.Add("release_time", releaseTime);
            return UpdateWhere(columnValues, $"where alarm_id = '{alarmID}' ");
        }

        public static int DeleteSystemAlarm(string equipID, string alarmListID)
        {
            return DeleteWhere($"where equip_id = '{equipID}' and alarm_list_id = '{alarmListID}'");
        }
        #endregion
    }

    public class sysAlarmList : ASI.Wanda.CMFT.DB.Tables.Table<sys_alarm_list>
    {
        #region Methods

        /// <summary>
        /// 依據alarm_source取得
        /// </summary>
        /// <param name="alarmSource">系統別，例如：PA、DMD、TETRA</param>
        /// <returns></returns>
        static public List<sys_alarm_list> GetSystemAlarm(string alarmSource)
        {
            string sWhereString = $"where alarm_source = '{alarmSource}' ";
            List<sys_alarm_list> oSysAlarmList = SelectWhere(sWhereString);

            return oSysAlarmList;
        }
        public static int UpdateEnable(string alarmListID, bool isEnable)
        {
            Dictionary<string, object> columnValues = new Dictionary<string, object>();
            columnValues.Add("is_enable", isEnable);
            return UpdateWhere(columnValues, $"where alarm_list_id = '{alarmListID}' ");
        }
        #endregion
    }

    public class sysConfig : ASI.Wanda.CMFT.DB.Tables.Table<sys_config>
    {
        #region Methods
        public static void InsertSystemConfig(string configName, string configValue, string configDescription)
        {
            Update(configName, configValue, configDescription);
        }
     
        #endregion
    }

    public class sysEquipAlarmMatch : ASI.Wanda.CMFT.DB.Tables.Table<sys_equip_alarm_match>
    {
        #region Methods

        /// <summary>
        /// 依據設備編號取得該設備的所有告警編號
        /// </summary>
        /// <param name="equipID">設備編號</param>
        /// <returns></returns>
        public static List<sys_equip_alarm_match> GetSysEequipAlarmMatch(string equipID)
        {
            string sWhereString = $"where equip_id = '{equipID}'";
            var oSysEequipAlarmMatch = SelectWhere(sWhereString);

            return oSysEequipAlarmMatch;
        }

        /// <summary>
        /// 依據設備編號及該設備的狀態參考值取得所屬的告警編號
        /// </summary>
        /// <param name="equipID">設備編號</param>
        /// <param name="refValue">設備的狀態參考值</param>
        /// <returns></returns>
        public static sys_equip_alarm_match GetSysEequipAlarmMatch(string equipID,string refValue)
        {
            string sWhereString = $"where equip_id = '{equipID}' and ref_value ='{refValue}' ";
            var oSysEequipAlarmMatch = SelectWhere(sWhereString);

            return oSysEequipAlarmMatch.FirstOrDefault();
        }

        /// <summary>
        /// 依據系統編號取得該通訊子系統所有的告警編號
        /// </summary>
        /// <param name="systemID">系統編號，例如：PA、DMD、TETRA</param>
        /// <returns></returns>
        public static List<sys_equip_alarm_match> GetBySystemID(string systemID)
        {
            string sWhereString = $"where alarm_list_id like '{systemID}%' ";
            var oSysEequipAlarmMatch = SelectWhere(sWhereString);

            return oSysEequipAlarmMatch;
        }

        #endregion
    }

    public class sysEquipStatus : ASI.Wanda.CMFT.DB.Tables.Table<sys_equip_status>
    {
        #region Methods

        /// <summary>
        /// 取得指定通訊子系統的設備狀態
        /// </summary>
        /// <param name="systemID">通訊子系統編碼，如：PA、DMD、TETRA...</param>
        /// <returns></returns>
        public static List<sys_equip_status> GetSysEquipStatus(string systemID)
        {
            string sWhereString = $"where system_id = '{systemID}' ";
            var oSysEquipStatus = SelectWhere(sWhereString);

            return oSysEquipStatus;
        }

        /// <summary>
        /// 修改指定equip_id的equip_status欄位
        /// </summary>
        /// <param name="equipID">指定equip_id</param>
        /// <param name="status">equip_status欄位更新值</param>
        /// <returns></returns>
        public static int UpdSysEquipStatus(string equipID, bool status)
        {
            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            columnVals.Add("equip_status", status);

            string sWhere = $" where equip_id = '{equipID}' ";

            return UpdateWhere(columnVals, sWhere);
        }


        public static List<string> GetOverviewSubSystemList(string regionID)
        {
            var regionSubSystemList = GetOverviewEquipList(regionID)
                .Select(x => x.system_id)
                .Distinct()
                .ToList();
            return regionSubSystemList;
        }
        public static List<sys_equip_status> GetOverviewEquipErrorList(string regionID)
        {
            string whereString = string.Format("where region_id = '{0}' and equip_status = 'false' and is_overview ='true' ", regionID);
            return SelectWhere(whereString);
        }
        public static List<sys_equip_status> GetOverviewEquipErrorList()
        {
            string whereString = string.Format("where equip_status = 'false' and is_overview ='true' ");
            return SelectWhere(whereString);
        }
        public static List<sys_equip_status> GetOverviewEquipList(string regionID)
        {
            string whereString = string.Format("where region_id = '{0}' and is_overview ='true' ", regionID);
            return SelectWhere(whereString);
        }
        public static List<sys_equip_status> GetOverviewSubSystemEquipList(string regionID, string systemID)
        {
            string whereString = string.Format("where region_id = '{0}' and system_id = '{1}' and is_overview ='true' ", regionID, systemID);
            return SelectWhere(whereString);
        }
        #endregion
    }

    public class sysOperLog : ASI.Wanda.CMFT.DB.Tables.Table<sys_oper_log>
    {
        #region Methods
        public static void InsertOperationLog(Guid logID, string accountID, string accountName, string operationID, string logContent)
        {
            Insert(logID, accountID, accountName, operationID, logContent);
        }
        #endregion
    }

    public class sysRegion : ASI.Wanda.CMFT.DB.Tables.Table<sys_region>
    {
        #region Methods

        public static string GetRegionName(string regionID)
        {
            string whereString = string.Format("where region_id = '{0}' ", regionID);
            sys_region selectedRegion = SelectWhere(whereString).SingleOrDefault();
            string selectedRegionName = string.Empty;
            if (selectedRegion!= null)
            {
                selectedRegionName = selectedRegion.region_name;
            }
            return selectedRegionName;
        }

        #endregion
    }

    public class sysSeat : ASI.Wanda.CMFT.DB.Tables.Table<sys_seat>
    {
        #region Methods

        public static List<sys_seat> GetAll()
        {
            var SeatList = SelectAll();

            return SeatList;
        }

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

    public class sysStation : ASI.Wanda.CMFT.DB.Tables.Table<sys_station>
    {
        #region Methods
        static public string GetStationName(string stationID)
        {
            string stationName = SelectAll()
                .FirstOrDefault(x => x.station_id == stationID)
                ?.station_name;
            return stationName;
        }
        static public string GetStationID(string stationName)
        {
            string stationID = SelectAll()
                .FirstOrDefault(x => x.station_name == stationName)
                ?.station_id;
            return stationID;
        }
        static public List<sys_station> GetPhase1Stations()
        {
            var temp = new List<sys_station>();
            foreach (var item in SelectAll())
            {
                if( item.station_id.Equals("LG09") )
                {
                    break;
                }
                else
                {
                    temp.Add(item);
                }
            }
            return temp;
        }
       
        #endregion
    }

    public class sysStationArea : ASI.Wanda.CMFT.DB.Tables.Table<sys_station_area>
    {
        #region Methods
        static public string GetAreaName(string stationID,string areaID)
        {
            string areaName = SelectAll()
                .Where(x => (x.station_id == stationID) && (x.area_id == areaID))
                .FirstOrDefault()
                ?.area_name;
            return areaName;
        }
        #endregion
    }

}
