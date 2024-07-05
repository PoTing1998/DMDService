using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.PA.FromCMFT
{
    #region CMFT Server to HMI

    /// <summary>
    /// 預錄廣播傳送回應
    /// </summary>
    public class Res_SendPreRecordVoice : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendPreRecordVoice(string seatID) : base(seatID)
        {
           
        }

        /// <summary>
        /// 車站ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 控制權取得成功與否
        /// </summary>
        public bool Result { get; set; }

    }

    /// <summary>
    /// 口語廣播傳送回應
    /// </summary>
    public class Res_SendInstantVoice : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendInstantVoice(string seatID) : base(seatID)
        {
           
        }

        /// <summary>
        /// 車站ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 控制權取得成功與否
        /// </summary>
        public bool Result { get; set; }

    }


    #endregion

    #region CMFT Server 內部模組使用

    /// <summary>
    /// 取得車站控制權
    /// </summary>
    public class GetPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public GetPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 1
        /// </summary>
        public readonly int PacketNum = 1;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

    }

    /// <summary>
    /// 釋放車站控制權
    /// </summary>
    public class RealeasePAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public RealeasePAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 2
        /// </summary>
        public readonly int PacketNum = 2;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

    }

    /// <summary>
    /// 查詢車站控制權狀態
    /// </summary>
    public class GetPAAuthorityStatus : ASI.Wanda.CMFT.JsonObject.Base
    {
        public GetPAAuthorityStatus(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 3
        /// </summary>
        public readonly int PacketNum = 3;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

    }

    /// <summary>
    /// 設定車站音量
    /// </summary>
    public class SetStationVolume : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SetStationVolume(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 41
        /// </summary>
        public readonly int PacketNum = 41;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 尖峰時段音量 0~255
        /// </summary>
        public int PeakTime { get; set; }

        /// <summary>
        /// 離峰時段音量 0~255
        /// </summary>
        public int OffPeakTime { get; set; }

        /// <summary>
        /// 夜間時段音量 0~255
        /// </summary>
        public int NightTime { get; set; }

    }

    /// <summary>
    /// 設定尖峰、離峰、夜間時段
    /// </summary>
    public class SetTimeInteval : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SetTimeInteval(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 43
        /// </summary>
        public readonly int PacketNum = 43;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 1:星期一、2:星期二……6:星期六、7:星期日
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Time Interval個數
        /// </summary>
        public int Count 
        {
            get 
            {
                return TimeIntevals.Count;
            }
        }

        /// <summary>
        /// Time Inteval設定，以起始時間(例:0930)為key
        /// </summary>
        public Dictionary<string, TimeInteval> TimeIntevals { get; set; } = new Dictionary<string, TimeInteval>();
    }

    /// <summary>
    /// Time Inteval設定(Packet Num 43設定尖峰、離峰、夜間時段的細項設定)
    /// </summary>
    public class TimeInteval
    {
        /// <summary>
        /// 1:尖峰、2:離峰、3:夜間
        /// </summary>
        public ASI.Wanda.CMFT.Enum.PeakTimeType PeakTimeType { get; set; }

        /// <summary>
        /// 起始時間。例:0930
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 結束時間。例:1830
        /// </summary>
        public string EndTime { get; set; }
    }

    /// <summary>
    /// 取得緊急廣播控制權
    /// </summary>
    public class GetEmergencyPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public GetEmergencyPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 144
        /// </summary>
        public readonly int PacketNum = 44;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

    }

    /// <summary>
    /// 取得緊急廣播控制權
    /// </summary>
    public class ReleaseEmergencyPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public ReleaseEmergencyPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 145
        /// </summary>
        public readonly int PacketNum = 45;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

    }

    /// <summary>
    /// 回應通知開始進行排程廣播(HMI-Server自定義協定)
    /// </summary>
    public class AckStartPASchedule : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckStartPASchedule(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 81
        /// </summary>
        public readonly int PacketNum = 81;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 播放是否成功
        /// 0:播放成功
        /// 1:播放失敗
        /// </summary>
        public int HMIResult { get; set; }
    }

    /// <summary>
    /// 回應取得車站控制權
    /// </summary>
    public class AckGetPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckGetPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 101
        /// </summary>
        public readonly int PacketNum = 101;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車站控制權取得狀態
        /// 0:成功取得車站控制權
        /// 1:取得失敗，斷線
        /// 2:取得失敗，車站廣播控制權使用中
        /// 3:取得失敗，OCC其他席位廣播控制權使用中
        /// </summary>
        public int PAAuthority { get; set; }
    }

    /// <summary>
    /// 回應釋放車站控制權
    /// </summary>
    public class AckRealeasePAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckRealeasePAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 102
        /// </summary>
        public readonly int PacketNum = 102;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車站廣播控制權釋放狀態
        /// 0:成功釋放車站廣播控制權
        /// 1:釋放失敗，Ex:尚未取得控制權無須釋放
        /// </summary>
        public int PARelease { get; set; }
    }

    /// <summary>
    /// 回應查詢車站控制權狀態
    /// </summary>
    public class AckGetPAAuthorityStatus : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckGetPAAuthorityStatus(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 103
        /// </summary>
        public readonly int PacketNum = 103;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車站控制權狀態
        /// 0:車站控制權未被使用
        /// 1:取得失敗，斷線
        /// 2:車站廣播控制權使用中
        /// 3:OCC其他席位廣播控制權使用中
        /// </summary>
        public int PAAuthorityStatus { get; set; }
    }

    /// <summary>
    /// 回應設定車站音量
    /// </summary>
    public class AckSetStationVolume : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckSetStationVolume(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 141
        /// </summary>
        public readonly int PacketNum = 141;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 設定是否成功
        /// 0:設定成功
        /// 1:設定失敗
        /// </summary>
        public int PAResult { get; set; }
    }

    /// <summary>
    /// 回應設定尖峰、離峰、夜間時段
    /// </summary>
    public class AckSetTimeInterval : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckSetTimeInterval(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 143
        /// </summary>
        public readonly int PacketNum = 143;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 設定是否成功
        /// 0:設定成功
        /// 1:設定失敗
        /// </summary>
        public int PAResult { get; set; }
    }

    /// <summary>
    /// 回應取得緊急廣播控制權
    /// </summary>
    public class AckGetEmergencyPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckGetEmergencyPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 144
        /// </summary>
        public readonly int PacketNum = 144;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車站控制權取得狀態
        /// 0:成功取得緊急廣播控制權
        /// 1:取得失敗，斷線
        /// 2:取得失敗，車站廣播控制權使用中
        /// </summary>
        public int PAAuthority { get; set; }
    }

    /// <summary>
    /// 回應取得緊急廣播控制權
    /// </summary>
    public class AckReleaseEmergencyPAAuthority : ASI.Wanda.CMFT.JsonObject.Base
    {
        public AckReleaseEmergencyPAAuthority(ASI.Wanda.CMFT.Enum.COMDevice messageFrom, string stationID) : base(messageFrom)
        {
            MessageFrom = messageFrom;
            StationID = stationID;
        }

        /// <summary>
        /// Packet Num 145
        /// </summary>
        public readonly int PacketNum = 145;

        /// <summary>
        /// 車站編號 LG01、DEPOT、BBS1、BBS2
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 車站廣播控制權釋放狀態
        /// 0:成功釋放緊急廣播控制權
        /// 1:釋放失敗，Ex:尚未取得控制權無須釋放
        /// </summary>
        public int PARelease { get; set; }
    }

    #endregion
}
