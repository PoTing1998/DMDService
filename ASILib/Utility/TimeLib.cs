using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Utility
{
    public class TimeLib
    {
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime([System.Runtime.InteropServices.In] ref SYSTEMTIME st);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime([System.Runtime.InteropServices.In] ref SYSTEMTIME st);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemTime([System.Runtime.InteropServices.In] ref SYSTEMTIME st);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetLocalTime([System.Runtime.InteropServices.In] ref SYSTEMTIME st);

        /// <summary>
        /// 設定系統時間(UTC)
        /// </summary>
        /// <param name="pTime"></param>
        /// <returns></returns>
        public static int SetSystemTime(DateTime pTime)
        {
            try
            {
                var sysTime = Format2Time(pTime);
                SetSystemTime(ref sysTime);
                return 1;
            }
            catch { }
            return -1;
        }

        /// <summary>
        /// 設定當地時間(台北時間,UTC + 8:00)
        /// </summary>
        /// <param name="pTime"></param>
        /// <returns></returns>
        public static int SetLocalTime(DateTime pTime)
        {
            try
            {
                var sysTime = Format2Time(pTime);
                SetLocalTime(ref sysTime);
                return 1;
            }
            catch { }
            return -1;
        }

        /// <summary>
        /// 取得系統時間(UTC)
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetSystemTime()
        {
            try
            {
                SYSTEMTIME sysTime = new SYSTEMTIME();
                GetSystemTime(ref sysTime);
                var dateTime = Format2Time(sysTime);
                return dateTime;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 取得當地時間(台北時間,UTC + 8:00)
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetLocalTime()
        {
            try
            {
                SYSTEMTIME localTime = new SYSTEMTIME();
                GetLocalTime(ref localTime);
                var dateTime = Format2Time(localTime);
                return dateTime;
            }
            catch { }
            return null;
        }


        public static DateTime? GetNetworkTime(  string ntpServer = "pool.ntp.org")
        {
            //"time.windows.com";
            try
            {
                
                var ntpData = new byte[48];
                ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

                var addresses = Dns.GetHostEntry(ntpServer).AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();

                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
                ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);


                return networkDateTime;
            }
            catch { }
            return null;
 
        }
     
        /// <summary>
        /// 將字串格式YYYYMMDDhhnnss.mms轉成DateTime
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static DateTime MakeTime(string pString)
        {
            int year = 0, month = 0, date = 0, hour = 0, minute = 0, second = 0, msec = 0;
            if (pString.Length >= 4)
                year = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(0, 4));
            if (pString.Length >= 6)
                month = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(4, 2));
            if (pString.Length >= 8)
                date = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(6, 2));
            if (pString.Length >= 10)
                hour = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(8, 2));
            if (pString.Length >= 12)
                minute = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(10, 2));
            if (pString.Length >= 14)
                second = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(12, 2));
            if (pString.Length >= 16 && pString.Substring(14, 1) == ".")
                msec = ASI.Lib.Text.Parsing.String.String2Int(pString.Substring(15));

            if (year > 1900
                 && month >= 1 && month <= 12
                 && date >= 1 && date <= 31
                 && hour >= 0 && hour <= 23
                 && minute >= 0 && minute <= 59
                 && second >= 0 && second <= 59
                 && msec >= 0 && msec <= 999)
                return new DateTime(year, month, date, hour, minute, second, msec);
            else
                return new DateTime(2010, 1, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// 將字串格式YYYY/MM/DD hh:nn:ss.mms轉成DateTime
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static DateTime Format2Time(string pString)
        {
            string tm_str = "";
            tm_str = pString.Substring(0, 4) + pString.Substring(5, 2) + pString.Substring(8, 2) +
                        pString.Substring(11, 2) + pString.Substring(14, 2) + pString.Substring(17);

            return MakeTime(tm_str);
        }

        /// <summary>
        /// 將DateTime轉成SYSTEMTIME
        /// </summary>
        /// <param name="sysTime"></param>
        /// <returns></returns>
        public static SYSTEMTIME Format2Time(DateTime dateTime)
        {
            var sysTime = new SYSTEMTIME();
            sysTime.wYear = (short)dateTime.Year; // must be short
            sysTime.wMonth = (short)dateTime.Month;
            sysTime.wDay = (short)dateTime.Day;
            sysTime.wHour = (short)dateTime.Hour;
            sysTime.wMinute = (short)dateTime.Minute;
            sysTime.wSecond = (short)dateTime.Second;
            sysTime.wMilliseconds = (short)dateTime.Millisecond;

            return sysTime;
        }

        /// <summary>
        /// 將SYSTEMTIME轉成DateTime
        /// </summary>
        /// <param name="sysTime"></param>
        /// <returns></returns>
        public static DateTime Format2Time(SYSTEMTIME sysTime)
        {
            var dateTime = new DateTime(
                       sysTime.wYear
                     , sysTime.wMonth
                     , sysTime.wDay
                     , sysTime.wHour
                     , sysTime.wMinute
                     , sysTime.wSecond
                     , sysTime.wMilliseconds);
            return dateTime;
        }

        //以SetLocalTime取代
        //public static int Set(System.DateTime pTime)
        //{
        //    try
        //    {
        //        if (pTime.Year < 2014)
        //        {
        //            Console.WriteLine("Command error : Time format is wrong");
        //            return -1;
        //        }
        //        Console.WriteLine("Set System Time : " + pTime.ToString("yyyyMMddHHmmss.fff"));

        //        pTime = pTime.ToUniversalTime();

        //        SYSTEMTIME st = new SYSTEMTIME();
        //        st.wYear = (short)pTime.Year; // must be short
        //        st.wMonth = (short)pTime.Month;
        //        st.wDay = (short)pTime.Day;
        //        st.wHour = (short)pTime.Hour;
        //        st.wMinute = (short)pTime.Minute;
        //        st.wSecond = (short)pTime.Second;
        //        st.wMilliseconds = (short)pTime.Millisecond;

        //        SetSystemTime(ref st); // invoke this method.
        //        return 1;
        //    }
        //    catch { }
        //    return -1;
        //}
    }
}
