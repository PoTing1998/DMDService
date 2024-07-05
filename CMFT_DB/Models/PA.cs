using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.CMFT.DB.Models.PA
{
    public class pa_daypart_time
    {
        /// <summary>  
        /// 分時時間序號        
        /// </summary> 
        [Key]
        public Guid daypart_time_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 車站編號        
        /// </summary> 
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 星期        
        /// </summary> 
        public int week_day
        {
            get;
            set;
        }

        /// <summary>  
        /// 時段起始時間        
        /// </summary> 
        public string start_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 時段結束時間        
        /// </summary> 
        public string end_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 時段        
        /// </summary> 
        public int daypart
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
    public class pa_daypart_volume
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
        /// 時段        
        /// </summary> 
        [Key]
        public int daypart
        {
            get;
            set;
        }

        /// <summary>  
        /// 音量        
        /// </summary> 
        public int volume
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
    public class pa_group
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
    public class pa_group_target
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
    public class pa_pre_record_voice
    {
        /// <summary>  
        /// 語音編號        
        /// </summary> 
        [Key]
        public string folder_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 語音類別        
        /// </summary> 
        /// <remarks> 1: 一般;  2:緊急</remarks>
        public int voice_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 優先等級        
        /// </summary> 
        /// <remarks>1~5</remarks>
        public int voice_priority
        {
            get;
            set;
        }

        /// <summary>  
        /// 語音名稱        
        /// </summary> 
        public string voice_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 語音版本(日期)        
        /// </summary> 
        public string version
        {
            get;
            set;
        }

        /// <summary>  
        /// 啟用狀態        
        /// </summary> 
        public bool status
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔類型        
        /// </summary> 
        public string file_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔名稱        
        /// </summary> 
        public string file_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 語音內容        
        /// </summary> 
        public string voice_content
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔含有國語        
        /// </summary> 
        public bool include_chinese
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔含有英語        
        /// </summary> 
        public bool include_english
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔含有台語        
        /// </summary> 
        public bool include_taiwanese
        {
            get;
            set;
        }

        /// <summary>  
        /// 音檔含有客語        
        /// </summary> 
        public bool include_hakka
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
    public class pa_schedule
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
    public class pa_schedule_playlist
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
        /// 語音編號        
        /// </summary> 
        [Key]
        public string folder_id
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
        /// 區域編號        
        /// </summary> 
        [Key]
        public string area_id
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
    public class pa_target
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
}
