using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.Sync.FromHMI
{
    #region 發送
    /// <summary>
    /// (1)連動內容傳送 Step1-PA
    /// <summary>
    public class SendSyncPA : ASI.Wanda.CMFT.JsonObject.Base 
    {
        public SendSyncPA(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 預錄廣播ID (folder_id)
        /// </summary>
        public List<string> voice_id { get; set; } = new List<string>();
        /// <summary>
        /// 廣播對象車站 (station_id)
        /// </summary>
        public List<string> target_station { get; set; } = new List<string>();

    }

    /// <summary>
    /// (2)連動內容傳送 Step2-DMD
    /// <summary>
    public class SendSyncDMD : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendSyncDMD(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 即時訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> target_du { get; set; } = new List<string>();


    }
    
    /// <summary>
    /// (3)連動內容傳送 Step3-PIDS
    /// <summary>
    public class SendSyncPIDS : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendSyncPIDS(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "SendSyncPIDS";

        /// <summary>
        /// 接收的列車編號
        /// </summary>
        public string trainID { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Message { get; set; }
    }

    #endregion
}
