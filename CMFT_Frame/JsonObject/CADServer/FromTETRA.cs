using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.CADServer.FromTETRA
{
    /// <summary>
    /// TETRA to CMFT，4.1.1.(1) CAD告警
    /// </summary>
    public class CadAlarm 
    {       
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "CadAlarm";

        /// <summary>
        /// 告警種類
        /// </summary>
        public string Alarm { get; set; }

        /// <summary>
        /// 告警時間 格式: “日期,時間,時區”範例:"2020-9-3,12:20:32.3,+8:00"
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 告警發生/告警結束 true/false
        /// </summary>
        public string Raised { get; set; }

    }

    /// <summary>
    /// TETRA to CMFT，4.1.1.(2) 列車告警
    /// </summary>
    public class TrainAlarm 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "TrainAlarm";

        /// <summary>
        /// 告警時間 格式: “日期,時間,時區”範例:"2020-9-3,12:20:32.3,+8:00"
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 列車的Rake ID
        /// </summary>
        public string RakeID { get; set; }

        /// <summary>
        /// 列車的節數
        /// </summary>
        //public string NumOfCars { get; set; }

        /// <summary>
        /// 每節車廂的Car ID，範例: [101,102,103,104,105]
        /// </summary>
        //public string Cars { get; set; }

        /// <summary>
        /// 列車告警種類
        /// </summary>
        public string Alarm { get; set; }

    }

    /// <summary>
    /// TETRA to CMFT，TETRA回應Response
    /// </summary>
    public class Response 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "Response";

        /// <summary>
        /// 執行結果代碼
        /// 0:OK
        /// 400:Bad Request	訊息內容錯誤
        /// 401:Unauthorized 未授權使用此功能
        /// 403:Forbidden	功能呼叫被拒絕
        /// 404:Not Found 未支援的訊息類別
        /// 408:Request Timeout	處理時間逾時
        /// 500:Internal Error	內部錯誤
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 執行結果文字描述
        /// </summary>
        public string Description { get; set; }

    }
}
