using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Compression
{
    /// <summary>
    /// GZip壓縮(速度快但壓縮率較低)
    /// 需注意，一些重複率不高的訊息內容壓縮後資料量可能會加倍
    /// </summary>
    public static class GZip
    {
        /// <summary>
        /// 使用ICSharpCode壓縮
        /// </summary>
        /// <param name="msg">msg</param>
        /// <param name="level">1 - store only to 9 - means best compression</param>
        /// <returns></returns>
        public static byte[] CompressByICSharpCode(byte[] msg, int level)
        {
            byte[] bZipData = null;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream oZipOut = null;

            try
            {
                if (level < 1)
                {
                    level = 1;
                }
                else if (level > 9)
                {
                    level = 9;
                }
                oMemoryStream = new System.IO.MemoryStream();
                oZipOut = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(oMemoryStream);
                oZipOut.SetLevel(level);
                oZipOut.Write(msg, 0, msg.Length);
                oZipOut.Flush();
                oZipOut.Close();
                oZipOut.Dispose();
                oZipOut = null;
                bZipData = oMemoryStream.ToArray();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oZipOut != null)
                {
                    oZipOut.Close();
                    oZipOut.Dispose();
                    oZipOut = null;
                }
                if (oMemoryStream != null)
                {
                    oMemoryStream.Close();
                    oMemoryStream.Dispose();
                    oMemoryStream = null;
                }
            }
            return bZipData;
        }

        /// <summary>
        /// 使用ICSharpCode壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] CompressByICSharpCode(byte[] msg)
        {
            byte[] bZipData = null;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream oZipOut = null;

            try
            {
                oMemoryStream = new System.IO.MemoryStream();
                oZipOut = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(oMemoryStream);
                oZipOut.SetLevel(9);
                oZipOut.Write(msg, 0, msg.Length);
                oZipOut.Flush();
                oZipOut.Close();
                oZipOut.Dispose();
                oZipOut = null;
                bZipData = oMemoryStream.ToArray();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oZipOut != null)
                {
                    oZipOut.Close();
                    oZipOut.Dispose();
                    oZipOut = null;
                }
                if (oMemoryStream != null)
                {
                    oMemoryStream.Close();
                    oMemoryStream.Dispose();
                    oMemoryStream = null;
                }
            }
            return bZipData;
        }

        /// <summary>
        /// 使用ICSharpCode解壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] DeCompressByICSharpCode(byte[] msg)
        {
            byte[] bBuffer = new byte[2048];
            byte[] bUnZipData = null;
            int iLength = 0;
            int iIndex = 0;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.GZip.GZipInputStream oZipIn = null;

            try
            {
                oMemoryStream = new System.IO.MemoryStream(msg);
                oZipIn = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(oMemoryStream);
                oZipIn.Flush();
                while (true)
                {
                    iLength = oZipIn.Read(bBuffer, 0, bBuffer.Length);
                    if (iLength > 0)
                    {
                        if (bUnZipData == null)
                        {
                            iIndex = 0;
                            bUnZipData = new byte[iLength];
                        }
                        else
                        {
                            iIndex = bUnZipData.Length;
                            System.Array.Resize<byte>(ref bUnZipData, iIndex + iLength);
                        }

                        System.Array.Copy(bBuffer, 0, bUnZipData, iIndex, iLength);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bBuffer = null;
                if (oZipIn != null)
                {
                    oZipIn.Close();
                    oZipIn.Dispose();
                    oZipIn = null;
                }
                if (oMemoryStream != null)
                {
                    oMemoryStream.Close();
                    oMemoryStream.Dispose();
                    oMemoryStream = null;
                }

            }
            return bUnZipData;
        }

        /// <summary>
        /// 使用System.IO.Compression.GZipStream解壓縮
        /// </summary>
        /// <param name="gzBuffer">壓縮的訊息</param>
        /// <returns></returns>
        public static byte[] DeCompress(byte[] gzmessage)
        {
            System.IO.MemoryStream oMemoryStream = null;
            byte[] bBuffer = null;
            int iMessageLength = 0;
            System.IO.Compression.GZipStream oGZipStream = null;

            try
            {
                oMemoryStream = new System.IO.MemoryStream();

                //read the length of the uncompressed string
                iMessageLength = BitConverter.ToInt32(gzmessage, 0);

                oMemoryStream.Write(gzmessage, 4, gzmessage.Length - 4);
                bBuffer = new byte[iMessageLength];
                oMemoryStream.Position = 0;


                oGZipStream = new System.IO.Compression.GZipStream(oMemoryStream, System.IO.Compression.CompressionMode.Decompress);
                oGZipStream.Read(bBuffer, 0, bBuffer.Length);

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (oGZipStream != null)
                {
                    oGZipStream.Close();
                    oGZipStream.Dispose();
                    oGZipStream = null;
                }

                if (oMemoryStream != null)
                {
                    oMemoryStream.Close();
                    oMemoryStream.Dispose();
                    oMemoryStream = null;
                }
            }
            return bBuffer;
        }

        /// <summary>
        /// 使用System.IO.Compression.GZipStream壓縮
        /// </summary>
        /// <param name="message">原始訊息</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] message)
        {
            System.IO.MemoryStream oMemoryStream = null;
            System.IO.Compression.GZipStream oGZipStream = null;
            byte[] bCompressedData = null;
            byte[] bGzBuffer = null;
            byte[] bBufferLength = null;
            try
            {
                oMemoryStream = new System.IO.MemoryStream();
                oGZipStream = new System.IO.Compression.GZipStream(oMemoryStream, System.IO.Compression.CompressionMode.Compress, true);
                oGZipStream.Write(message, 0, message.Length);
                oMemoryStream.Position = 0;
                bCompressedData = new byte[oMemoryStream.Length];
                oMemoryStream.Read(bCompressedData, 0, bCompressedData.Length);

                //make a 4-byte header with the uncompressed size of the timetable
                bGzBuffer = new byte[bCompressedData.Length + 4];
                System.Buffer.BlockCopy(bCompressedData, 0, bGzBuffer, 4, bCompressedData.Length);
                bBufferLength = BitConverter.GetBytes(message.Length);
                System.Buffer.BlockCopy(bBufferLength, 0, bGzBuffer, 0, bBufferLength.Length);

                return bGzBuffer;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                message = null;
                if (oGZipStream != null)
                {
                    oGZipStream.Close();
                    oGZipStream.Dispose();
                    oGZipStream = null;
                }
                if (oMemoryStream != null)
                {
                    oMemoryStream.Close();
                    oMemoryStream.Dispose();
                    oMemoryStream = null;
                }
                bCompressedData = null;
                bBufferLength = null;
            }

        }
    }
}
