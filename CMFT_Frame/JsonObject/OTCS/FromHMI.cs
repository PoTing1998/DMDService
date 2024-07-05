using System.Collections.Generic;

namespace ASI.Wanda.CMFT.JsonObject.OTCS.FromHMI
{
    /// <summary>
    /// HMI to CMFT Server，4.1.2.3(1) 列車PIDS即時訊息
    /// </summary>
    public class SendOnTrainPIDS : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendOnTrainPIDS(string seatID) : base(seatID)
        {
        }

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
    /// HMI to CMFT Server，4.1.2.3(2) 列車預錄廣播
    /// </summary>
    public class SendOnTrainPA : ASI.Wanda.CMFT.JsonObject.Base
    {
        public SendOnTrainPA(string seatID) : base(seatID)
        {
        }

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
}