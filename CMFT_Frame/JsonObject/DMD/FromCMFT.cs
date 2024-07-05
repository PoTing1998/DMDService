using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.DMD.FromCMFT
{
    #region CMFT to DMD

    /// <summary>
    /// CMFT to DMD，4.1.2.(1) 降級模式啟用/停用通知
    /// </summary>
    public class DownGrade : ASI.Wanda.CMFT.JsonObject.Base
    {
        /// <summary>
        /// 
        /// CMFT to DMD，4.1.2.(1) 降級模式啟用/停用通知
        /// </summary>
        /// <param name="messageFrom"></param>
        public DownGrade(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 狀態(啟用/停用 on/off)
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 目前CMFT server IP
        /// </summary>
        public string server_ip { get; set; }

    }
       
    /// <summary>
    /// CMFT to DMD，4.1.2.(2) 預錄訊息命令
    /// </summary>
    public class SendPreRecordMessage : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendPreRecordMessage(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 預錄訊息ID
        /// </summary>
        public List<string> msg_id { get; set; } = new List<string>();

        /// <summary>
        /// 目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> target_du { get; set; } = new List<string>();

        public string dbName1 { get; set; } = "dmd_pre_record_message";
        public string dbName2 { get; set; } = "dmd_target";
    }

    /// <summary>
    /// CMFT to DMD，4.1.2.(3) 即時訊息命令
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
    /// CMFT to DMD，4.1.2.(4) 預錄訊息排程
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

    /// <summary>
    /// CMFT to DMD，4.1.2.(5) 預錄訊息設定
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
    /// CMFT to DMD，4.1.2.(6) 列車訊息設定
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
    /// CMFT to DMD，4.1.2.(7) 電源設定
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
    /// CMFT to DMD，4.1.2.(8) 群組設定
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

    /// <summary>
    /// CMFT to DMD，4.1.2.(9) 參數設定
    /// </summary>
    public class ParameterSetting : ASI.Wanda.CMFT.JsonObject.Base
    {
        public ParameterSetting(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 命令種類 add/update/delete
        /// </summary>
        public Enum.SqlCommand command { get; set; }

        public string dbName1 { get; set; } = "";
    }

    /// <summary>
    /// 刪除目前播放內容
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

    #endregion

    #region CMFT to HMI

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
