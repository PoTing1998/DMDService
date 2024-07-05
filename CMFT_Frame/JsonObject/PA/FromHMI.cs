using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.PA.FromHMI
{
    #region 功能
    /// <summary>
    /// (1)預錄廣播傳送
    /// <summary>
    public class SendPreRecordVoice : ASI.Wanda.CMFT.JsonObject.Base 
    {
        public SendPreRecordVoice(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 預錄廣播ID (folder_id)
        /// </summary>
        public List<string> voice_id { get; set; } = new List<string>();
        /// <summary>
        /// 廣播對象車站 (station_id)
        /// </summary>
        public List<string> target_station { get; set; } = new List<string>();
    }

    /// <summary>
    /// (2)口語廣播傳送
    /// </summary>
    public class SendInstantVoice : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendInstantVoice(string seatID) : base(seatID)
        {  
        }
        /// <summary>
        /// 廣播對象車站 station_id
        /// </summary>
        public List<string> target_station { get; set; } = new List<string>();
    }

    /// <summary>
    /// (3)廣播排程 (無須傳送)
    /// </summary>
    public class ScheduleSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public ScheduleSetting(string seatID) : base(seatID)
        {

        }

        /// <summary>
        /// 排程ID
        /// </summary>
        public string sched_id { get; set; }

        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }
    }

    /// <summary>
    /// (4)廣播結束
    /// </summary>
    public class SendVoiceFinished : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendVoiceFinished(string seatID) : base(seatID)
        {

        }
        /// <summary>
        /// 廣播對象車站 (station_id)
        /// </summary>
        public List<string> target_station { get; set; } = new List<string>();
    }
    #endregion

    #region 設定
    /// <summary>
    /// (5)預錄廣播設定(廢棄:預錄廣播僅可檢視)
    /// </summary>
    public class PreRecordVoiceSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public PreRecordVoiceSetting(string seatID) : base(seatID)
        {
           
        }

        /// <summary>
        /// 預錄廣播ID
        /// </summary>
        public string voice_id { get; set; }

        /// <summary>
        /// 命令種類 update
        /// </summary>
         public Enum.SqlCommand command { get; set; }
    }

    /// <summary>
    /// (6)廣播分時音量設定 (無須等待回應，強迫PA Server接受設定)
    /// 直接刷新pa_daypart_time/pa_daypart_olume兩張資料表
    /// </summary>
    public class DayPartTimeVolumeSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public DayPartTimeVolumeSetting(string seatID) : base(seatID)
        {  
        }
        /// <summary>
        /// 命令種類 update only
        /// </summary>
        public Enum.SqlCommand command { get; set; }
    }

    /// <summary>
    /// (7)群組設定 (無須傳送)
    /// </summary>
    public class GroupSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public GroupSetting(string seatID) : base(seatID)
        { 
        }

        /// <summary>
        /// 群組ID
        /// </summary>
        public string group_id { get; set; }
        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }
    }

    #endregion
}
