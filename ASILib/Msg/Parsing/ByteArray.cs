using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Msg.Parsing
{
    public class ByteArray
    {
        /// <summary>
        /// 從byte[]中擷取出指定的內容
        /// </summary>
        /// <param name="source">
        /// Byte[]</param>
        /// <param name="startIdx">起始索引，必須大於或等於0</param>
        /// <param name="length">擷取長度，必須大於0</param>
        /// <returns></returns>
        public static byte[] Capture(byte[] source,int startIdx,int length)
        {
            byte[] arrReturn = null;
            int iLength = 0;

            if (source == null)
            {
                return null;
            }

            if (startIdx < 0)
            {
                return source;
            }

            if (length < 0)
            {
                return null;
            }
            else if (length == 0)
            {
                return new byte[0];
            }
            else if (length > 0)
            {
                if (length - startIdx > source.Length - startIdx)
                {
                    //欲擷取的長度超過來源長度
                    iLength = source.Length - startIdx;
                }
                else
                {
                    iLength = length;
                }

                arrReturn = new byte[iLength];
                Array.Copy(source, startIdx, arrReturn, 0, iLength);
            }

            return arrReturn;
        }

        /// <summary>
        /// 插入至byte[]
        /// </summary>
        /// <param name="insertByte">將插入的byte</param>
        /// <param name="source">原本的byte[]</param>
        /// <param name="Idx">指定原本byte[]的索引位置，在此插入insertByte</param>
        /// <returns></returns>
        public static byte[] Insert(byte insertByte, byte[] source, int Idx)
        {
            if (source == null)
            {
                return null;
            }

            if (Idx != 0 && 
                source.Length < Idx)
            {
                return source;
            }

            List<byte> oByteList = new List<byte>(source.Length);
            oByteList.AddRange(source);
            oByteList.Insert(Idx, insertByte);
            return oByteList.ToArray();            
        }

        /// <summary>
        /// 插入至byte[]
        /// </summary>
        /// <param name="insertByteArray">將插入的byte[]</param>
        /// <param name="source">原本的byte[]</param>
        /// <param name="Idx">指定原本byte[]的索引位置，在此插入insertByteArray</param>
        /// <returns></returns>
        public static byte[] Insert(byte[] insertByteArray, byte[] source, int Idx)
        {
            if (insertByteArray == null)
            {
                return null;
            }

            if (source == null)
            {
                return null;
            }

            if (Idx != 0 &&
                source.Length < Idx)
            {
                return source;
            }
               
            List<byte> oByteList = new List<byte>();
            oByteList.AddRange(source);

            for(int ii = insertByteArray.Length - 1; ii >=0; ii--)
            {
                oByteList.Insert(Idx, insertByteArray[ii]);
            }

            return oByteList.ToArray();
        }

        public static byte[] Combine(byte[] firstByteArray, byte[] secondByteArray)
        {
            byte[] ret = new byte[firstByteArray.Length + secondByteArray.Length];
            Buffer.BlockCopy(firstByteArray, 0, ret, 0, firstByteArray.Length);
            Buffer.BlockCopy(secondByteArray, 0, ret, firstByteArray.Length, secondByteArray.Length);
            return ret;
        }

        /// <summary>
        /// 將byte[]中從指定索引位置開始移除指定數量
        /// </summary>
        /// <param name="source">來源byte[]</param>
        /// <param name="Idx">指定索引位置</param>
        /// <param name="count">指定數量</param>
        /// <returns></returns>
        public static byte[] Remove(byte[] source, int Idx, int count)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Length < Idx + count)
            {
                return source;
            }

            List<byte> oByteList = new List<byte>(source.Length);
            oByteList.AddRange(source);
            oByteList.RemoveRange(Idx, count);
            return oByteList.ToArray();
        }

        /// <summary>
        /// 將數字(Int32)轉為byte[]
        /// </summary>
        /// <param name="source">來源數字</param>
        /// <param name="arrayLength">轉成陣列長度</param>
        /// <returns></returns>
        public static byte[] IntToBytes(int source, int arrayLength)
        {
            if (arrayLength > 0)
            {
                byte[] arrByte = new byte[arrayLength];

                for (int ii = arrayLength - 1; ii >= 0; ii--)
                {
                    if (ii != 0)
                    {
                        arrByte[ii] = (byte)((source >> (ii * 8)) & 0xFF);
                    }
                    else
                    {
                        arrByte[0] = (byte)(source & 0xFF);
                    }
                }

                //arrByte[3] = (byte)((source >> 24) & 0xFF);
                //arrByte[2] = (byte)((source >> 16) & 0xFF);
                //arrByte[1] = (byte)((source >> 8) & 0xFF);
                //arrByte[0] = (byte)(source & 0xFF);
                return arrByte;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 將數字(Int32)轉為byte[](長度4)
        /// </summary>
        /// <param name="source">來源數字</param>
        /// <param name="Endianness">位元組順序</param>
        /// <returns></returns>
        public static byte[] Int32ToBytes(Int32 source, Enum.Endianness Endianness)
        {
            byte[] arrByte = BitConverter.GetBytes(source);
            return BytesEndianness(arrByte, Endianness);
        }

        /// <summary>
        /// 將數字(UInt32)轉為byte[](長度4)
        /// </summary>
        /// <param name="source">來源數字</param>
        /// <param name="Endianness">位元組順序</param>
        /// <returns></returns>
        public static byte[] UInt32ToBytes(UInt32 source, Enum.Endianness Endianness)
        {
            byte[] arrByte = BitConverter.GetBytes(source);
            return BytesEndianness(arrByte, Endianness);
        }

        /// <summary>
        /// 將數字(Int16)轉為byte[](長度2)
        /// </summary>
        /// <param name="source">來源數字</param>
        /// <param name="Endianness">位元組順序</param>
        /// <returns></returns>
        public static byte[] Int16ToBytes(Int16 source, Enum.Endianness Endianness)
        {
            byte[] arrByte = BitConverter.GetBytes(source);
            return BytesEndianness(arrByte, Endianness);
        }

        /// <summary>
        /// 將數字(UInt16)轉為byte[](長度2)
        /// </summary>
        /// <param name="source">來源數字</param>
        /// <param name="Endianness">位元組順序</param>
        /// <returns></returns>
        public static byte[] UInt16ToBytes(UInt16 source, Enum.Endianness Endianness)
        {
            byte[] arrByte = BitConverter.GetBytes(source);
            return BytesEndianness(arrByte, Endianness);
        }

        /// <summary>
        /// 將byte[]轉換為指定的位元組順序
        /// </summary>
        /// <param name=""></param>
        /// <param name="Endianness"></param>
        /// <returns></returns>
        public static byte[] BytesEndianness(byte[] source, Enum.Endianness Endianness)
        {
            if (source != null)
            {
                byte[] arrByte = source;

                if (BitConverter.IsLittleEndian)
                {
                    if (Endianness == Enum.Endianness.LittleEndian)
                    {
                        return arrByte;
                    }
                    else
                    {
                        Array.Reverse(arrByte);
                    }
                }

                return arrByte;
            }

            return null;
        }

        /// <summary>
        /// 將byte[]轉成數字，從最右位元起算
        /// 例如:0b_0000_0000_0000_0010 = 2；0b_0000_0010_0000_0000 = 512
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int BytesToInt(byte[] source)
        {
            if (source == null)
            {
                return 0;
            }

            int iReturn = 0;
            for (int ii = source.Length - 1; ii >= 0; ii--)
            {
                int iMultiplier = (int)Math.Pow(256, ((source.Length - 1) - ii));
                iReturn = iReturn + source[ii] * iMultiplier;
            }

            return iReturn;
        }

        /// <summary>
        /// 比對兩個byte[]
        /// </summary>
        /// <param name="byteArray1">第一個byte[]</param>
        /// <param name="byteArray2">第二個byte[]</param>
        /// <returns>當 byteArray1 小於 byteArray2 會回傳負數，反之為正數，相等則為 0。</returns>
        public static int CompareBytes(byte[] byteArray1,byte[] byteArray2)
        {
            int result = ((System.Collections.IStructuralComparable)byteArray1)
            .CompareTo(byteArray2, Comparer<byte>.Default);

            return result;
        }

        /// <summary>
        /// 從陣列中尋找是否存在另一個指定陣列內容
        /// </summary>
        /// <param name="source">來源陣列</param>
        /// <param name="search">欲搜尋的陣列內容</param>
        /// <returns>若來源陣列可搜尋到則回傳索引位置，否則回傳-1</returns>
        public static int IndexOf(byte[] source, byte[] search)
        {
            int iResult = -1;

            if (source == null)
            {
                return iResult;
            }

            if (search == null)
            {
                return iResult;
            }

            string sSource = ASCIIEncoding.ASCII.GetString(source);
            string sSearch = ASCIIEncoding.ASCII.GetString(search);

            iResult = sSource.IndexOf(sSearch);

            return iResult;
        }
    }
}
