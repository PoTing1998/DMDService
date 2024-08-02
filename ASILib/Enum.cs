using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib
{
    public class Enum
    {
        /// <summary>
        /// 位元組順序，用來表示在多個位元組中，哪個位元組先被儲存，哪個位元組後被儲存
        /// </summary>
        public enum Endianness
        {
            /// <summary>
            /// 高位的位元組放在最高的記憶體位址上，例如：0x12345678會以0x78,0x56,0x34,0x12的順序儲存
            /// </summary>
            LittleEndian,

            /// <summary>
            /// 高位的位元組會放在最低的記憶體位址上，例如：0x12345678會以0x12,0x34,0x56,0x78的順序儲存
            /// </summary>
            BigEndian
        }
    }
}
