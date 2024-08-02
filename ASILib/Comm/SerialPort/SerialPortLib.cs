using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm.SerialPort
{
    /// <summary>
    /// 核心通訊模組提供之序列埠物件。
    /// </summary>
    public class SerialPortLib :
         ASI.Lib.Comm.ReceivedEvents,
         ASI.Lib.Comm.ICommunication
    {
        #region Events

        /// <summary>
        /// 傳輸速率限制(%)
        /// </summary>
        private decimal m_SpeedPersend = 100;

        /// <summary>
        /// 傳輸速率上限(bps)
        /// </summary>
        private decimal m_LimitBPS = 115200;

        /// <summary>
        /// 用以接收資料的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler ReceivedEvent;

        /// <summary>
        /// 當通訊連接後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ConnectedEventHandler ConnectedEvent;

        /// <summary>
        /// 當通訊發生錯誤後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ErrorEventHandlers ErrorEvent;

        /// <summary>
        /// 當通訊斷線後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler DisconnectedEvent;


        private byte[] m_SendBuffer = new byte[0];


        #endregion


        #region EventBodies
        /// <summary>
        /// 預設處理 ReceivedEvent 的函式。
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="source"></param>
        void SerialPort_ReceivedEvent(byte[] dataBytes, string source)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 預設處理 ErrorEvent 之方法。
        /// </summary>
        /// <param name="exception"></param>
        void SerialPort_ErrorEvent(Exception exception)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 預設處理 ConnectedEvent 之方法。
        /// </summary>
        /// <param name="source"></param>
        void SerialPort_ConnectedEvent(string source)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 預設處理 DisconnectedEvent 之方法。
        /// </summary>
        /// <param name="source"></param>
        void SerialPort_DisconnectedEvent(string source)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 訊息接收處理器
        /// </summary>
        private void ReceiveEngine()
        {
            byte[] bReadBuffer = null;
            System.DateTime oTime;
            while (m_IsStart)
            {
                if (m_oSerialPort != null && m_oSerialPort.IsOpen)
                {
                    try
                    {
                        int iBytes = m_oSerialPort.BytesToRead;
                        if (iBytes > 0)
                        {
                            oTime = System.DateTime.Now;
                            bReadBuffer = new byte[iBytes];
                            m_oSerialPort.Read(bReadBuffer, 0, iBytes);
                            ReceivedEvent(bReadBuffer, m_oSerialPort.PortName);
                            bReadBuffer = null;
                            System.Threading.Thread.Sleep(1);
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(3);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                        this.ErrorEvent(ex);
                        System.Threading.Thread.Sleep(1);
                    }
                    finally
                    {
                        bReadBuffer = null;
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(5);
                }
            }
        }

        private string ByteConvertToString(byte[] msg)
        {
            string sMsg = "";
            if (msg != null)
            {
                foreach (byte item in msg)
                {
                    if (sMsg != "")
                    {
                        sMsg += " ";
                    }
                    sMsg += System.Convert.ToString(item, 16).ToUpper().PadLeft(2, '0');
                }
            }
            return sMsg;
        }

        #endregion


        #region Module Level Variables

        /// <summary>
        /// 處理訊息接收的Thread
        /// </summary>
        private System.Threading.Thread m_ReceiveThread = null;


        /// <summary>
        /// 處理傳送的Thread
        /// </summary>
        private System.Threading.Thread m_SendThread = null;

        /// <summary>
        /// 傳送逾時
        /// </summary>
        private const double m_MAX_RESPONSE_TIME = 3000;

        /// <summary>
        /// 最大的SEND BUFFER SIZE
        /// </summary>
        private const int m_MAX_SEND_BUFFER_SIZE = 4096;

        /// <summary>
        /// 控制只能單線傳送
        /// </summary>
        private object m_SendSignal = new object();

        /// <summary>
        /// 使用 .NET 2.0 提供的 SerialPort 物件進行操作。
        /// </summary>
        private System.IO.Ports.SerialPort m_oSerialPort = null;

        /// <summary>
        /// 通訊裝置的連線字串。
        /// 例如："PortName=COM1;BaudRate=9600;DataBits=8;StopBits=One;Parity=None"。
        /// </summary>
        private string m_sConnectionString;

        /// <summary>
        /// 與 SerialPort 對應的參數
        /// </summary>
        private string m_sPortName;
        /// <summary>
        /// 與 SerialPort 對應的參數
        /// </summary>
        private int m_iBaudRate = 9600;
        /// <summary>
        /// 與 SerialPort 對應的參數
        /// </summary>
        private int m_iDataBits;
        /// <summary>
        /// 與 SerialPort 對應的參數
        /// </summary>
        private System.IO.Ports.StopBits m_enumStopBits;
        /// <summary>
        /// 與 SerialPort 對應的參數
        /// </summary>
        private System.IO.Ports.Parity m_enumParity;
        /// <summary>
        /// 訊息長度
        /// </summary>
        private byte[] m_bytesSerialPortBuffer = new byte[1024];


        bool m_IsStart = false;

        ///// <summary>
        ///// Timer Thread
        ///// </summary>
        //private System.Threading.Timer m_oTimer = null;
        /// <summary>
        /// 管理Timer Thread的Lock物件
        /// </summary>
        private object m_oLockBuffer = new object();

        #endregion


        #region Properties
        /// <summary>
        /// 存取通訊裝置之連線字串。
        /// 例如："PortName=COM1;BaudRate=9600;DataBits=8;StopBits=One;Parity=None"。
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
            set
            {
                m_sConnectionString = value;
                ParseConnectString();
            }
        }

        /// <summary>
        /// 通訊埠名稱。
        /// </summary>
        public string PortName
        {
            get
            {
                return m_sPortName;
            }
            set
            {
                m_sPortName = value;
                ComposeConnectionString();
            }
        }

        /// <summary>
        /// 通訊埠之 baud rate。
        /// </summary>
        public int BaudRate
        {
            get
            {
                return m_iBaudRate;
            }
            set
            {
                int iBaudRate = value;
                if (iBaudRate < 1)
                {
                    iBaudRate = 1;
                }
                m_iBaudRate = iBaudRate;
                m_LimitBPS = (decimal)m_iBaudRate * (m_SpeedPersend / 100);
                ComposeConnectionString();
            }
        }

        /// <summary>
        /// 通訊埠之 data bits。
        /// </summary>
        public int DataBits
        {
            get
            {
                return m_iDataBits;
            }
            set
            {
                m_iDataBits = value;
                ComposeConnectionString();
            }
        }

        /// <summary>
        /// 通訊埠之 stop bits。
        /// </summary>
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                return m_enumStopBits;
            }
            set
            {
                m_enumStopBits = value;
                ComposeConnectionString();
            }
        }

        /// <summary>
        /// 通訊埠之 parity。
        /// </summary>
        public System.IO.Ports.Parity Parity
        {
            get
            {
                return m_enumParity;
            }
            set
            {
                m_enumParity = value;
                ComposeConnectionString();
            }
        }



        /// <summary>
        /// 是否開啟
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (m_oSerialPort != null)
                {
                    return m_oSerialPort.IsOpen;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 取得或設定傳輸速率(%) 必須 >= 1;
        /// </summary>
        public decimal SpeedPersend
        {
            set
            {
                decimal devSpeedPersend = value;
                if (devSpeedPersend > 100)
                {
                    devSpeedPersend = 100;
                }
                else if (devSpeedPersend < 1)
                {
                    devSpeedPersend = 1;
                }
                m_SpeedPersend = devSpeedPersend;
                m_LimitBPS = (decimal)m_iBaudRate * (m_SpeedPersend / 100);
            }
            get
            {
                return m_SpeedPersend;
            }
        }


        #endregion


        #region Methods


        /// <summary>
        /// 開啟已設定之的通訊埠。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：無法開啟。</returns>
        /// <returns>-3：剖析連線字串發生錯誤。</returns>
        /// <returns>-4：初始化序列埠參數發生錯誤。</returns>
        public int Open()
        {
            System.Threading.ThreadStart oThreadStart = null;
            try
            {
                // 檢查通訊埠是否已關閉，若開啟則關閉之。
                if (m_oSerialPort.IsOpen)
                {
                    this.Close();
                    System.Threading.Thread.Sleep(500);
                    m_ReceiveThread = null;
                    m_SendThread = null;
                }

                m_IsStart = true;
                // 對通訊埠進行初始化並開啟。
                if (m_sConnectionString.Length > 0)
                {
                    if (ParseConnectString() != 0)
                    {
                        return -3;
                    }
                }
                if (InitialSerialPort() != 0)
                {
                    return -4;
                }

                m_oSerialPort.ReadTimeout = 3000;
                m_oSerialPort.WriteTimeout = 3000;
                m_oSerialPort.ReadBufferSize = 10240;
                m_oSerialPort.WriteBufferSize = 5120;

                lock (m_SendSignal)
                {
                    m_SendBuffer = null;
                    m_SendBuffer = new byte[0];
                }

                oThreadStart = new System.Threading.ThreadStart(ReceiveEngine);
                m_ReceiveThread = new System.Threading.Thread(oThreadStart);
                oThreadStart = null;
                oThreadStart = new System.Threading.ThreadStart(SendEngine);
                m_SendThread = new System.Threading.Thread(oThreadStart);
                oThreadStart = null;

                m_ReceiveThread.Start();
                m_SendThread.Start();
                m_oSerialPort.Open();

                // 檢驗是否確實開啟。
                if (!m_oSerialPort.IsOpen)
                {
                    return -2;
                }

                this.ConnectedEvent(this.m_sConnectionString);
                return 0;
            }

            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }


        /// <summary>
        /// 測試用，呼叫時會寫入狀態
        /// </summary>
        public void WriteStatus()
        {
            string sName = "";
            string sValue = "";
            string sMessage = "\r\n====== " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + " ======\r\n";
            if (m_oSerialPort != null)
            {
                sName = "CDHolding";
                try
                {
                    sValue = m_oSerialPort.CDHolding.ToString();
                }
                catch
                {
                    sValue = "Err";
                }
                sMessage += sName + "=" + sValue + "\r\n";


                sName = "CtsHolding";
                try
                {
                    sValue = m_oSerialPort.CtsHolding.ToString();
                }
                catch
                {
                    sValue = "Err";
                }
                sMessage += sName + "=" + sValue + "\r\n";


                sName = "DsrHolding";
                try
                {
                    sValue = m_oSerialPort.DsrHolding.ToString();
                }
                catch
                {
                    sValue = "Err";
                }
                sMessage += sName + "=" + sValue + "\r\n";


                sName = "IsOpen";
                try
                {
                    sValue = m_oSerialPort.IsOpen.ToString();
                }
                catch
                {
                    sValue = "Err";
                }
                sMessage += sName + "=" + sValue + "\r\n";

            }
            else
            {
                sName = "CDHolding";
                sValue = "Err";
                sMessage += sName + "=" + sValue + "\r\n";

                sName = "CtsHolding";
                sValue = "Err";
                sMessage += sName + "=" + sValue + "\r\n";


                sName = "DsrHolding";
                sValue = "Err";
                sMessage += sName + "=" + sValue + "\r\n";

                sName = "IsOpen";
                sValue = "Err";
                sMessage += sName + "=" + sValue + "\r\n";
            }

            ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), sMessage);
        }

        /// <summary>
        /// 將通訊埠關閉。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：無法關閉序列埠。</returns>
        public int Close()
        {
            try
            {
                m_oSerialPort.Close();
                // 檢驗是否確實關閉。
                if (m_oSerialPort.IsOpen)
                {
                    return -2;
                }
                System.Threading.Thread.Sleep(500);
                m_IsStart = false;
                System.Threading.Thread.Sleep(100);
                m_ReceiveThread = null;
                m_SendThread = null;
                lock (m_SendSignal)
                {
                    m_SendBuffer = null;
                    m_SendBuffer = new byte[0];
                }

                this.DisconnectedEvent(this.m_sConnectionString);
                return 0;
            }
            catch (System.Exception ex)
            {
                m_IsStart = false;
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }


        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataText"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：通訊埠未開啟。</returns>
        public int Send(string dataText)
        {
            byte[] bMsg = null;
            try
            {
                // 若通訊埠已開啟則傳送資料。
                if (m_oSerialPort != null && m_oSerialPort.IsOpen)
                {
                    bMsg = m_oSerialPort.Encoding.GetBytes(dataText);
                    return Send(bMsg);
                }
                else
                {
                    // 通訊埠未開啟。
                    return -2;
                }

            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                this.ErrorEvent(ex);
                return -1;
            }
            finally
            {
                bMsg = null;
            }
        }

        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataText"></param>
        /// <param name="target"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：通訊埠未開啟。</returns>
        public int Send(
            string dataText,
            string target)
        {
            try
            {
                return Send(dataText);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }


        private void SendEngine()
        {
            int iBufferLength = 0;
            byte[] bSendBuffer = null;
            byte[] bWaitBuffer = null;
            int iSendSize = 1024;

            decimal decLimitBPS = (decimal)m_iBaudRate;

            //需等待的全部時間
            int iTotalSleepTimeSpan = 1;
            //需等待的時間
            int iSleepTimeSpan = 1;

            while (m_IsStart)
            {
                if (iTotalSleepTimeSpan <= 0)
                {
                    iTotalSleepTimeSpan = 1;
                    try
                    {
                        if (m_oSerialPort != null && m_oSerialPort.IsOpen && m_SendBuffer != null)
                        {
                            lock (m_SendSignal)
                            {
                                iBufferLength = m_SendBuffer.Length;
                                if (iBufferLength > 0)
                                {
                                    if (iBufferLength > iSendSize)
                                    {
                                        bSendBuffer = new byte[iSendSize];
                                        bWaitBuffer = new byte[iBufferLength - iSendSize];
                                        System.Array.Copy(m_SendBuffer, 0, bSendBuffer, 0, iSendSize);
                                        System.Array.Copy(m_SendBuffer, iSendSize, bWaitBuffer, 0, bWaitBuffer.Length);

                                        if (m_oSerialPort != null && m_oSerialPort.IsOpen)
                                        {
                                            try
                                            {
                                                //計算每秒傳送速度上線  
                                                m_oSerialPort.Write(bSendBuffer, 0, iSendSize);
                                                m_SendBuffer = null;
                                                m_SendBuffer = bWaitBuffer;
                                                iTotalSleepTimeSpan = (int)System.Math.Ceiling((((decimal)iSendSize * 8) / m_LimitBPS) * 1000);
                                            }
                                            catch (System.Exception ex)
                                            {
                                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                                                this.ErrorEvent(ex);
                                            }
                                            finally
                                            {
                                                bSendBuffer = null;
                                                bWaitBuffer = null;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (m_oSerialPort != null && m_oSerialPort.IsOpen)
                                        {
                                            try
                                            {
                                                bSendBuffer = new byte[iBufferLength];
                                                System.Array.Copy(m_SendBuffer, 0, bSendBuffer, 0, iBufferLength);
                                                m_oSerialPort.Write(bSendBuffer, 0, iBufferLength);
                                                m_SendBuffer = null;
                                                iTotalSleepTimeSpan = (int)System.Math.Ceiling((((decimal)iBufferLength * 8) / m_LimitBPS) * 1000);
                                            }
                                            catch (System.Exception ex)
                                            {
                                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                                                this.ErrorEvent(ex);
                                            }
                                            finally
                                            {
                                                bSendBuffer = null;
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                        this.ErrorEvent(ex);
                    }
                    finally
                    {
                        bSendBuffer = null;
                        bWaitBuffer = null;
                    }

                }


                try
                {
                    if (iTotalSleepTimeSpan >= 200)
                    {
                        iSleepTimeSpan = 200;
                        iTotalSleepTimeSpan -= iSleepTimeSpan;
                    }
                    else
                    {
                        iSleepTimeSpan = iTotalSleepTimeSpan;
                        iTotalSleepTimeSpan = 0;
                    }

                    System.Threading.Thread.Sleep(iSleepTimeSpan);
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                    this.ErrorEvent(ex);
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        private object m_LockCallSend = new object();

        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：通訊埠未開啟。</returns>
        public int Send(byte[] dataBytes)
        {
            try
            {
                // m_MAX_SEND_BUFFER_SIZE
                // 若通訊埠已開啟則傳送資料。 
                long lLastInputInSendBuffer = 0;
                int iIndex = 0;
                int iSourceLength = 0;
                int iBufferLength = 0;
                int iBufferSIndex = 0;
                int iSendLength = 0;
                System.TimeSpan oTimeSpan;
                lock (m_LockCallSend)
                {
                    if (m_oSerialPort != null && m_oSerialPort.IsOpen && dataBytes != null && dataBytes.Length > 0)
                    {
                        iSourceLength = dataBytes.Length;
                        lLastInputInSendBuffer = System.DateTime.Now.Ticks;
                        while (iSourceLength > 0)
                        {
                            lock (m_SendSignal)
                            {
                                if (m_SendBuffer == null)
                                {
                                    iBufferLength = 0;
                                    iBufferSIndex = 0;
                                }
                                else
                                {
                                    iBufferLength = m_SendBuffer.Length;
                                    iBufferSIndex = iBufferLength;
                                }

                                //Buffer有空位才塞資料
                                if (iBufferLength < m_MAX_SEND_BUFFER_SIZE)
                                {
                                    iSendLength = m_MAX_SEND_BUFFER_SIZE - iBufferLength;
                                    if (iSendLength > iSourceLength)
                                    {
                                        iSendLength = iSourceLength;
                                    }

                                    if (m_SendBuffer == null)
                                    {
                                        m_SendBuffer = new byte[iSendLength];
                                    }
                                    else
                                    {
                                        System.Array.Resize<byte>(ref m_SendBuffer, iBufferLength + iSendLength);
                                    }
                                    
                                    System.Array.Copy(dataBytes, iIndex, m_SendBuffer, iBufferSIndex, iSendLength);

                                    iIndex += iSendLength;
                                    iSourceLength -= iSendLength;
                                    lLastInputInSendBuffer = System.DateTime.Now.Ticks;
                                }
                                else
                                {
                                    oTimeSpan = new TimeSpan(System.DateTime.Now.Ticks - lLastInputInSendBuffer);
                                    if (oTimeSpan.TotalMilliseconds > m_MAX_RESPONSE_TIME)
                                    {
                                        ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"訊息寫入傳送Buffer逾時，超過{m_MAX_RESPONSE_TIME}ms 無回應!!");
                                        break; 
                                    }
                                }
                            }

                            if (iSourceLength > 0)
                            {
                                System.Threading.Thread.Sleep(1);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        // 通訊埠未開啟。
                        return -2;
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                this.ErrorEvent(ex);
                return -1;
            }
        }

        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="target"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：通訊埠未開啟。</returns>
        public int Send(byte[] dataBytes, string target)
        {
            return Send(dataBytes);
        }

        #endregion


        #region Constructor & Destructor
        /// <summary>
        /// 類別建構子
        /// </summary>
        public SerialPortLib()
        {
            try
            {
                //計算速度上限
                m_LimitBPS = (decimal)m_iBaudRate * (m_SpeedPersend / 100);

                m_oSerialPort = new System.IO.Ports.SerialPort();
                this.ReceivedEvent += new ReceivedEventHandler(SerialPort_ReceivedEvent);
                this.ConnectedEvent += new ConnectedEventHandler(SerialPort_ConnectedEvent);
                this.ErrorEvent += new ErrorEventHandlers(SerialPort_ErrorEvent);
                this.DisconnectedEvent += new DisconnectedEventHandler(SerialPort_DisconnectedEvent);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
            }
        }

        /// <summary>
        /// 類別解構子
        /// </summary>
        ~SerialPortLib()
        {
            try
            {
                m_oSerialPort.Close();
                m_IsStart = false;
            }
            catch (System.Exception ex)
            {
                m_IsStart = false;
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
            }
        }
        #endregion


        #region Other Functions
        /// <summary>
        /// 將已被剖析或定義好的 SerialPort 相關屬性進行寫入與設定。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int InitialSerialPort()
        {
            try
            {
                m_oSerialPort.PortName = m_sPortName;
                m_oSerialPort.BaudRate = m_iBaudRate;
                m_oSerialPort.DataBits = m_iDataBits;
                m_oSerialPort.StopBits = m_enumStopBits;
                m_oSerialPort.Parity = m_enumParity;

                return 0;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 剖析 ConnectionString，並寫入相對應的 SerialPort 屬性。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int ParseConnectString()
        {
            try
            {
                string[] sParams = ASI.Lib.Text.Parsing.String.Split(m_sConnectionString.Trim(), ";");

                foreach (string ss in sParams)
                {
                    string[] sLRs = ASI.Lib.Text.Parsing.String.Split(ss.Trim(), "=");

                    if (sLRs.Length >= 2)
                    {
                        string sLvalue = sLRs[0].Trim().ToUpper();
                        string sRvalue = sLRs[1].Trim();

                        if (sLvalue == "PortName".ToUpper())
                        {
                            m_sPortName = sRvalue;
                        }
                        else if (sLvalue == "BaudRate".ToUpper())
                        {
                            m_iBaudRate = int.Parse(sRvalue);
                        }
                        else if (sLvalue == "DataBits".ToUpper())
                        {
                            m_iDataBits = int.Parse(sRvalue);
                        }
                        else if (sLvalue == "StopBits".ToUpper())
                        {
                            m_enumStopBits = (System.IO.Ports.StopBits)System.Enum.Parse(typeof(System.IO.Ports.StopBits), sRvalue);
                        }
                        else if (sLvalue == "Parity".ToUpper())
                        {
                            m_enumParity = (System.IO.Ports.Parity)System.Enum.Parse(typeof(System.IO.Ports.Parity), sRvalue);
                        }
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }

        /// <summary>
        /// 從此物件的屬性重組出連線字串。
        /// </summary>
        /// <returns></returns>
        private int ComposeConnectionString()
        {
            try
            {
                System.Text.StringBuilder ss = new System.Text.StringBuilder();

                ss.Length = 0;
                ss.Append("PortName=" + m_sPortName + ";");
                ss.Append("BaudRate=" + m_iBaudRate.ToString() + ";");
                ss.Append("DataBits=" + m_iDataBits.ToString() + ";");
                ss.Append("StopBits=" + m_enumStopBits.ToString() + ";");
                ss.Append("Parity=" + m_enumParity.ToString() + ";");
                m_sConnectionString = ss.ToString();

                return 0;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }


        #endregion
    }
}
