using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Compression
{
    /// <summary>
    /// Compound壓縮與解壓縮
    /// </summary>
    public static class Compound
    {
        /// <summary>
        /// 壓縮方式
        /// </summary>
        public enum CompressionType
        {
            /// <summary>
            /// 未壓縮
            /// </summary>
            None,
            /// <summary>
            /// 
            /// </summary>
            Zip,
            /// <summary>
            /// 
            /// </summary>  
            GZip,
            /// <summary>
            /// 
            /// </summary>
            BZip2
        }

        /// <summary>
        /// 取得壓縮方式的定義值
        /// </summary>
        /// <param name="ctype">壓縮方式</param>
        /// <returns></returns>
        public static byte GetCompressionTypeValue(CompressionType ctype)
        {
            if (ctype == CompressionType.None)
            {
                return 0x00;
            }
            else if (ctype == CompressionType.Zip)
            {
                return 0x01;
            }
            else if (ctype == CompressionType.BZip2)
            {
                return 0x02;
            }
            else if (ctype == CompressionType.GZip)
            {
                return 0x03;
            }
            throw new Exception("the compression is undefined:" + ctype.ToString());
        }

        /// <summary>
        /// 取得壓縮方式
        /// </summary>
        /// <param name="value">定義值</param>
        /// <returns></returns>
        public static CompressionType GetCompressionType(byte value)
        {
            if (value == 0x00)
            {
                return CompressionType.None;
            }
            else if (value == 0x01)
            {
                return CompressionType.Zip;
            }
            else if (value == 0x02)
            {
                return CompressionType.BZip2;
            }
            else if (value == 0x03)
            {
                return CompressionType.GZip;
            }
            throw new Exception("the compression(0x" + System.Convert.ToString(value, 16).PadLeft(2, '0') + ") is undefined");
        }


        /// <summary>
        /// 取得解碼方式
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte GetEncodingValue(System.Text.Encoding encoding)
        {
            if (encoding == null)
            {
                return 0x00;
            }
            else if (encoding == System.Text.Encoding.Unicode)
            {
                return 0x01;
            }
            else if (encoding == System.Text.Encoding.ASCII)
            {
                return 0x02;
            }
            else if (encoding == System.Text.Encoding.UTF8)
            {
                return 0x03;
            }
            else if (encoding == System.Text.Encoding.UTF32)
            {
                return 0x04;
            }
            else if (encoding == System.Text.Encoding.UTF7)
            {
                return 0x05;
            }
            throw new Exception("the encoding(" + System.Convert.ToString(encoding).PadLeft(2, '0') + ") is undefined");
        }



        /// <summary>
        /// 取得編碼方式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static System.Text.Encoding GetEncoding(byte value)
        {

            if (value == 0x00)
            {
                return null;
            }
            else if (value == 0x01)
            {
                return System.Text.Encoding.Unicode;
            }
            else if (value == 0x02)
            {
                return System.Text.Encoding.ASCII;
            }
            else if (value == 0x03)
            {
                return System.Text.Encoding.UTF8;
            }
            else if (value == 0x04)
            {
                return System.Text.Encoding.UTF32;
            }
            else if (value == 0x05)
            {
                return System.Text.Encoding.UTF7;
            }
            throw new Exception("the encoding(0x" + System.Convert.ToString(value, 16).PadLeft(2, '0') + ") is undefined");
        }



        /// <summary>
        /// 使用將byte訊息壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] msg, System.Text.Encoding encoding, CompressionType ctype)
        {
            byte[] bZipData = null;
            byte[] bReZipData = null;
            try
            {
                if (ctype == CompressionType.None)
                {
                    bZipData = msg;
                }
                else if (ctype == CompressionType.Zip)
                {
                    bZipData = ASI.Lib.Text.Compression.Zip.Compress(msg);
                }
                else if (ctype == CompressionType.BZip2)
                {
                    bZipData = ASI.Lib.Text.Compression.BZip2.Compress(msg);
                }
                else if (ctype == CompressionType.GZip)
                {
                    bZipData = ASI.Lib.Text.Compression.GZip.CompressByICSharpCode(msg);
                }

                bReZipData = new byte[bZipData.Length + 2];
                bReZipData[0] = GetCompressionTypeValue(ctype);
                bReZipData[1] = GetEncodingValue(encoding);
                System.Array.Copy(bZipData, 0, bReZipData, 2, bZipData.Length);
                return bReZipData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bZipData = null;
                bReZipData = null;
            }
        }

        /// <summary>
        /// 使用將byte訊息壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <param name="ctype"></param>
        /// <param name="level">壓縮等級(0~9(最好))</param>
        /// <returns></returns>
        private static byte[] Compress(byte[] msg, System.Text.Encoding encoding, CompressionType ctype, int level)
        {
            byte[] bZipData = null;
            byte[] bReZipData = null;
            try
            {
                if (ctype == CompressionType.None)
                {
                    bZipData = msg;
                }
                else if (ctype == CompressionType.Zip)
                {
                    bZipData = ASI.Lib.Text.Compression.Zip.Compress(msg, level);
                }
                else if (ctype == CompressionType.BZip2)
                {
                    bZipData = ASI.Lib.Text.Compression.BZip2.Compress(msg);
                }
                else if (ctype == CompressionType.GZip)
                {
                    bZipData = ASI.Lib.Text.Compression.GZip.CompressByICSharpCode(msg, level);
                }

                bReZipData = new byte[bZipData.Length + 2];
                bReZipData[0] = GetCompressionTypeValue(ctype);
                bReZipData[1] = GetEncodingValue(encoding);
                System.Array.Copy(bZipData, 0, bReZipData, 2, bZipData.Length);
                return bReZipData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bZipData = null;
                bReZipData = null;
            }
        }


        /// <summary>
        /// 將訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="encoding">編碼方式</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強迫壓縮，如不強迫壓縮，當訊息長度小於200byte或訊息壓縮後長度未達原始長度的95%以下則不壓縮</param>
        /// <returns></returns>
        private static byte[] Compress(byte[] msg, System.Text.Encoding encoding, CompressionType ctype, bool isForce)
        {

            byte[] bReData = null;
            double dLen = 0;
            double dComLen = 0;
            try
            {
                if (!isForce)
                {
                    dLen = (double)(msg.Length);
                    if (dLen > 200)
                    {
                        bReData = Compress(msg, encoding, ctype);
                        dComLen = (double)(bReData.Length);
                        if (dComLen > 0)
                        {
                            double dPar = dComLen / dLen;
                            if (dPar > 0.95)
                            {
                                bReData = Compress(msg, encoding, CompressionType.None);
                            }
                        }
                    }
                    else
                    {
                        bReData = Compress(msg, encoding, CompressionType.None);
                    }
                }
                else
                {
                    bReData = Compress(msg, encoding, ctype);
                }
                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bReData = null;
            }
        }


        /// <summary>
        /// 將訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="encoding">編碼方式</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強迫壓縮，如不強迫壓縮，當訊息長度小於200byte或訊息壓縮後長度未達原始長度的95%以下則不壓縮</param>
        /// <param name="level">壓縮等級(0~9(最好))</param>
        /// <returns></returns>
        private static byte[] Compress(byte[] msg, System.Text.Encoding encoding, CompressionType ctype, bool isForce, int level)
        {
            byte[] bReData = null;
            double dLen = 0;
            double dComLen = 0;
            try
            {
                if (!isForce)
                {
                    dLen = (double)(msg.Length);
                    if (dLen > 200)
                    {
                        bReData = Compress(msg, encoding, ctype);
                        dComLen = (double)(bReData.Length);
                        if (dComLen > 0)
                        {
                            double dPar = dComLen / dLen;
                            if (dPar > 0.95)
                            {
                                bReData = Compress(msg, encoding, CompressionType.None);
                            }
                        }
                    }
                    else
                    {
                        bReData = Compress(msg, encoding, CompressionType.None);
                    }
                }
                else
                {
                    bReData = Compress(msg, encoding, ctype, level);
                }
                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bReData = null;
            }
        }


        /// <summary>
        /// 使用將byte訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強迫壓縮，如不強迫壓縮，當訊息長度小於200byte或訊息壓縮後長度未達原始長度的95%以下則不壓縮</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] msg, CompressionType ctype, bool isForce)
        {
            return Compress(msg, null, ctype, isForce);
        }



        /// <summary>
        /// 使用將byte訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強迫壓縮，如不強迫壓縮，當訊息長度小於200byte或訊息壓縮後長度未達原始長度的95%以下則不壓縮</param>
        /// <param name="level">壓縮等級(0~9(最好))</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] msg, CompressionType ctype, bool isForce, int level)
        {
            return Compress(msg, null, ctype, isForce, level);
        }

        /// <summary>
        ///  將字串訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="encoding">文字編碼方式</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強制壓縮</param>
        /// <returns></returns>
        public static byte[] Compress(string msg, System.Text.Encoding encoding, CompressionType ctype, bool isForce)
        {

            byte[] bData = null;
            byte[] bReData = null;
            try
            {
                if (msg != null)
                {
                    bData = encoding.GetBytes(msg);
                    bReData = Compress(bData, encoding, ctype, isForce);
                }

                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bData = null;
                bReData = null;
            }
        }


        /// <summary>
        ///  將字串訊息壓縮
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param>
        /// <param name="encoding">文字編碼方式</param>
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強制壓縮</param>
        /// <param name="level">壓縮等級(0~9(最好))</param>
        /// <returns></returns>
        public static byte[] Compress(string msg, System.Text.Encoding encoding, CompressionType ctype, bool isForce, int level)
        {

            byte[] bData = null;
            byte[] bReData = null;
            try
            {
                if (msg != null)
                {
                    bData = encoding.GetBytes(msg);
                    bReData = Compress(bData, encoding, ctype, isForce, level);
                }

                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bData = null;
                bReData = null;
            }
        }


        /// <summary>
        /// 將字串訊息壓縮(自動選擇最佳的文字編碼方式)
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param> 
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強制壓縮</param>
        /// <returns></returns>
        public static byte[] Compress(string msg, CompressionType ctype, bool isForce)
        {

            byte[] bData = null;
            byte[] bReData = null;
            System.Text.Encoding oEncoding = null;

            try
            {
                if (msg != null)
                {
                    oEncoding = System.Text.Encoding.ASCII;
                    bData = oEncoding.GetBytes(msg);
                    if (msg != oEncoding.GetString(bData))
                    {
                        oEncoding = System.Text.Encoding.UTF8;
                        bData = oEncoding.GetBytes(msg);
                        if (msg != oEncoding.GetString(bData))
                        {
                            oEncoding = System.Text.Encoding.Unicode;
                            bData = oEncoding.GetBytes(msg);
                            if (msg != oEncoding.GetString(bData))
                            {
                                oEncoding = System.Text.Encoding.UTF32;
                                bData = oEncoding.GetBytes(msg);
                            }
                        }
                    }
                    bReData = Compress(bData, oEncoding, ctype, isForce);
                }

                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bData = null;
                bReData = null;
                oEncoding = null;
            }
        }


        /// <summary>
        /// 將字串訊息壓縮(自動選擇最佳的文字編碼方式)
        /// </summary>
        /// <param name="msg">要壓縮的訊息</param> 
        /// <param name="ctype">壓縮方式</param>
        /// <param name="isForce">是否強制壓縮</param>
        /// <param name="level">壓縮等級(0~9(最好))</param>
        /// <returns></returns>
        public static byte[] Compress(string msg, CompressionType ctype, bool isForce, int level)
        {

            byte[] bData = null;
            byte[] bReData = null;
            System.Text.Encoding oEncoding = null;

            try
            {
                if (msg != null)
                {
                    oEncoding = System.Text.Encoding.ASCII;
                    bData = oEncoding.GetBytes(msg);
                    if (msg != oEncoding.GetString(bData))
                    {
                        oEncoding = System.Text.Encoding.UTF8;
                        bData = oEncoding.GetBytes(msg);
                        if (msg != oEncoding.GetString(bData))
                        {
                            oEncoding = System.Text.Encoding.Unicode;
                            bData = oEncoding.GetBytes(msg);
                            if (msg != oEncoding.GetString(bData))
                            {
                                oEncoding = System.Text.Encoding.UTF32;
                                bData = oEncoding.GetBytes(msg);
                            }
                        }
                    }
                    bReData = Compress(bData, oEncoding, ctype, isForce, level);
                }
                return bReData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bData = null;
                bReData = null;
                oEncoding = null;
            }
        }

        /// <summary>
        /// 將Byte解壓縮
        /// </summary>
        /// <param name="encoding">編碼方式</param>
        /// <param name="deCompressMsg">解壓訊息(如原始訊息格式為byte，則會使用System.Convert.ToBase64String轉成字串)</param>
        /// <param name="msg">壓縮訊息</param>
        public static void DeCompress(out System.Text.Encoding encoding, out string deCompressMsg, byte[] msg)
        {
            deCompressMsg = null;
            encoding = null;
            byte[] bData = null;
            DeCompress(out encoding, out bData, msg);
            if (bData != null)
            {
                if (encoding == null)
                {
                    deCompressMsg = System.Convert.ToBase64String(bData);
                }
                else
                {
                    deCompressMsg = encoding.GetString(bData);
                }
            }
        }

        /// <summary>
        /// 將Byte解壓縮
        /// </summary>
        /// <param name="encoding">編碼方式</param>
        /// <param name="deCompressMsg">解壓訊息</param>
        /// <param name="msg">壓縮訊息</param>
        public static void DeCompress(out System.Text.Encoding encoding, out byte[] deCompressMsg, byte[] msg)
        {
            deCompressMsg = null;
            encoding = null;
            CompressionType oCType;
            byte[] bData = null;
            try
            {
                if (msg != null && msg.Length >= 2)
                {
                    oCType = GetCompressionType(msg[0]);
                    encoding = GetEncoding(msg[1]);
                    bData = new byte[msg.Length - 2];
                    if (bData.Length > 0)
                    {
                        System.Array.Copy(msg, 2, bData, 0, bData.Length);
                    }
                    if (oCType == CompressionType.None)
                    {
                        deCompressMsg = bData;
                    }
                    else if (oCType == CompressionType.Zip)
                    {
                        deCompressMsg = ASI.Lib.Text.Compression.Zip.DeCompress(bData);
                    }
                    else if (oCType == CompressionType.BZip2)
                    {
                        deCompressMsg = ASI.Lib.Text.Compression.BZip2.DeCompress(bData);
                    }
                    else if (oCType == CompressionType.GZip)
                    {
                        deCompressMsg = ASI.Lib.Text.Compression.GZip.DeCompressByICSharpCode(bData);
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bData = null;
            }
        }
    }
}
