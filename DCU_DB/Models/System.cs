using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DCU.DB.Models.System
{
    public class sys_alarm
    {
        /// <summary>  
        /// 告警編號        
        /// </summary> 
        [Key]
        public Guid alarm_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 發生時間        
        /// </summary> 
        public string alarm_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 解除時間        
        /// </summary> 
        public string release_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 確認時間        
        /// </summary> 
        public string check_time
        {
            get;
            set;
        }

        /// <summary>  
        /// 確認人員帳號        
        /// </summary> 
        public string check_account_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 確認人員姓名        
        /// </summary> 
        public string check_account_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號        
        /// </summary> 
        public string equip_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 告警編號        
        /// </summary> 
        public string alarm_list_id
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
    public class sys_alarm_list
    {
        /// <summary>  
        /// 告警編號        
        /// </summary> 
        [Key]
        public string alarm_list_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 警告來源        
        /// </summary> 
        public string alarm_source
        {
            get;
            set;
        }

        /// <summary>  
        /// 告警等級        
        /// </summary> 
        public int alarm_level
        {
            get;
            set;
        }

        /// <summary>  
        /// 說明(中文)        
        /// </summary> 
        public string description
        {
            get;
            set;
        }

        /// <summary>  
        /// 說明(英文)        
        /// </summary> 
        public string description_eng
        {
            get;
            set;
        }

        /// <summary>  
        /// 可能原因(中文)        
        /// </summary> 
        public string possible_reason
        {
            get;
            set;
        }

        /// <summary>  
        /// 可能原因(英文)        
        /// </summary> 
        public string possible_reason_eng
        {
            get;
            set;
        }

        /// <summary>  
        /// 告警音量        
        /// </summary> 
        public int alarm_volume
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
    public class sys_config
    {
        /// <summary>  
        /// 系統參數名稱        
        /// </summary> 
        [Key]
        public string config_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 系統參數值        
        /// </summary> 
        public string config_value
        {
            get;
            set;
        }

        /// <summary>  
        /// 系統參數說明        
        /// </summary> 
        public string config_description
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
    public class sys_equip_status
    {
        /// <summary>  
        /// 設備編號        
        /// </summary> 
        [Key]
        public string equip_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 地區編號        
        /// </summary> 
        public string region_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 地區名稱        
        /// </summary> 
        public string region_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 地點名稱        
        /// </summary> 
        public string place_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 區域名稱        
        /// </summary> 
        public string area_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 系統編號        
        /// </summary> 
        public string system_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 系統名稱        
        /// </summary> 
        public string system_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備類型        
        /// </summary> 
        public string equip_type
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備狀態        
        /// </summary> 
        public bool equip_status
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
    public class sys_oper_log
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
        /// 使用者帳號        
        /// </summary> 
        public string account_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 使用者姓名        
        /// </summary> 
        /// <remarks>必須不依賴ID,萬一腳色被刪除仍可查找</remarks>
        public string account_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 操作識別代碼        
        /// </summary> 
        public string oper_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 事件紀錄內容        
        /// </summary> 
        public string log_content
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
    public class sys_oper_log_history
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
        /// 操作人員        
        /// </summary> 
        public string account_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 使用者姓名        
        /// </summary> 
        /// <remarks>必須不依賴ID,萬一腳色被刪除仍可查找</remarks>
        public string account_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 操作識別代碼        
        /// </summary> 
        public string oper_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 事件紀錄內容        
        /// </summary> 
        public string log_content
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
    public class sys_region
    {
        /// <summary>  
        /// 區域編號        
        /// </summary> 
        [Key]
        public string region_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 區域名稱        
        /// </summary> 
        public string region_name
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
    public class sys_seat
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
        /// 對應PA的席位編號
        /// </summary> 
        public int pa_seat_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 席位名稱        
        /// </summary> 
        public string seat_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 席位人員        
        /// </summary> 
        public string seat_stuff
        {
            get;
            set;
        }

        /// <summary>  
        /// 地區編號        
        /// </summary> 
        public string region_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 地區名稱        
        /// </summary> 
        public string region_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 地點名稱        
        /// </summary> 
        public string place_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 區域名稱        
        /// </summary> 
        public string area_name
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
    public class sys_seat_equip
    {
        /// <summary>  
        /// 席位編號        
        /// </summary> 
        [Key]
        public string steat_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號-cmft        
        /// </summary> 
        public string equip_id_cmft
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號-dlts        
        /// </summary> 
        public string equip_id_dlts
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號-epabx        
        /// </summary> 
        public string equip_id_epabx
        {
            get;
            set;
        }

        /// <summary>  
        /// 設備編號-tetra        
        /// </summary> 
        public string equip_id_tetra
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
    public class sys_station
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
        /// 車站名稱        
        /// </summary> 
        public string station_name
        {
            get;
            set;
        }

        /// <summary>  
        /// 車站英文名稱        
        /// </summary> 
        public string station_name_eng
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
    public class sys_station_area
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
        /// 車站區域編號        
        /// </summary> 
        [Key]
        public string area_id
        {
            get;
            set;
        }

        /// <summary>  
        /// 車站區域名稱        
        /// </summary> 
        public string area_name
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
