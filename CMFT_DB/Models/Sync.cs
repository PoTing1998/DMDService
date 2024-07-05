using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.CMFT.DB.Models.Sync
{
    public class sync_linked_setting

    {
        /// <summary>  
        /// 車站廣播語音編號        
        /// </summary> 
        [Key]
        public string pa_folder_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 車站看板訊息編號        
        /// </summary> 
        public Guid dmd_message_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 列車看板訊息編號        
        /// </summary> 
        public Guid pids_message_id
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
