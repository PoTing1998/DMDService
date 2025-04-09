using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.CMFT.DB.Models.OTCS
{
    public class otcs_pre_record_message
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

    public class otcs_train
    {
        /// <summary>  
        /// 列車編號        
        /// </summary> 
        [Key]
        public string train_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車營運車號        
        /// </summary> 
        public string train_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車營運狀態        
        /// </summary> 
        public bool operation_status
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車位置        
        /// </summary> 
        public string train_location
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車所在軌道        
        /// </summary> 
        public string train_track
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車方向        
        /// </summary> 
        public string train_direction
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
