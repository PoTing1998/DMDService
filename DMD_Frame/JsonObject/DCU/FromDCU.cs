using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.JsonObject.DCU.FromDCU
{
    #region DCU  Server to  DMD Server
    public class Du_list : ASI.Wanda.DMD.JsonObject.Base
    {
        public Du_list(ASI.Wanda.DMD.Enum.Station station) : base(station)
        {
        }
        public string equip_id { get; set; }
        public int status { get; set; }
        public string dbName1 { get; set; } = "sys_equip_status";
    }
    //(4)	預錄訊息命令
    public class Res_SendPreRecordMessage : ASI.Wanda.DMD.JsonObject.Base
    {
        public Res_SendPreRecordMessage(ASI.Wanda.DMD.Enum.Station station) : base(station)
        {
        }
        //席位
        public string seatID { get; set; }
        // 即時訊息ID
        public string msg_id { get; set; }
        public string station_id { get; set; }

        public bool is_success { get; set; }    

        public List<string> failed_target { get; set; }
    }
    //(5).	即時訊息命令 SendInstantMessage
    public class Res_SendInstantMessage : ASI.Wanda.DMD.JsonObject.Base
    {
        public Res_SendInstantMessage(ASI.Wanda.DMD.Enum.Station station) : base(station)
        {
        }
        //席位
        public string seatID { get; set; }
        // 即時訊息ID
        public string msg_id { get; set; }
        public string station_id { get; set; }
        public bool is_success { get; set; }

        public List<string> failed_target { get; set; }
    }
    



    #endregion
}
