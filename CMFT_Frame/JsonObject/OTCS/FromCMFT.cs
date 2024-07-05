using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.OTCS.FromCMFT
{    

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
        /// 執行失敗的目標列車Rake ID ["T01","T02"]
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
        /// 執行失敗的目標列車Rake ID ["T01","T02"]
        /// </summary>
        public List<string> failed_target { get; set; }
    }

    #endregion
}
