using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DCU.DB.Models.DU
{
    public class du_list
    {
        [Key]
        public string equip_id
        {
            get;
            set;
        }
        [Key]
        public string panel_id
        {
            get;
            set;
        }
        public string du_id
        {
            get;
            set;
        }
        public bool is_back
        {
            get;
            set;
        }
        public int status
        {
            get;
            set;
        }
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

    public class du_alarm_list
    {
        [Key]
      public Guid alrm_id
        {
            get;
            set;
        }

        public string problem_situation
        {
            get;
            set;
        }
        public string error_code
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

    public class du_alarm  
    { 
        public int du_id
        {
            get;
            set;
        }

       [Key]
        public int alrm_id
        {
            get;
            set;
        }
        public DateTime alrm_time
        {
            get;
            set;
        }

        public string du_station
        {
            get;
            set;
        }
    }
    public class du_message_setup
    {
        [Key]
        public Guid message_id
        {
            get;set;
        }
        [Key]
        public string panel_id
        {
            get;
            set;
        }
        [Key]
        public string station_id
        {
            get;
            set;
        }
        [Key]
        public string area_id
        {
            get;
            set;
        }
        [Key]
        public string device_id
        {
            get;
            set;
        }
        public string text_size
        {
            get;
            set;
        }
        public string text_font
        {
            get;
            set;
        }
        public string text_color
        {
            get;
            set;
        }
        public int priority
        {
            get;
            set;
        }
        public string move_type
        {
            get;
            set;
        }
        public int speed
        {
            get;
            set;
        }
        public int stay_time
        {
            get;
            set;
        }
        public string content
        {
            get;
            set;
        }
        public int windowdisplaymode
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
    public class layout_type
    {
        /// <summary>
        /// 面板指令左側
        /// </summary>
        [Key]
        public string windowactioncodeleft
        {
            get;
            set;
        }
        /// <summary>
        /// 左側顏色
        /// </summary>
        public string left_photo_color
        {
            get;
            set;
        }
        /// <summary>
        /// 左側圖片索引
        /// </summary>
        public int left_photo_index
        {
            get;
            set;
        }
        /// <summary>
        /// 面板指令
        /// </summary>
        [Key]
        public string windowactioncoderight
        {
            get;
            set;
        }
        /// <summary>
        /// 右側時間區塊顯示
        /// </summary>
        public bool right_time_onoff
        {
            get;
            set;
        }
        /// <summary>
        /// 右側時間顏色
        /// </summary>
        public string right_time_color
        {
            get;
            set;
        }
        /// <summary>
        /// 右側時間開始
        /// </summary>
        public int start_time
        {
            get;
            set;
        }
        /// <summary>
        /// 右側結束時間
        /// </summary>
        public int end_time
        {
            get;
            set;
        }
        /// <summary>
        /// 面板版型
        /// </summary>
        [Key]
        public string windowdisplaymode
        {
            get;
            set;
        }
        public int rows 
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 列車訊息格式
    /// </summary>
    public class train_location_parameters_setup
    {
        public int train_type //command
        {
            get;
            set;
        }
        public string dynamic_text_command //前兩站的閃爍或恆亮命令
        {
            get;
            set;
        }
        public string starting_station_color //顏色設定
        {
            get;
            set;
        }
        public string starting_station_name //名稱
        {
            get;
            set;
        }
        public string image_dynamic_command //command
        {
            get;
            set;
        }
        public int image_start_index //開始的圖片順序
        {
            get;
            set;
        }
        public int image_change_quantity //更動的圖片數量
        {
            get;
            set;
        }
        public string image_color //圖片顏色
        {
            get;
            set;
        }
        public string prev_dynamic_text_command //前一站的 閃爍或恆亮命令
        {
            get;
            set;
        }
        public string prev_station_color //前一站的 顏色
        {
            get;
            set;
        }
        public string prev_station_name //前一站的 名稱
        {
            get;
            set;
        }
        public string image_dynamic_command_prev // command
        {
            get;
            set;
        }
        public int image_start_index_prev // 前一站 開始的圖片順序
        {
            get;
            set;
        }
        public int image_change_quantity_prev // 前一站 更動的圖片數量
        {
            get;
            set;
        }
        public string image_color_prev //前一站 顏色
        {
            get;
            set;
        }
        public string dynamic_text_command_current //本站的 閃爍或恆亮命令
        {
            get;
            set;
        }
        public string current_station_color //本站顏色
        {
            get;
            set;
        }
        public string current_station_name //本站名稱
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 操作人員可操作面板   
    /// </summary>
    public class user_control_panel
    {
        [Key]
        public int panel_id
        {
            get;set;   
        }
        public int row
        {
            get;set;
        }
        public int WindowDisplayMode //版型設定
        {
            get;set;
        }
        public string WindowActionCodeLeft   
        {
            get;set;
        }
        public string RightPhotoColor  
        {
            get;set; 
        }
        public int RightPhotoIndex
        {
            get;set;
        }
        public string WindowActionCodeRight
        {
            get;set;
        }
        public bool RightTimeOnOff
        {
            get;set;
        }
        public string RightTimeColor
        {
            get;set;
        }
        public string ins_user
        {
            get;set;
        }
        public DateTime ins_time
        {
            get;set;
        }
        public string upd_user
        { 
            get;set;
        }
        public DateTime upd_time
        {
            get;set;
        }
    }

}
