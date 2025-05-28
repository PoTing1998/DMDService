using ASI.Wanda.DMD.DB.Models.Train;
using ASI.Wanda.DMD.DB.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.DB.Tables.Train
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

    public class ocsData : Table<OCS_Data>
    {
        #region Methods
        static public void updatePlatform_ID(int platform_id)
        {
            string whereString = string.Format("where platform_id = '{0}'   ", platform_id);
            Update(whereString);
        }
      

        #endregion
    }

    public class trainMessage : Table<train_message>
    {
        public static void DeleteTrainMSG(string type)
        {
            Delete(type);
        }
        #region Methods
        static public void InsertTrain_MSG(int platform_id ,int arrive_time1 ,int depart_time1 ,int destination1 ,int arrive_time2 ,int depart_time2 ,int destination2)
        {
            Insert(
                arrive_time1,
                depart_time1,
                destination1,
                arrive_time2,
                depart_time2,
                destination2
                );
        }

        static public void UpdateTrain_MSG(int strat_address )
        {
            string whereString = string.Format("where start_address = '{0}'   ", strat_address );
            Update(whereString);
        }

        static public void selectAddressID (int startAddress)
        {
            string whereString = string.Format("where start_address = '{0}'   ", startAddress);
            Select(whereString);
        }
      
        #endregion
    }
}
