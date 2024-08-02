using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Msg.Parsing
{
    /// <summary>
    /// Generate checksum of CRC16.
    /// </summary>
    public class CRC16
    {
        #region Class-level declarations
        /// <summary>
        /// Object for CRC computations.
        /// </summary>
        private Classless.Hasher.CRC m_CRC = null;

        /// <summary>
        /// 取得可用來對 CRC16 進行同步存取的物件。
        /// </summary>
        public readonly object SyncRoot = new object();

        #endregion


        #region Methods
        /// <summary>
        /// Get CRC16 checksum.
        /// </summary>
        /// <param name="checksum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetChecksum(out byte[] checksum, byte[] value)
        {
            checksum = null;
            byte[] bytesChecksum = null;
            byte[] byteCRC16 = null;
            try
            {
                // Compute CRC16 checksum of parameter of value.
                lock (SyncRoot)
                {
                    bytesChecksum = m_CRC.ComputeHash(value);
                }

                // Getting high and low bytes.
                byte byteHB = bytesChecksum[0];
                byte byteLB = bytesChecksum[1];
                // Reverse high and low bytes.
                byteCRC16 = new byte[2] { byteLB, byteHB };
                // Output checksum.
                checksum = byteCRC16;

                return 0;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                return -1;
            }
            finally
            {
                bytesChecksum = null;
                byteCRC16 = null;
            }
        }
        #endregion


        #region Constructor & Destructor
        #region Constructor.
        /// <summary>
        /// Constructor.
        /// </summary>
        public CRC16()
        {
            try
            {
                m_CRC = new Classless.Hasher.CRC(
                    Classless.Hasher.CRCParameters.GetParameters(Classless.Hasher.CRCStandard.CRC16_ARC));
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
            }
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor.
        /// </summary>
        ~CRC16()
        {
            try
            {
                m_CRC = null;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
            }
        }
        #endregion
        #endregion
    }
}
