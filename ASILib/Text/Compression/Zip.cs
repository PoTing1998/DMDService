using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Compression
{
    /// <summary>
    /// Zip壓縮與解壓縮
    /// </summary>
    public static class Zip
    {

        /// <summary>
        /// 使用Zip將byte訊息壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] msg, int level)
        {
            byte[] bZipData = null;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.Zip.Compression.Deflater oDeflater = null;
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream oZipOut = null;
            try
            {

                if (level <= 1)
                {
                    level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.NO_COMPRESSION;
                }
                else if (level == 2)
                {
                    level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_SPEED;
                }
                else if (level == 3)
                {
                    level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.DEFLATED;
                }
                else if (level >= 4)
                {
                    level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION;
                }
                //建立記憶體的Stream
                oMemoryStream = new System.IO.MemoryStream();
                oDeflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(level);
                oZipOut = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(oMemoryStream, oDeflater, 131072);
                oZipOut.Write(msg, 0, msg.Length);
                oZipOut.Close();
                oZipOut.Dispose();
                bZipData = oMemoryStream.ToArray();
                return bZipData;

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
        }

        /// <summary>
        /// 使用Zip將byte訊息壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] msg)
        {
            byte[] bZipData = null;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream oZipOut = null;
            try
            {
                oMemoryStream = new System.IO.MemoryStream();
                oZipOut = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(oMemoryStream);
                oZipOut.Write(msg, 0, msg.Length);
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
        /// Zip將Byte解壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] DeCompress(byte[] msg)
        {
            byte[] bBuffer = new byte[2048];
            byte[] bUnZipData = null;
            int iLength = 0;
            int iIndex = 0;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream oZipIn = null;
            try
            {
                oMemoryStream = new System.IO.MemoryStream(msg);
                oZipIn = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(oMemoryStream);
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


    }
}
