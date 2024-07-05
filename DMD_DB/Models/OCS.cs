using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Wanda.DMD.DB.Models
{
    public  class ocs_data
    {
        [Key]
        public int number_of_platforms
        {
            get;
            set;
        }

        [Key]
        public int platform_id
        {
            get;
            set;
        }

        public int arrival
        {
            get;
            set;
        }

        public int departure
        {
            get;
            set;
        }

        public int skip_hold
        {
            get;
            set;
        }
        public int number_of_journey_data
        {
            get;
            set;
        }
        public int validity_field
        {
            get;
            set;
        }
        public int train_unit_id
        {
            get;
            set;
        }
        public int service_number
        {
            get;
            set;
        }
        public int trip_number
        {
            get;
            set;
        }
        public int destination_number
        {
            get;
            set;
        }
        public int arrivaltime
        {
            get;
            set;
        }
        public int departuretime
        {
            get;
            set;
        }
        public int delayatarrival
        {
            get;
            set;
        }
        public int delayatdeparture
        {
            get;
            set;
        }
        public int cancelledtrain
        {
            get;
            set;
        }
        public int trainend_of_service
        {
            get;
            set;
        }
        public int lasttrainoftheoperatingday
        {
            get;
            set;
        }
        public int line_operation_mode
        {
            get;
            set;
        }
        public int train_direction
        {
            get;
            set;
        }
        public int validity_field2
        {
            get;
            set;
        }
        public int train_unit_id2
        {
            get;
            set;
        }
        public int service_number2
        {
            get;
            set;
        }
        public int trip_number2
        {
            get;
            set;
        }
        public int destination_number2
        {
            get;
            set;
        }
        public int arrivaltime2
        {
            get;
            set;
        }
        public int departuretime2
        {
            get;
            set;
        }

        public int delayatarrival2
        {
            get;
            set;
        }
        public int delayatdeparture2
        {
            get;
            set;
        }
        public int cancelledtrain2
        {
            get;
            set;
        }
        public int trainend_of_service2
        {
            get;
            set;
        }
        public int lasttrainoftheoperatingday2
        {
            get;
            set;
        }
        public int line_operation_mode2
        {
            get;
            set;
        }
        public int train_direction2
        {
            get;
            set;
        }
    }
}
