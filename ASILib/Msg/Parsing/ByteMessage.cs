using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Msg.Parsing
{
    /// <summary>
    /// byte訊息切割
    /// </summary>
    public class ByteMessage
    {
        /// <summary>
        /// 起始位元
        /// </summary>
        private byte m_HeadByte = 0x00;

        /// <summary>
        /// 結束位元
        /// </summary>
        private byte m_EndByte = 0x00;

        /// <summary>
        /// 完整訊息
        /// </summary>
        private ASI.Lib.Queue.QueueLib<byte[]> m_FinalMessage = new ASI.Lib.Queue.QueueLib<byte[]>(3000);

        /// <summary>
        /// 目前Parsing的狀態
        /// </summary>
        private byte m_ParsingType = 0x00;

        /// <summary>
        /// 不完整訊息
        /// </summary>
        private byte[] m_NoFinalMessage = new byte[0];

        /// <summary>
        /// Final Message訊息數超過限制大小
        /// </summary>
        public event ASI.Lib.Queue.OverflowEventHandler FinalMessageOverFlow;


        private object m_LockInput = new object();

        /// <summary>
        /// 是否為偵錯模式
        /// </summary>
        private bool m_IsDebugMode = false;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="head">起始位元</param>
        /// <param name="end">結束位元</param>
        public ByteMessage(byte head, byte end)
        {
            m_HeadByte = head;
            m_EndByte = end;
            m_FinalMessage.MaxSize = 0;
        }

        /// <summary>
        /// 取得或設定是否啟用Debug模式，啟用後若切割訊息出現異常，會寫詳細Log(包含輸入的訊息內容)
        /// </summary>
        public bool IsDebugMode
        {
            get
            {
                return this.m_IsDebugMode;
            }
            set
            {
                this.m_IsDebugMode = value;
            }
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="head">起始位元</param>
        /// <param name="end">結束位元</param>
        /// <param name="isDebugMode">是否為偵錯模式(預設為false)</param>
        public ByteMessage(byte head, byte end, bool isDebugMode)
        {
            m_HeadByte = head;
            m_EndByte = end;
            m_FinalMessage.MaxSize = 0;
            m_IsDebugMode = isDebugMode;
        }


        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="head">起始位元</param>
        /// <param name="end">結束位元</param>
        /// <param name="maxSize">限制儲存Final訊息數</param>
        /// <param name="overflowControlType">當訊息數超過時的行為</param>
        public ByteMessage(byte head, byte end, int maxSize, ASI.Lib.Queue.OverflowControlType overflowControlType)
        {
            m_HeadByte = head;
            m_EndByte = end;
            m_FinalMessage.MaxSize = maxSize;
            m_FinalMessage.OverflowControl = overflowControlType;
            m_FinalMessage.MessageOverflow += new ASI.Lib.Queue.OverflowEventHandler(m_FinalMessage_MessageOverflow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fe"></param>
        private void m_FinalMessage_MessageOverflow(object sender, ASI.Lib.Queue.OverflowEventArgs fe)
        {
            try
            {
                if (FinalMessageOverFlow != null)
                {
                    FinalMessageOverFlow(this, fe);
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// 取得完整訊息長度
        /// </summary>
        /// <returns></returns>
        public int FinalCount()
        {
            return m_FinalMessage.Count;
        }

        /// <summary>
        /// 取得不完整訊息byte長度
        /// </summary>
        /// <returns></returns>
        public int NoFinalLength()
        {
            int iLength = 0;
            byte[] bNoFinal = m_NoFinalMessage;
            if (bNoFinal != null)
            {
                iLength = bNoFinal.Length;
            }
            bNoFinal = null;
            return iLength;
        }


        /// <summary>
        /// 取得一筆完整訊息(無完整訊息則為null)
        /// </summary>
        /// <returns></returns>
        public byte[] GetFinalMessage()
        {
            int iCount = 0;
            byte[] msg = null;
            if (m_FinalMessage != null)
            {
                iCount = m_FinalMessage.Count;
                if (iCount > 0)
                {
                    try
                    {
                        msg = m_FinalMessage.Dequeue();
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        msg = null;
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// 取得所有完整訊息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.Queue<byte[]> GetAllFinalMessage()
        {
            int iCount = 0;
            System.Collections.Generic.Queue<byte[]> oQueue = new System.Collections.Generic.Queue<byte[]>();
            if (m_FinalMessage != null)
            {
                iCount = m_FinalMessage.Count;
                for (int ii = 0; ii < iCount; ii++)
                {
                    try
                    {
                        oQueue.Enqueue(m_FinalMessage.Dequeue());
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        break;
                    }
                }
            }
            return oQueue;
        }


        /// <summary>
        /// 儲存完整訊息
        /// </summary>
        /// <param name="msg"></param>
        private int EnqueueFinalMessage(byte[] msg)
        {
            int iLength = 0;
            if (msg != null && m_FinalMessage != null)
            {
                iLength = msg.Length;
                if (iLength > 0)
                {
                    if (msg[0] != m_HeadByte || msg[iLength - 1] != m_EndByte)
                    {
                        ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"訊息切割出現異常，切割後頭尾與定義不符!! Head:" + System.Convert.ToString(msg[0], 16).ToUpper().PadLeft(2, '0') + " End:" + System.Convert.ToString(msg[iLength - 1], 16).ToUpper().PadLeft(2, '0'));
                        return -1;
                    }
                    else
                    {
                        m_FinalMessage.Enqueue(msg);
                        return 0;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 輸入訊息
        /// </summary>
        /// <param name="msg">byte[]訊息內容</param>
        public void InputMessage(byte[] msg)
        {
            InputMessage(msg, "");
        }

        /// <summary>
        /// 輸入訊息
        /// </summary>
        /// <param name="msg">byte[]訊息內容</param>
        /// <param name="source">訊息來源</param>
        public void InputMessage(byte[] msg, string source)
        {
            System.Text.StringBuilder oSB = null;
            System.Text.StringBuilder oSBFindHead = null;
            int iSIndex = 0;
            int iEIndex = 0;
            int iLength = 0;
            int iIndex = 0;
            iSIndex = -1;
            iEIndex = -1;
            byte[] bCompleteMsgs = null;

            try
            {
                if (msg != null && msg.Length > 0)
                {

                    lock (m_LockInput)
                    {
                        iLength = msg.Length;
                        if (iLength > 0)
                        {
                            foreach (byte item in msg)
                            {
                                if (m_ParsingType == 0x00)
                                {
                                    //找起始
                                    if (item == m_HeadByte)
                                    {
                                        m_ParsingType = 0x01;
                                        iSIndex = iIndex;
                                        iEIndex = -1;
                                        m_NoFinalMessage = null;
                                        if (this.m_IsDebugMode)
                                        {
                                            if (oSBFindHead != null && oSBFindHead.Length > 0)
                                            {
                                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"找不到起始byte ：" + oSBFindHead.ToString());
                                                oSBFindHead.Length = 0;
                                                oSBFindHead = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.m_IsDebugMode)
                                        {
                                            if (oSBFindHead == null)
                                            {
                                                oSBFindHead = new StringBuilder();
                                            }
                                            oSBFindHead.Append(System.Convert.ToString(item, 16).ToUpper().PadLeft(2, '0') + " ");
                                            if (oSBFindHead.Length >= 5000)
                                            {
                                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"找不到起始byte ：" + oSBFindHead.ToString());
                                                oSBFindHead.Length = 0;
                                            }
                                        }
                                    }
                                }
                                else if (m_ParsingType == 0x01)
                                {
                                    //找結尾
                                    if (item == m_EndByte)
                                    {
                                        //找到結尾
                                        m_ParsingType = 0x00;
                                        iEIndex = iIndex;
                                        if (iSIndex != -1)
                                        {
                                            bCompleteMsgs = new byte[iEIndex - iSIndex + 1];
                                            System.Array.Copy(msg, iSIndex, bCompleteMsgs, 0, bCompleteMsgs.Length);
                                        }
                                        else
                                        {
                                            bCompleteMsgs = new byte[iEIndex + 1];
                                            System.Array.Copy(msg, 0, bCompleteMsgs, 0, bCompleteMsgs.Length);
                                            if (m_NoFinalMessage != null)
                                            {
                                                bCompleteMsgs = m_NoFinalMessage.Concat<byte>(bCompleteMsgs).ToArray<byte>();
                                            }
                                        }
                                        m_NoFinalMessage = null;
                                        this.EnqueueFinalMessage(bCompleteMsgs);


                                        bCompleteMsgs = null;
                                        //位置歸0
                                        iSIndex = -1;
                                        iEIndex = -1;
                                    }
                                    else if (item == m_HeadByte)
                                    {
                                        //重複的起始
                                        try
                                        {
                                            if (this.m_IsDebugMode)
                                            {
                                                oSB = new StringBuilder();
                                                if (m_NoFinalMessage != null && m_NoFinalMessage.Length > 0)
                                                {
                                                    foreach (byte data in this.m_NoFinalMessage)
                                                    {
                                                        oSB.Append(System.Convert.ToString(data, 16).ToUpper().PadLeft(2, '0') + " ");
                                                    }
                                                }
                                                oSB.Append(System.Convert.ToString(item, 16).ToUpper().PadLeft(2, '0') + " ");
                                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"出現重複的起始byte ：" + oSB.ToString());
                                                oSB.Length = 0;
                                            }
                                        }
                                        catch (System.Exception ex)
                                        {
                                            ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                                        }
                                        finally
                                        {
                                            oSB = null;
                                            m_ParsingType = 0x01;
                                            iSIndex = iIndex;
                                            iEIndex = -1;
                                            m_NoFinalMessage = null;
                                        }
                                    }
                                }
                                iIndex++;
                            }

                            //若有不完整訊息則與之前所留不完整訊息合併
                            if (m_ParsingType == 0x01)
                            {
                                //尚在尋找結尾
                                if (iSIndex != -1)
                                {
                                    //表示有新起始
                                    bCompleteMsgs = new byte[iLength - iSIndex];
                                    System.Array.Copy(msg, iSIndex, bCompleteMsgs, 0, bCompleteMsgs.Length);
                                    m_NoFinalMessage = bCompleteMsgs;
                                    bCompleteMsgs = null;
                                }
                                else
                                {
                                    //表示不完整訊息內有舊起始  
                                    //將不完整訊息接在訊息的前端
                                    if (m_NoFinalMessage != null)
                                    {
                                        m_NoFinalMessage = m_NoFinalMessage.Concat<byte>(msg).ToArray<byte>();
                                    }
                                    else
                                    {
                                        m_NoFinalMessage = msg;
                                    }
                                    bCompleteMsgs = null;
                                }
                            }
                            else
                            {
                                //尚在尋找起始
                                m_NoFinalMessage = null;
                                iSIndex = -1;
                                iEIndex = -1;
                            }
                        }
                        else
                        {
                            if (m_IsDebugMode)
                            {
                                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"輸入訊息長度為0!!");
                            }
                        }
                    }
                }
                else
                {
                    if (m_IsDebugMode)
                    {
                        ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetComponentName(this),"輸入訊息為null!!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                //出現異常，復原至初始狀態
                m_NoFinalMessage = null;
                m_ParsingType = 0x00;
                iSIndex = -1;
                iEIndex = -1;
                throw new Exception(ex.Message + "\r\nStackTrace:" + ex.StackTrace);
            }
            finally
            {
                bCompleteMsgs = null;
                oSB = null;
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        {
            lock (m_LockInput)
            {
                if (m_FinalMessage != null)
                {
                    m_FinalMessage.Clear();
                    m_NoFinalMessage = null;
                    m_NoFinalMessage = new byte[0];
                    m_ParsingType = 0x00;
                }
            }

        }

        /// <summary>
        ///  釋放資源
        /// </summary>
        public void Dispose()
        {
            Clear();
            m_FinalMessage = null;
            m_NoFinalMessage = null;
        }
    }
}
