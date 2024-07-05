using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT
{
    public class Enum
    {
        /// <summary>
        /// COM相關設備
        /// </summary>
        public enum COMDevice
        {
            undefined = 0,
            OCC_CMFT_server = 10,
            OCC_CMFT_console_1 = 11,
            OCC_CMFT_console_2 = 12,
            OCC_CMFT_console_3 = 13,
            OCC_CMFT_console_4 = 14,
            OCC_CMFT_console_5 = 15,
            OCC_CMFT_console_6 = 16,
            OCC_CMFT_console_7 = 17,
            OCC_test_track_console = 18,
            BOCC_CMFT_server = 50,
            BOCC_CMFT_console_1 = 51,
            BOCC_CMFT_console_2 = 52,
            BOCC_CMFT_console_3 = 53,
            BOCC_test_track_console = 54,
            maintenance_console_1 = 61,
            maintenance_console_2 = 62,
            OCC_PA_server = 71,
            OCC_DMD_server = 72,
            OCC_CCTV_server = 73,
            OCC_DLT_server = 74,
            OCC_TETRA_server = 75,
            BOCC_PA_server = 81,
            BOCC_DMD_server = 82,
            BOCC_CCTV_server = 83,
            BOCC_DLT_server = 84,
            BOCC_TETRA_server = 85
        }

        /// <summary>
        /// 0xFF:車站全區域
        /// bit0:大廳
        /// bit1:月台1
        /// bit2:月台2
        /// bit3:非公共區
        /// </summary>
        public enum StationArea
        {
            all = 0xFF,
            concourse = 0b_0000_0001,
            platform1 = 0b_0000_0010,
            platform2 = 0b_0000_0100,
            non_public = 0b_0000_1000
        }

        /// <summary>
        /// 1:尖峰
        /// 2:離峰
        /// 3:夜間
        /// </summary>
        public enum PeakTimeType
        {
            peak_time = 1,
            Off_peak_time = 2,
            night_time = 3
        }

        /// <summary>
        /// 資料庫命令種類
        /// </summary>
        public enum SqlCommand
        {
            insert,
            update,
            delete
        }
    }
}
