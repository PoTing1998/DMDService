using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject.CADClient.FromCMFT
{
    /// <summary>
    /// CMFT to TETRA，4.2.2.(1) 使用者登入
    /// </summary>
    public class Login 
    {
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "Login";

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 使用者角色
        /// </summary>
        public string Role { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.2.2.(2) 使用者登出
    /// </summary>
    public class Logout 
    {       
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "Logout";

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string User { get; set; }

    }

    /// <summary>
    /// CMFT to TETRA，4.2.2.(3) 控制項外觀參數傳遞
    /// </summary>
    public class SetDisplay 
    {        
        /// <summary>
        /// 訊息類別
        /// </summary>
        public string Type { get; } = "SetDisplay";

        /// <summary>
        /// 設定CAD Client的視窗左上角X軸座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 設定CAD Client的視窗左上角Y軸座標
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 設定CAD Client的視窗寬度
        /// </summary>
        public int W { get; set; }

        /// <summary>
        /// 設定CAD Client的視窗高度
        /// </summary>
        public int H { get; set; }
    }

}
