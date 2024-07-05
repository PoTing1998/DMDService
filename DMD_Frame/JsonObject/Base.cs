using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.JsonObject
{
    public class Base
    {
        /// <summary>
        /// 取得包含namespace的完整JsonObject名稱，例如ASI.Wanda.CMFT.JsonObject.Base
        /// </summary>
        public string JsonObjectName 
        { 
            get { return this.ToString(); }                
        }

        // 訊息發送來源
        public ASI.Wanda.DMD.Enum.Station  station;
        public Base(ASI.Wanda.DMD.Enum.Station station)
        {
            this.station = station;
        }

    }
}
