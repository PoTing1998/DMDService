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

        /// <summary>
        /// 插入完整的 OCS 数据
        /// </summary>
        static public int InsertOCSData(OCS_Data data)
        {
            return Insert(
                data.number_of_platforms,
                data.platform_id,
                data.arrival,
                data.departure,
                data.skip_hold,
                data.number_of_journey_data,
                data.validity_field,
                data.train_unit_id,
                data.service_number,
                data.trip_number,
                data.destination_number,
                data.arrivaltime,
                data.departuretime,
                data.delayatarrival,
                data.delayatdeparture,
                data.cancelledtrain,
                data.trainend_of_service,
                data.lasttrainoftheoperatingday,
                data.line_operation_mode,
                data.train_direction,
                data.validity_field2,
                data.train_unit_id2,
                data.service_number2,
                data.trip_number2,
                data.destination_number2,
                data.arrivaltime2,
                data.departuretime2,
                data.delayatarrival2,
                data.delayatdeparture2,
                data.cancelledtrain2,
                data.trainend_of_service2,
                data.lasttrainoftheoperatingday2,
                data.line_operation_mode2,
                data.train_direction2
            );
        }

        /// <summary>
        /// 查询指定 platform_id 的 OCS 数据是否存在
        /// </summary>
        static public OCS_Data SelectByPlatformID(int platform_id)
        {
            string whereString = string.Format("where platform_id = {0}", platform_id);
            var result = SelectWhere(whereString);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// 更新指定 platform_id 的 OCS 数据（使用自定义 WHERE 条件）
        /// </summary>
        static public int UpdateOCSDataByPlatformID(int platform_id, OCS_Data data)
        {
            string commandString = string.Format(@"
                update dbo.OCS_Data set
                    number_of_platforms = {0},
                    platform_id = {1},
                    arrival = {2},
                    departure = {3},
                    skip_hold = {4},
                    number_of_journey_data = {5},
                    validity_field = {6},
                    train_unit_id = {7},
                    service_number = {8},
                    trip_number = {9},
                    destination_number = {10},
                    arrivaltime = {11},
                    departuretime = {12},
                    delayatarrival = {13},
                    delayatdeparture = {14},
                    cancelledtrain = {15},
                    trainend_of_service = {16},
                    lasttrainoftheoperatingday = {17},
                    line_operation_mode = {18},
                    train_direction = {19},
                    validity_field2 = {20},
                    train_unit_id2 = {21},
                    service_number2 = {22},
                    trip_number2 = {23},
                    destination_number2 = {24},
                    arrivaltime2 = {25},
                    departuretime2 = {26},
                    delayatarrival2 = {27},
                    delayatdeparture2 = {28},
                    cancelledtrain2 = {29},
                    trainend_of_service2 = {30},
                    lasttrainoftheoperatingday2 = {31},
                    line_operation_mode2 = {32},
                    train_direction2 = {33}
                where platform_id = {34};",
                data.number_of_platforms,
                data.platform_id,
                data.arrival,
                data.departure,
                data.skip_hold,
                data.number_of_journey_data,
                data.validity_field,
                data.train_unit_id,
                data.service_number,
                data.trip_number,
                data.destination_number,
                data.arrivaltime,
                data.departuretime,
                data.delayatarrival,
                data.delayatdeparture,
                data.cancelledtrain,
                data.trainend_of_service,
                data.lasttrainoftheoperatingday,
                data.line_operation_mode,
                data.train_direction,
                data.validity_field2,
                data.train_unit_id2,
                data.service_number2,
                data.trip_number2,
                data.destination_number2,
                data.arrivaltime2,
                data.departuretime2,
                data.delayatarrival2,
                data.delayatdeparture2,
                data.cancelledtrain2,
                data.trainend_of_service2,
                data.lasttrainoftheoperatingday2,
                data.line_operation_mode2,
                data.train_direction2,
                platform_id
            );

            return NonQuery(commandString, null);
        }

        /// <summary>
        /// 插入或更新 OCS 数据（自动判断）
        /// </summary>
        static public int InsertOrUpdateOCSData(OCS_Data data)
        {
            var existing = SelectByPlatformID(data.platform_id);
            if (existing == null)
            {
                // 记录不存在，执行 Insert
                return InsertOCSData(data);
            }
            else
            {
                // 记录已存在，执行 Update
                return UpdateOCSDataByPlatformID(data.platform_id, data);
            }
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
