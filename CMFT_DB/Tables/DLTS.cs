using System;
using System.Collections.Generic;
//using System.Windows.Media;
using ASI.Wanda.CMFT.DB.Models.DLTS;

namespace ASI.Wanda.CMFT.DB.Tables.DLTS
{
    public class dltsSeatStatus : ASI.Wanda.CMFT.DB.Tables.Table<dlts_seat_status>
    {
        /// <summary>
        /// 依據話機號碼更新當前通話狀態
        /// </summary>
        /// <param name="telNumber">話機號碼</param>
        /// <param name="curCallNumber">當前通話號碼</param>
        /// <param name="curCallStatus">當前通話狀態</param>
        static public int UpdateCallStatus(string telNumber, string curCallNumber, int curCallStatus)
        {
            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            columnVals.Add("cur_call_number", curCallNumber);
            columnVals.Add("cur_call_status", curCallStatus);            

            string sWhere = $" where tel_number = '{telNumber}' ";

            int iReturn = UpdateWhere(columnVals, sWhere);
            return iReturn;
        }
    }

    public class dltsCallerDisplay : ASI.Wanda.CMFT.DB.Tables.Table<dlts_caller_display>
    {
        /// <summary>
        /// 依據話機號碼更新或新增當前通話狀態
        /// </summary>
        /// <param name="telNumber">話機號碼</param>
        /// <param name="curCallNumber">當前通話號碼</param>
        /// <param name="curCallStatus">當前通話狀態</param>
        static public int UpdateCallStatus(string telNumber, string curCallNumber, int curCallStatus)
        {
            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            columnVals.Add("cur_call_status", curCallStatus);

            string sWhere = $" where tel_number = '{telNumber}' and cur_call_number = '{curCallNumber}'";

            int iReturn = UpdateWhere(columnVals, sWhere);
            if (iReturn == 0)
            {
                iReturn = Insert(telNumber, curCallNumber, curCallStatus);
            }

            return iReturn;
        }

        /// <summary>
        /// 依據話機號碼刪除資料列
        /// </summary>
        /// <param name="telNumber"></param>
        /// <returns></returns>
        static public int DelCallStatus(string telNumber)
        {
            string sWhere = $" where tel_number = '{telNumber}' ";
            int iReturn = DeleteWhere(sWhere);
            return iReturn;
        }

        /// <summary>
        /// 刪除cur_call_status為0(閒置)或9(話後處理)的資料列
        /// </summary>
        /// <returns></returns>
        static public int DelCallStatus09()
        {
            string sWhere = $" where cur_call_status = 0 or cur_call_status = 9";
            int iReturn = DeleteWhere(sWhere);   
            return iReturn;
        }
    }

    public class dltsCallerLog : ASI.Wanda.CMFT.DB.Tables.Table<dlts_caller_log>
    {
        /// <summary>
        /// 新增一筆dlts_caller_log
        /// </summary>
        /// <param name="logID">Guid</param>
        /// <param name="calledNumber">受話方直線電話號碼</param>
        /// <param name="callerNumber">發話方直線電話號碼</param>
        /// <param name="startTime">通話開始時間</param>
        /// <param name="endTime">通話結束時間</param>
        /// <returns></returns>
        static public int InsertCallerLog(Guid logID, string calledNumber, string callerNumber, DateTime startTime, DateTime? endTime)
        {
            int iReturn = Insert(logID, calledNumber, callerNumber, startTime, endTime);
            return iReturn;
        }

        /// <summary>
        /// 依據受話方和發話方取得最近一筆通話紀錄的log_id
        /// </summary>
        /// <param name="calledNumber">受話方直線電話號碼</param>
        /// <param name="callerNumber">發話方直線電話號碼</param>
        /// <returns></returns>
        static public string GetLastLogID(string calledNumber, string callerNumber)
        {
            string logID = "";
            string sWhere = $" where called_number = '{calledNumber}' and caller_number = '{callerNumber}' ";
            var dltsCallerLogs = SelectWhere(sWhere, Table<dlts_caller_log>.eSortWay.Desc);
            if (dltsCallerLogs?.Count > 0)
            {
                logID = dltsCallerLogs[0].log_id.ToString();
            }

            return logID;
        }

        /// <summary>
        /// 更新指定事件紀錄編號的通話結束時間
        /// </summary>
        /// <param name="logID">事件紀錄編號</param>
        /// <param name="endTime">通話結束時間</param>
        /// <returns></returns>
        static public int UpdateEndTime(string logID, DateTime endTime)
        {
            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            columnVals.Add("end_time", endTime);

            string sWhere = $" where log_id = '{logID}' ";

            int iReturn = UpdateWhere(columnVals, sWhere);           
            return iReturn;
        }

        /// <summary>
        /// 受話方和發話方若為指定電話號碼，且通話結束時間為空值，則更新通話結束時間
        /// </summary>
        /// <param name="phoneNumber">指定電話號碼</param>
        /// <param name="endTime">通話結束時間</param>
        /// <returns></returns>
        static public int UpdateEndTimeByPhoneNumber(string phoneNumber, DateTime endTime)
        {
            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            columnVals.Add("end_time", endTime);

            string sWhere = $" where end_time is null and (called_number = '{phoneNumber}' or caller_number = '{phoneNumber}') ";

            int iReturn = UpdateWhere(columnVals, sWhere);
            return iReturn;
        }
    }

    
}
