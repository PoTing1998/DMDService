using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.DMD.DB.Models
{
    public class dmd_group
    {
        /// <summary>  
        /// 群組編號        
        /// </summary> 
        [Key]
        public Guid group_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 群組名稱        
        /// </summary> 
        public string group_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 群組說明        
        /// </summary> 
        public string group_description
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_group_target
    {
        /// <summary>  
        /// 群組編號        
        /// </summary> 
        [Key]
        public Guid group_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 裝設車站編號        
        /// </summary> 
        [Key]
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 裝設車站區域編號        
        /// </summary> 
        [Key]
        public string area_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號        
        /// </summary> 
        [Key]
        public string device_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_instant_message
    {
        /// <summary>  
        /// 訊息編號        
        /// </summary> 
        [Key]
        public Guid message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息類別        
        /// </summary> 
        /// <remarks> 1: 一般;  2:緊急  3: 超時</remarks>
        public int message_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 優先等級        
        /// </summary> 
        /// <remarks>1~5</remarks>
        public int message_priority
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動方式        
        /// </summary> 
        public int move_mode
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動速度        
        /// </summary> 
        /// <remarks>1~5</remarks>
        public int move_speed
        {
            get;
            set;
        }

        /// <summary>  
        /// 暫停時間        
        /// </summary> 
        public string Interval
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文訊息內容        
        /// </summary> 
        public string message_content
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文字型        
        /// </summary> 
        /// <remarks>標楷體, 新細明體 , 微軟正黑體</remarks>
        public string font_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文大小        
        /// </summary> 
        /// <remarks>1:小; 2:中; 3:大</remarks>
        public int font_size
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文顏色        
        /// </summary> 
        /// <remarks>紅,綠,藍,黃</remarks>
        public string font_color
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文訊息內容        
        /// </summary> 
        public string message_content_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文字型        
        /// </summary> 
        /// <remarks>Arial, Calibri , TimesNewRoman</remarks>
        public string font_type_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文大小        
        /// </summary> 
        /// <remarks>1:小; 2:中; 3:大</remarks>
        public int font_size_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文顏色        
        /// </summary> 
        /// <remarks>紅,綠,藍,黃</remarks>
        public string font_color_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_schedule_playlist
    {
        /// <summary>  
        /// 排程編號        
        /// </summary> 
        [Key]
        public Guid schedule_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息編號        
        /// </summary> 
        [Key]
        public Guid message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 車站編號        
        /// </summary> 
        [Key]
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 點矩陣設備編號        
        /// </summary> 
        [Key]
        public string device_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 發送時間        
        /// </summary> 
        public DateTime sned_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_schedule
    {
        /// <summary>  
        /// 排程編號        
        /// </summary> 
        [Key]
        public Guid schedule_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 排程名稱        
        /// </summary> 
        public string schedule_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 啟用狀態        
        /// </summary> 
        public bool is_enable
        {
            get;
            set;
        }

        /// <summary>  
        /// 啟用日期        
        /// </summary> 
        public DateTime start_date
        {
            get;
            set;
        }

        /// <summary>  
        /// 結束日期        
        /// </summary> 
        public DateTime end_date
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_target
    {
        /// <summary>  
        /// 裝設車站編號        
        /// </summary> 
        [Key]
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 裝設車站區域編號        
        /// </summary> 
        [Key]
        public string area_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號        
        /// </summary> 
        [Key]
        public string device_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備名稱        
        /// </summary> 
        public string device_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備狀態        
        /// </summary> 
        public int device_status
        {
            get;
            set;
        }

        /// <summary>  
        /// 備註        
        /// </summary> 
        public string remark
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_playlist
    {
        /// <summary>  
        /// 播放清單編號     
        /// </summary> 
        [Key]
        public Guid playlist_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 裝設車站編號        
        /// </summary> 
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 裝設車站區域編號        
        /// </summary> 
        public string area_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號      
        /// </summary> 
        public string device_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息編號        
        /// </summary> 
        public Guid message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息類型        
        /// </summary> 
        public int message_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 發送時間        
        /// </summary> 
        public string send_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_power_setting
    {
        /// <summary>  
        /// 車站編號        
        /// </summary> 
        [Key]
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 節能模式        
        /// </summary> 
        /// <remarks>on/off</remarks>
        public string eco_mode
        {
            get;
            set;
        }

        /// <summary>  
        /// 進入節能模式(秒)        
        /// </summary> 
        public int eco_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 不休眠日期(跨年)        
        /// </summary> 
        public string not_eco_day
        {
            get;
            set;
        }

        /// <summary>  
        /// 自動啟動時間        
        /// </summary> 
        public string auto_play_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 自動關閉時間        
        /// </summary> 
        public string auto_eco_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_pre_record_message
    {
        /// <summary>  
        /// 訊息編號        
        /// </summary> 
        [Key]
        public Guid message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息名稱        
        /// </summary> 
        public string message_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息類別        
        /// </summary> 
        public int message_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 優先等級        
        /// </summary> 
        public int message_priority
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動方式        
        /// </summary> 
        public int move_mode
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動速度        
        /// </summary> 
        public int move_speed
        {
            get;
            set;
        }

        /// <summary>  
        /// 暫停時間        
        /// </summary> 
        public string Interval
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文訊息內容        
        /// </summary> 
        public string message_content
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文字型        
        /// </summary> 
        public string font_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文大小        
        /// </summary> 
        public int font_size
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文顏色        
        /// </summary> 
        public string font_color
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文訊息內容        
        /// </summary> 
        public string message_content_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文字型        
        /// </summary> 
        public string font_type_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文大小        
        /// </summary> 
        public int font_size_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文顏色        
        /// </summary> 
        public string font_color_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
    public class dmd_train_message
    {
        /// <summary>  
        /// 訊息編號        
        /// </summary> 
        [Key]
        public string message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息類別        
        /// </summary> 
        /// <remarks> 1: 一般;  2:緊急  3: 超時</remarks>
        public int message_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 訊息子類別        
        /// </summary> 
        public int message_subtype
        {
            get;
            set;
        }

        /// <summary>  
        /// 優先等級        
        /// </summary> 
        /// <remarks>1~5</remarks>
        public int message_priority
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動方式        
        /// </summary> 
        public int move_mode
        {
            get;
            set;
        }

        /// <summary>  
        /// 移動速度        
        /// </summary> 
        /// <remarks>1~5</remarks>
        public int move_speed
        {
            get;
            set;
        }

        /// <summary>  
        /// 顯示次數        
        /// </summary> 
        public int display_times
        {
            get;
            set;
        }

        /// <summary>  
        /// 倒數顯示間格        
        /// </summary> 
        public int countdown_display_Interval
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文訊息內容        
        /// </summary> 
        public string message_content
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文字型        
        /// </summary> 
        /// <remarks>標楷體, 新細明體 , 微軟正黑體</remarks>
        public string font_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文大小        
        /// </summary> 
        /// <remarks>1:小; 2:中; 3:大</remarks>
        public int font_size
        {
            get;
            set;
        }

        /// <summary>  
        /// 中文顏色        
        /// </summary> 
        /// <remarks>紅,綠,藍,黃</remarks>
        public string font_color
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文訊息內容        
        /// </summary> 
        public string message_content_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文字型        
        /// </summary> 
        /// <remarks>Arial, Calibri , TimesNewRoman</remarks>
        public string font_type_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文大小        
        /// </summary> 
        /// <remarks>1:小; 2:中; 3:大</remarks>
        public int font_size_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 英文顏色        
        /// </summary> 
        /// <remarks>紅,綠,藍,黃</remarks>
        public string font_color_en
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增人員        
        /// </summary> 
        public string ins_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料新增時間        
        /// </summary> 
        public DateTime ins_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改人員        
        /// </summary> 
        public string upd_user
        {
            get;
            set;
        }

        /// <summary>  
        /// 資料修改時間        
        /// </summary> 
        public DateTime upd_time
        {
            get;
            set;
        }

    }
}
