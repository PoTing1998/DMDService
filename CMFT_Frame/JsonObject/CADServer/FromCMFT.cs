using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.CADServer.FromCMFT
{
    /// <summary>
    /// CMFT to TETRA，4.1.2.(1) 降級模式啟用/停用通知
    /// </summary>
    public class CMFTDegrade 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "CMFTDegrade";

        /// <summary>
        /// 狀態(啟用/停用 true/false)
        /// </summary>
        public string Enabled { get; set; }

        /// <summary>
        /// 目前CMFT server IP
        /// </summary>
        public string ServerIP { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.1.2.(2) 傳送文字訊息啟用/停用
    /// </summary>
    public class EnableTextMessage
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "EnableTextMessage";

        /// <summary>
        /// 狀態(啟用/停用 true/false)
        /// </summary>
        public string Enabled { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.1.2.(3) 傳送文字訊息
    /// </summary>
    public class SendTextMessage 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "SendTextMessage";

        /// <summary>
        /// 傳送者名稱，做為紀錄之用
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 接收者的無線電ISSI；範例:[75001,75002]
        /// </summary>
        public int[] Recipients { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Message { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.1.2.(4) 傳送列車PIDS即時訊息
    /// </summary>
    public class SendOnTrainPIDS
    {
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "SendOnTrainPIDS";

        /// <summary>
        /// 傳送者名稱，做為紀錄之用
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 列車的Rake ID；範例:["T01","T02"]
        /// </summary>
        public string[] RakeID { get; set; }

        /// <summary>
        /// 即時訊息內容，長度35個中文或70個英文字元
        /// </summary>
        public string Message { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.1.2.(5) 傳送列車預錄廣播命令
    /// </summary>
    public class SendOnTrainPA
    {
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "SendOnTrainPA";

        /// <summary>
        /// 傳送者名稱，做為紀錄之用
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 列車的Rake ID；範例:["T01","T02"]
        /// </summary>
        public string[] RakeID { get; set; }

        /// <summary>
        /// 列車預錄廣播ID
        /// </summary>
        public int PAID { get; set; }

        /// <summary>
        /// 列車預錄廣播方式(Audio/Text/Both)
        /// </summary>
        public string PAType { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.1.2.(6) PIDS字元黑名單設定
    /// </summary>
    public class CharacterBlacklist
    {       
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "CharacterBlacklist";

        /// <summary>
        /// 黑名單內容，範例: [堃,栢, 喆]
        /// </summary>
        public string[] Blacklist { get; set; }

    }

    #region CMFT to HMI

    /// <summary>
    /// 列車預錄廣播命令回應
    /// </summary>
    public class Res_SendOnTrainPA : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendOnTrainPA(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標列車 ["T01","T02"]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    /// <summary>
    /// 列車PIDS即時訊息命令回應
    /// </summary>
    public class Res_SendOnTrainPIDS : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendOnTrainPIDS(ASI.Wanda.CMFT.Enum.COMDevice messageFrom) : base(messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標列車 ["T01","T02"]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    #endregion
}
