using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.PA.FromCMFT
{
    #region CMFT Server to HMI

    /// <summary>
    /// 連動內容傳送 Step1-PA 傳送回應
    /// </summary>
    public class Res_SendSyncPA : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendSyncPA(string seatID) : base(seatID)
        {
        }

        /// <summary>
        /// 車站ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 控制權取得成功與否
        /// </summary>
        public bool Result { get; set; }

    }

    /// <summary>
    /// 連動內容傳送 Step2-DMD 傳送回應
    /// </summary>
    public class Res_SendSyncDMD : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendSyncDMD(string seatID) : base(seatID)
        {
           
        }

        /// <summary>
        /// 即時訊息ID
        /// </summary>
        public string msg_id { get; set; }

        /// <summary>
        /// 回應車站
        /// </summary>
        public string station_id { get; set; }

        /// <summary>
        /// 執行結果
        /// </summary>
        public bool is_success { get; set; }

        /// <summary>
        /// 執行失敗的目標看板 [stationID]_[areaID]_[deviceID]
        /// </summary>
        public List<string> failed_target { get; set; }

    }

    /// <summary>
    /// (3)連動內容傳送 Step3-PIDS 傳送回應
    /// <summary>
    public class Res_SendSyncPIDS : ASI.Wanda.CMFT.JsonObject.Base
    {
        public Res_SendSyncPIDS(string seatID) : base(seatID)
        {

        }

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
    #endregion
}
