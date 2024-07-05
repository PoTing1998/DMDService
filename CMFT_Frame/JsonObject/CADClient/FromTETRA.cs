using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.CADClient.FromTETRA
{
    /// <summary>
    /// TETRA to CMFT，4.2.1.(1) 操作紀錄回報
    /// </summary>
    public class ClientOperation 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "ClientOperation";

        /// <summary>
        /// 來源系統，固定為CAD
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 操作紀錄時間，格式:“日期,時間,時區”範例:"2020-9-3,12:20:32.3,+8:00"
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 操作使用者名稱
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 操作功能：
        /// TETRA-Voice call
        /// TETRA-Emergency call
        /// TETRA-SDS
        /// OTCS-Instant PA
        /// OTCS-Prerecorded PA
        /// OTCS-Instant PIDS
        /// OTCS-Prerecorded PIDS
        /// OTCS-Cabin listening
        /// OTCS-PIC
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// 通話/發送對象，範例:[Target1, Target2]
        /// </summary>
        public string[] Target { get; set; }

        /// <summary>
        /// 發送內容
        /// </summary>
        public string Content { get; set; }

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
