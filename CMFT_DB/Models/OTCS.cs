using Microsoft.Extensions.Logging;
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
        /// 第一節車廂編號      
        /// </summary> 
        public string car_mc1
        {
            get;
            set;
        }

        /// <summary>  
        /// 第二節車廂編號      
        /// </summary> 
        public string car_m1
        {
            get;
            set;
        }

        /// <summary>  
        /// 第三節車廂編號      
        /// </summary> 
        public string car_m2
        {
            get;
            set;
        }

        /// <summary>  
        /// 第四節車廂編號      
        /// </summary> 
        public string car_mc2
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

    public class otcs_equip_status
    {
        /// <summary>
        /// 列車通訊設備編號
        /// <summary>
        [Key]
        public string equip_id
        {
            get;
            set;
        }

        /// <summary>
        /// 列車通訊設備名稱
        /// <summary>
        public string equip_name
        {
            get;
            set;
        }

        /// <summary>
        /// 列車編號
        /// <summary>
        public string train_id
        {
            get;
            set;
        }

        /// <summary>
        /// 列車車廂編號
        /// <summary>
        public string car_type
        {
            get;
            set;
        }

        /// <summary>
        /// 設備狀態
        /// <summary>
        public int equip_status
        {
            get;
            set;
        }

        /// <summary>
        /// 備註
        /// <summary>
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

    public class otcs_event_list 
    {
        /// <summary>
        /// 列車事件編號
        ///<summary>   
        [Key]
        public string event_list_id 
        { 
            get;
            set;
        }

        /// <summary>
        /// 列車事件來源
        /// <summary>
        public string event_source
        {
            get;
            set;
        }

        /// <summary>
        /// 說明(中文)     
        /// <summary>
        public string description
        {
            get;
            set;
        }

        /// <summary>說明(英文)     
        /// <summary>
        public string description_eng
        {
            get;
            set;
        }

        /// <summary>
        /// 可能原因(中文) 
        /// <summary>    
        public string possible_reason
        {
            get;
            set;
        }

        /// <summary>
        /// 可能原因(英文) 
        /// <summary>    
        public string possible_reason_eng
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

    public class otcs_event
    {
        /// <summary>
        /// 事件編號
        /// <summary>
        [Key]
        public Guid event_id
        {
            get;
            set;
        }

        /// <summary>
        /// 發生時間
        /// <summary>
        public string event_time
        {
            get;
            set;
        }

        /// <summary>
        ///  解除時間
        /// <summary>
        public string release_time
        {
            get;
            set;
        }

        /// <summary>
        /// 確認時間
        /// <summary>
        public string check_time
        {
            get;
            set;
        }

        /// <summary>
        /// 確認人員ID
        /// <summary>
        public string check_account_id
        {
            get;
            set;
        }

        /// <summary>
        /// 確認人員名稱
        /// <summary>
        public string check_account_name
        {
            get;
            set;
        }

        /// <summary>
        /// 設備編號
        /// <summary>
        public string equip_id
        {
            get;
            set;
        }

        /// <summary>
        /// 列車事件編號
        /// <summary>
        public string event_list_id
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
