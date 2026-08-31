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

        /// <summary>
        /// 查詢指定 platform_id 的 OCS 資料是否存在
        /// </summary>
        static public OCS_Data SelectByPlatformID(int platform_id)
        {
            return SelectWhere("where platform_id = @platform_id", new { platform_id })
                   .FirstOrDefault();
        }

        /// <summary>
        /// 插入完整的 OCS 資料。欄位清單與值都由 Model 產生，不需在此逐一列出。
        /// </summary>
        static public int InsertOCSData(OCS_Data data)
        {
            return Insert(data);
        }

        /// <summary>
        /// 更新指定 platform_id 的 OCS 資料。欄位清單與值都由 Model 產生。
        /// </summary>
        static public int UpdateOCSDataByPlatformID(int platform_id, OCS_Data data)
        {
            return UpdateWhere(data, "where platform_id = @platform_id", new { platform_id });
        }

        /// <summary>
        /// 插入或更新 OCS 資料（自動判斷）
        /// </summary>
        static public int InsertOrUpdateOCSData(OCS_Data data)
        {
            var existing = SelectByPlatformID(data.platform_id);

            return existing == null
                 ? InsertOCSData(data)
                 : UpdateOCSDataByPlatformID(data.platform_id, data);
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
