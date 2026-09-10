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
        /// 以車站代碼取得該站 DCU 的 IP。
        /// station_conf 的主鍵為 (station_id, line_id)，未指定 line_id 時
        /// 若同一站存在多筆（多路線）資料，僅取第一筆並寫入警告記錄。
        /// </summary>
        /// <param name="stationID">車站代碼，例如 "LG08A"</param>
        /// <param name="lineID">路線代碼；null 或空字串表示不限定路線</param>
        /// <returns>該站 DCU 的 IP；查無資料或未設定時回傳 null</returns>
        static public string GetDcuIP(string stationID, string lineID = null)
        {
            if (string.IsNullOrWhiteSpace(stationID))
                return null;

            List<station_conf> rows;
            if (string.IsNullOrWhiteSpace(lineID))
            {
                rows = SelectWhere("where station_id = @station_id", new { station_id = stationID });
            }
            else
            {
                rows = SelectWhere("where station_id = @station_id and line_id = @line_id",
                                   new { station_id = stationID, line_id = lineID });
            }

            var usable = rows.Where(IsInUse).ToList();
            if (usable.Count == 0)
            {
                ASI.Lib.Log.DebugLog.Log("stationConf", $"查無車站 [{stationID}] 的 station_conf 資料 (line_id={lineID ?? "(不限)"})");
                return null;
            }
            if (usable.Count > 1)
            {
                ASI.Lib.Log.DebugLog.Log("stationConf",
                    $"車站 [{stationID}] 有 {usable.Count} 筆 station_conf 資料，未指定 line_id，取第一筆 (line_id={usable[0].line_id})");
            }

            var dcuIP = usable[0].dcu_ip;
            if (string.IsNullOrWhiteSpace(dcuIP))
            {
                ASI.Lib.Log.DebugLog.Log("stationConf", $"車站 [{stationID}] 的 dcu_ip 未設定");
                return null;
            }
            return dcuIP.Trim();
        }

        /// <summary>
        /// 以 DCU 的 IP 反查車站代碼，供 Socket Server 端在客戶端連入時判定來源車站使用。
        /// </summary>
        /// <param name="dcuIP">DCU 的 IP；可帶 "IP:port" 形式，會自動去除 port</param>
        /// <returns>車站代碼；查無對應時回傳 null</returns>
        static public string GetStationIdByDcuIp(string dcuIP)
        {
            if (string.IsNullOrWhiteSpace(dcuIP))
                return null;

            // 允許傳入 "10.107.26.99:2000" 這種 Socket 來源字串
            var ip = dcuIP.Split(':')[0].Trim();

            var match = SelectWhere("where dcu_ip = @dcu_ip", new { dcu_ip = ip })
                        .Where(IsInUse)
                        .FirstOrDefault();

            if (match == null)
            {
                ASI.Lib.Log.DebugLog.Log("stationConf", $"查無 dcu_ip = [{ip}] 對應的車站");
                return null;
            }
            return match.station_id;
        }

        /// <summary>
        /// 取得所有已設定 dcu_ip 的車站對照表 (station_id -&gt; dcu_ip)。
        /// </summary>
        static public Dictionary<string, string> GetAllDcuIP()
        {
            var result = new Dictionary<string, string>();
            foreach (var row in SelectAll().Where(IsInUse))
            {
                if (string.IsNullOrWhiteSpace(row.dcu_ip))
                    continue;
                if (!result.ContainsKey(row.station_id))
                    result.Add(row.station_id, row.dcu_ip.Trim());
            }
            return result;
        }

        /// <summary>
        /// 判斷該筆設定是否啟用。in_use 未設定時視為啟用，
        /// 僅在明確標示停用 (N / 0 / F) 時排除。
        /// </summary>
        static private bool IsInUse(station_conf row)
        {
            if (row == null)
                return false;
            if (string.IsNullOrWhiteSpace(row.in_use))
                return true;
            switch (row.in_use.Trim().ToUpperInvariant())
            {
                case "N":
                case "0":
                case "F":
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 舊名稱，實際回傳的是 dcu_ip 而非 station_id，請改用 <see cref="GetDcuIP"/>。
        /// </summary>
        [Obsolete("方法名稱與回傳值不符，請改用 GetDcuIP(stationID, lineID)")]
        static public string GetStationID(string stationID)
        {
            return GetDcuIP(stationID);
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
