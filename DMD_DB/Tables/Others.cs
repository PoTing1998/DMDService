using ASI.Wanda.DMD.DB.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.DB.Models.Others
{
    public  class lineConf: Table<line_conf>
    {
        #region Methods
        #endregion
    }
    public class lineOperation : Table<line_operation>
    {
        #region Methods
        #endregion
    }

    public  class platformConf : Table<platform_conf>
    {
        #region Methods
        #endregion
    }

    public class stationConf :Table<station_conf>
    {
        #region Methods
        /// <summary>
        /// 取得當站的ip 進行比對
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        static public string GetStationID(string stationID)
        {
            string stationName = SelectAll().First(x => x.station_id == stationID).dcu_ip;
            return stationName;
        }
        #endregion
    }
    public class trainLocation : Table<train_location>
    {
        #region Methods
        #endregion
    }
}
