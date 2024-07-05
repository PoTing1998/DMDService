using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.DB.Models.Others
{
    public class line_conf
    {
        [Key]
        public string line_id
        {
            get;
            set;
        }
        public string remark
        {
            get;
            set;
        }
        public string in_use
        {
            get;
            set;
        }
        public string upd_user
        {
            get;
            set;
        }

    
        public DateTime upd_time
        {
            get;
            set;
        }

    }

    public class line_operation
    {
        [Key]
        public int index
        {
            get;
            set;
        }

        public int lg01
        {
            get;
            set;
        }
        public int lg02
        {
            get;
            set;
        }
        public int lg03
        {
            get;
            set;
        }
        public int lg04
        {
            get;
            set;
        }
        public int lg05
        {
            get;
            set;
        }
        public int lg06
        {
            get;
            set;
        }
        public int lg07
        {
            get;
            set;
        }
        public int lg08
        {
            get;
            set;
        }
        public int lg08a
        {
            get;
            set;
        }
        public int lg09
        {
            get;
            set;
        }
        public int lg10
        {
            get;
            set;
        }
        public int lg11
        {
            get;
            set;
        }
        public int lg12
        {
            get;
            set;
        }
        public int lg13
        {
            get;
            set;
        }
        public int lg14
        {
            get;
            set;
        }
        public int lg15
        {
            get;
            set;
        }
        public int lg16
        {
            get;
            set;
        }
        public int lg17
        {
            get;
            set;
        }
        public int lg18
        {
            get;
            set;
        }
        public int lg19
        {
            get;
            set;
        }
        public int lg20
        {
            get;
            set;
        }
        public int lg21
        {
            get;
            set;
        }
        public string remark
        {
            get;
            set;
        }
       
    }

    public class platform_conf
    {
        [Key]
        public int platform_address
        {
            get;
            set;
        }

        public string platform_name
        {
            get;
            set;
        }
        public string platform_id
        {
            get;
            set;
        }
        public string train_direction
        {
            get;
            set;
        }
        public int approach_normal
        {
            get;
            set;
        }
        public int approach_reverse
        {
            get;
            set;
        }

        public int door_open
        {
            get;
            set;
        }

        public int dwell
        {
            get;
            set;
        }

        public int door_will_close
        {
            get;
            set;
        }

        public int depart
        {
            get;
            set;
        }

        public string remark
        {
            get;
            set;
        }

        public string upd_user
        {
            get;
            set;
        }


        public DateTime upd_time
        {
            get;
            set;
        }
    }

    public class station_conf
    {
        [Key]
        public string station_id
        {
            get;
            set;
        }
        [Key]
        public string line_id
        {
            get;
            set;
        }

        public int platform_address_up
        {
            get;
            set;
        }
        public int platform_address_dn
        {
            get;
            set;
        }
        public string english
        {
            get;
            set;
        }

        public string chinese
        {
            get;
            set;
        }
        public string in_use
        {
            get;
            set;
        }
        public string dcu_ip
        {
            get;
            set;
        }
        public string remark
        {
            get;
            set;
        }

        public string upd_user
        {
            get;
            set;
        }
        public DateTime upd_time
        {
            get;
            set;
        }

    }

    public class train_location
    {
        [Key]
        public string train_unit_id
        {
            get;
            set;
        }


        public string train_position
        {
            get;
            set;
        }

        public string train_destination
        {
            get;
            set;
        }

        public string train_event
        {
            get;
            set;
        }

        public string train_status
        {
            get;
            set;
        }



    }
}
