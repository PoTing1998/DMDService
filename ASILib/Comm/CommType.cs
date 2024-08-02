using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm
{
    /// <summary>
    /// Communication Type類舉類別
    /// </summary>
    public enum CommType : int
    {
        /// <summary>
        /// 連線成功
        /// </summary>
        ConnSuccessful,

        /// <summary>
        /// 連線失敗
        /// </summary>
        ConnFailure,

        /// <summary>
        /// 中斷連線
        /// </summary>
        Unconnect,

        /// <summary>
        /// 回傳失敗
        /// </summary>
        Failure,

        /// <summary>
        /// 傳送要求訊息
        /// </summary>
		Reqest,

        /// <summary>
        /// 訊息內容
        /// </summary>
		Context,

        /// <summary>
        /// 檢測通訊狀況
        /// </summary>
		CheckComm,

        /// <summary>
        /// 要求回傳狀況訊息
        /// </summary>
		AskStates,

        /// <summary>
        /// 回應待命狀態
        /// </summary>
		Ready,

        /// <summary>
        /// 回應狀態正常
        /// </summary>
		Ack,

        /// <summary>
        /// 回應狀態異常
        /// </summary>
		NAck,

        /// <summary>
        /// 系統停止運作
        /// </summary>
		Stop,

        /// <summary>
        /// 系統忙碌
        /// </summary>
		Busy,

        /// <summary>
        /// 訊息無法辨識
        /// </summary>
		UnKnown

    }
}
