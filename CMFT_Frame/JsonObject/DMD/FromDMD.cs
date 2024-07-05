using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.DMD.FromDMD
{
    #region DMD主動告警/回報

    /// <summary>
    /// DMD to CMFT，4.1.1.(1)設備狀態改變通知
    /// </summary>
    public class EquipStatus : ASI.Wanda.CMFT.JsonObject.Base
    {
        public EquipStatus(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 設備代碼，sys_equip_status資料表equip_id欄位
        /// </summary>
        public string equip_id { get; set; }

        /// <summary>
        /// 狀態，正常:true；異常:false
        /// </summary>
        public bool status { get; set; }

        /// <summary>
        /// 相關資料庫名稱
        /// </summary>
        public string dbName1 { get; set; } = "sys_equip_status";
    }

    /// <summary>
    ///  DMD to CMFT，4.1.1.(2)列車營運模式
    /// </summary>
    public class OperationMode : ASI.Wanda.CMFT.JsonObject.Base
    {
        public OperationMode(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 車站代碼
        /// </summary>
        public string station_id { get; set; }

        /// <summary>
        /// 營運狀態
        /// </summary>
        public string mode { get; set; }
    }

    #endregion

    #region DMD Response

    /// <summary>
    /// 預錄訊息命令回應
    /// </summary>
    public class Res_SendPreRecordMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendPreRecordMessage(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 預錄訊息ID
        /// </summary>
        public List<string> msg_id { get; set; }

        /// <summary>
        /// 回應車站
        /// </summary>
        public string station_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    /// <summary>
    /// 即時訊息命令回應
    /// </summary>
    public class Res_SendInstantMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendInstantMessage(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 即時訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 回應車站
        /// </summary>
        public string station_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    /// <summary>
    /// 預錄訊息排程設定回應
    /// </summary>
    public class Res_ScheduleSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_ScheduleSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 排程ID
        /// </summary>
        public string sched_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }
    }

    /// <summary>
    /// 預錄訊息設定回應
    /// </summary>
    public class Res_PreRecordMessageSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_PreRecordMessageSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 預錄訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }
    }

    /// <summary>
    /// 列車訊息設定回應
    /// </summary>
    public class Res_TrainMessageSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_TrainMessageSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 列車訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }
    }

    /// <summary>
    /// 電源設定回應
    /// </summary>
    public class Res_PowerTimeSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_PowerTimeSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }
    }

    /// <summary>
    /// 群組設定回應
    /// </summary>
    public class Res_GroupSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_GroupSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 群組ID
        /// </summary>
        public string group_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }
    }

    /// <summary>
    /// 刪除目前播放內容回應
    /// </summary>
    public class Res_DeletePlayingMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_DeletePlayingMessage(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// playlist_id  :目前播放訊息ID
        /// </summary>
        public List<string> playlist_id { get; set; } = new List<string>();

        /// <summary>
        /// 命令種類 delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    #endregion
}

