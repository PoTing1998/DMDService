using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD
{
    public class Enum
    {
        /// <summary>
        /// 資料庫命令種類
        /// </summary>
        public enum SqlCommand
        {
            insert,
            update,
            delete
        }

        public enum Station
        {
            OCC,
            BOCC,
            LG01,
            LG02,
            LG03,
            LG04,
            LG05,
            LG06,
            LG07,
            LG08,
            LG08A,
        } 
    }
}
