using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.CMFT.DB.Models.CMFT
{
    public class cmft_account
    {
        /// <summary>  
        /// 使用者帳號        
        /// </summary> 
        [Key]
        public string account_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 使用者姓名        
        /// </summary> 
        public string account_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 使用者密碼        
        /// </summary> 
        public string account_password
        {
            get;
            set;
        }

        /// <summary>  
        /// 使用者單位        
        /// </summary> 
        public string account_department
        {
            get;
            set;
        }

        /// <summary>  
        /// 角色識別代碼        
        /// </summary> 
        public Guid role_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 是否啟用        
        /// </summary> 
        public bool is_enable
        {
            get;
            set;
        }

        /// <summary>  
        /// 剩餘次數        
        /// </summary> 
        /// <remarks>登入失敗剩餘次數</remarks>
        public int remain_count
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

    public class cmft_oper
    {
        /// <summary>  
        /// 操作識別代碼        
        /// </summary> 
        [Key]
        public string oper_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 畫面編號 
        /// </summary> 
        public string page_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 畫面名稱
        /// </summary> 
        public string page_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 功能編號       
        /// </summary> 
        public string func_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 功能名稱        
        /// </summary> 
        public string func_name
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

    public class cmft_role
    {
        /// <summary>  
        /// 角色識別代碼        
        /// </summary> 
        [Key]
        public Guid role_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 角色名稱        
        /// </summary> 
        public string role_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 描述        
        /// </summary> 
        public string description
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

    public class cmft_role_auth
    {
        /// <summary>  
        /// 角色識別代碼        
        /// </summary> 
        [Key]
        public Guid role_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 操作功能識別代碼        
        /// </summary>
        [Key]
        public string oper_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 是否可瀏覽        
        /// </summary> 
        public bool allow_browsing
        {
            get;
            set;
        }

        /// <summary>  
        /// 是否可設定        
        /// </summary> 
        public bool allow_setting
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

    /// <summary>
    /// 命令管理
    /// </summary>
    public class cmft_command
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
        /// HMI命令的訊息編碼
        /// </summary>
        [Key]
        public int msg_id { get; set; }

        /// <summary>  
        /// 通訊子系統編號。ex:PA、DMD、TETRA...
        /// </summary> 
        public string system_id
        {
            get;
            set;
        }

        /// <summary>  
        /// HMI傳至CMFT Server的JsonObject物件名稱        
        /// </summary> 
        [Key]
        public string json_object_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 命令對象車站        
        /// </summary> 
        [Key]
        public string station_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 預期子系統會回應的命令定義
        /// PA = ASI.Wanda.PA.Enum.PacketSurvey
        /// DMD = ASI.Wanda.CMFT.JsonObject.DMD.FromDMD的JsonObject物件名稱
        /// </summary> 
        public string response_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 是否執行成功。成功 = Y；失敗 = N    
        /// </summary> 
        public string is_success
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
