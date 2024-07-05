using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT.JsonObject
{
    public class Base
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="seatID">訊息發送來源端(席位編號)</param>
        public Base(string seatID)
        {
            SeatID = seatID;
        }

        /// <summary>
        /// 訊息發送來源端(席位編號)
        /// </summary>
        public string SeatID;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="messageFrom">訊息發送來源端(參考廣播COM相關設備定義)</param>
        public Base(ASI.Wanda.CMFT.Enum.COMDevice messageFrom)
        {
            MessageFrom = messageFrom;
        }

        /// <summary>
        /// 訊息發送來源端(參考廣播COM相關設備定義)
        /// </summary>
        public ASI.Wanda.CMFT.Enum.COMDevice MessageFrom;

        /// <summary>
        /// 取得包含namespace的完整JsonObject名稱，例如ASI.Wanda.CMFT.JsonObject.Base
        /// </summary>
        public string JsonObjectName 
        { 
            get { return this.ToString(); }                
        }

    }
}
