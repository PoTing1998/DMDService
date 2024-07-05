using System.Collections.Generic;

namespace ASI.Wanda.CMFT.JsonObject.DMD.FromHMI
{
    #region 功能

    /// <summary>
    /// (1)刪除目前播放內容
    /// </summary>
    public class DeletePlayingMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public DeletePlayingMessage(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// playlist_id  :目前播放訊息ID
        /// </summary>
        public List<string> playlist_id { get; set; } = new List<string>();
        /// <summary>
        /// 命令種類 delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }
        public string dbName1 { get; set; } = "dmd_playlist";
     
    }

    /// <summary>
    /// (2)預錄訊息傳送
    /// </summary>
    public class SendPreRecordMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendPreRecordMessage(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// msg_id   :預錄訊息ID
        /// target_du:目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> msg_id { get; set; } = new List<string>();
        public List<string> target_du { get; set; } = new List<string>();

        public string dbName1 { get; set; } = "dmd_pre_record_message";
        public string dbName2 { get; set; } = "dmd_target";
    }

    /// <summary>
    /// (3)即時訊息傳送
    /// </summary>
    public class SendInstantMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendInstantMessage(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 即時訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> target_du { get; set; } = new List<string>();

        public string dbName1 { get; set; } = "dmd_instant_message";
        public string dbName2 { get; set; } = "dmd_target";
    }

    /// <summary>
    /// (4)訊息排程
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

        public string dbName1 { get; set; } = "dmd_schedule";
        public string dbName2 { get; set; } = "dmd_schedule_playlist";
    }

    #endregion

    #region 設定

    /// <summary>
    /// (5)預錄訊息設定
    /// </summary>
    public class PreRecordMessageSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public PreRecordMessageSetting(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 預錄訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }

        public string dbName1 { get; set; } = "dmd_pre_record_message";
    }

    /// <summary>
    /// (6)列車訊息設定
    /// </summary>
    public class TrainMessageSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public TrainMessageSetting(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 列車訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }

        public string dbName1 { get; set; } = "dmd_train_message";
    }

    /// <summary>
    /// (7)電源設定
    /// </summary>
    public class PowerTimeSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public PowerTimeSetting(string seatID) : base(seatID)
        {
        }
        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }

        public string dbName1 { get; set; } = "dmd_power_setting";
    }

    /// <summary>
    /// (8)群組設定
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

        public string dbName1 { get; set; } = "dmd_group";
        public string dbName2 { get; set; } = "dmd_group_target";
    }

    #endregion
}