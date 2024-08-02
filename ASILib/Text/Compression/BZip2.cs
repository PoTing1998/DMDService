using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Compression
{
    /// <summary>
    /// BZip壓縮與解壓縮
    /// </summary>
    public static class BZip2
    {
        /// <summary>
        /// 使用BZip2將byte訊息壓縮
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] msg)
        {
            byte[] bZipData = null;
            System.IO.MemoryStream oMemoryStream = null;
            ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream oZipOut = null;
            
            try
            {
                oMemoryStream = new System.IO.MemoryStream();
                oZipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(oMemoryStream);
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
        /// BZip2將Byte解壓縮
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
            ICSharpCode.SharpZipLib.BZip2.BZip2InputStream oZipIn = null;
            try
            {
                oMemoryStream = new System.IO.MemoryStream(msg);
                oZipIn = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(oMemoryStream);
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

    }
}
