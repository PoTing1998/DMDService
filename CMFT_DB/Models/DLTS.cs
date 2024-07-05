using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.CMFT.DB.Models.DLTS
{
    public class dlts_seat_status
    {
        /// <summary>  
        /// 席位編號        
        /// </summary> 
        [Key]
        public string seat_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 話機號碼        
        /// </summary> 
        public string tel_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 當前通話狀態        
        /// </summary> 
        /// <remarks>0:閒置 4:待接聽 5:撥號中 6:通話中 10:通話保留</remarks>
        public int cur_call_status
        {
            get;
            set;
        }

        /// <summary>  
        /// 當前通話號碼        
        /// </summary> 
        /// <remarks>可有兩組5位數號碼</remarks>
        public string cur_call_number
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

    public class dlts_caller_display
    {
        /// <summary>  
        /// 席位上的話機號碼        
        /// </summary> 
        [Key]
        public string tel_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 當前通話號碼        
        /// </summary> 
        [Key]
        public string cur_call_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 當前通話狀態        
        /// </summary> 
        /// <remarks>0:閒置 4:待接聽 5:撥號中 6:通話中 9:話後處理 10:通話保留</remarks>
        public int cur_call_status
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

    public class dlts_caller_log
    {
        /// <summary>  
        /// 事件紀錄編號        
        /// </summary> 
        [Key]
        public Guid log_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 受話方直線電話號碼        
        /// </summary> 
        public string called_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 發話方直線電話號碼        
        /// </summary> 
        public string caller_number
        {
            get;
            set;
        }

        /// <summary>  
        /// 通話開始時間        
        /// </summary> 
        public DateTime start_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 通話結束時間        
        /// </summary> 
        public DateTime end_time
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
