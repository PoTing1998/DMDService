using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Messaging;

namespace ASI.Lib.Comm.MSMQ
{
    /// <summary>
    /// 操作MSMQ的Lib物件
    /// </summary>
    public class MsgQueLib : IDisposable
    {
        private string mHostName = System.Net.Dns.GetHostName();
        private System.Messaging.MessageQueue mQueue;
        private object mQueLock = new object();

        public int TimeOutInMilliSeconds = 10000;

        public MsgQueLib()
        {
            MessageQueue.EnableConnectionCache = false;
        }

        public static bool IsExist(string pFullQueName)
        {
            string sHostName = System.Net.Dns.GetHostName();
            string sPath = sHostName + "\\Private$\\" + pFullQueName;
            return MessageQueue.Exists(sPath);
        }

        public static int Create(string pComputer, string pFullQueName)
        {
            Delete(pFullQueName);

            string sHostName = System.Net.Dns.GetHostName();
            string sPath = sHostName + "\\Private$\\" + pFullQueName;

            try
            {
                MessageQueue oMsgQue = MessageQueue.Create(sPath);
                ASI.Lib.Log.LogFile.Log(pComputer, "ASI.Lib.Comm.MSMQ.QueueLib", $"建立MSMQ:{sPath}");

                //設定權限
                AccessControlList oAccessCtrlList = new AccessControlList();
                oAccessCtrlList.Add(new AccessControlEntry(new Trustee("Everyone"),
                                GenericAccessRights.All, StandardAccessRights.All,
                                AccessControlEntryType.Allow));
                oAccessCtrlList.Add(new AccessControlEntry(new Trustee("ANONYMOUS LOGON"),
                                GenericAccessRights.All, StandardAccessRights.All,
                                AccessControlEntryType.Allow));
                oMsgQue.SetPermissions(oAccessCtrlList);

                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(pFullQueName + " Create : " + ex.Message);
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Comm.MSMQ.MsgQueLib", ex);
            }
            return -1;
        }

        public int Open(string pIP, string pFullQueName, bool pDenySharedRead)
        {
            Close();

            try
            {
                string sPath = "\\Private$\\" + pFullQueName;

                if (pIP == ".")
                {
                    // local private queue
                    sPath = pIP + sPath;
                }
                else if (pIP == mHostName)
                {
                    sPath = mHostName + sPath;
                }
                else
                {
                    // remote private queue
                    sPath = "FormatName:DIRECT=TCP:" + pIP + sPath;
                }

                mQueue = new MessageQueue(sPath, pDenySharedRead);
                mQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                return 1;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), pFullQueName + " Open : " + ex.Message);
                mQueue = null;
            }
            return -1;
        }

        public int Open(string pFullQueName, bool pDenySharedRead)
        {
            return Open(mHostName, pFullQueName, false);
        }

        public int Open(string pFullQueName)
        {
            return Open(pFullQueName, false);
        }

        public void Close()
        {
            try
            {
                if (mQueue != null)
                {
                    mQueue.Close();
                    mQueue = null;
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
            }
        }

        public static void Delete(string pFullQueName)
        {
            string sHostName = System.Net.Dns.GetHostName();
            string sPath = sHostName + "\\Private$\\" + pFullQueName;

            try
            {
                if (MessageQueue.Exists(sPath))
                {
                    MessageQueue.Delete(sPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(pFullQueName + " Delete : " + ex.Message);
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Comm.MSMQ.QueueLib", pFullQueName + " Delete : " + ex.Message);
            }
        }

        public int Write(string pBody)
        {
            return Write(pBody, MessagePriority.Normal);
        }

        public int Write(string pBody, MessagePriority pPriority)
        {
            try
            {
                Message oMsg = new Message(pBody, new XmlMessageFormatter(new Type[] { typeof(string) }));
                oMsg.Priority = pPriority;
                mQueue.Send(oMsg, MessageQueueTransactionType.None);
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(mQueue.Path + " Write : " + ex.Message);

                if (mQueue != null)
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"無法寫入到[{mQueue.Path}]");
                }

                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }

        public int Read(out string pBody)
        {
            try
            {
                Message oMsg;

                lock (mQueLock)
                {
                    if (TimeOutInMilliSeconds > 0)
                    {
                        oMsg = mQueue.Receive(new TimeSpan(0, 0, 0, 0, TimeOutInMilliSeconds));
                    }
                    else
                    {
                        oMsg = mQueue.Receive();
                    }
                }

                pBody = (string)oMsg.Body.ToString();
                return 1;
            }
            catch (TimeoutException ex)
            {
                if (mQueue != null)
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"無法讀取:[{mQueue.Path}]");
                }

                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            pBody = "";
            return -1;
        }

        public int StartAsynReceive()
        {
            try
            {
                mQueue.ReceiveCompleted += new System.Messaging.ReceiveCompletedEventHandler(mQueue_ReceiveCompleted);
                if (TimeOutInMilliSeconds > 0)
                {
                    mQueue.BeginReceive(new TimeSpan(0, 0, 0, 0, TimeOutInMilliSeconds));
                }
                else
                {
                    mQueue.BeginReceive();
                }

                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return -1;
            }
        }

        public Message StopAsynReceive(IAsyncResult pResult)
        {
            try
            {
                mQueue.ReceiveCompleted -= new System.Messaging.ReceiveCompletedEventHandler(mQueue_ReceiveCompleted);
                Message oMsg = mQueue.EndReceive(pResult);
                return oMsg;
            }
            catch (Exception ex)
            {
                if (mQueue != null)
                {
                    Console.WriteLine(mQueue.Path + " StopAsynReceive : " + ex.Message);
                }
            }

            return null;
        }

        private void mQueue_ReceiveCompleted(object sender, System.Messaging.ReceiveCompletedEventArgs e)
        {
            try
            {
                //mQueue.ReceiveCompleted -= new System.Messaging.ReceiveCompletedEventHandler(mQueue_ReceiveCompleted);
                //MessageQueue aqueue = (MessageQueue)sender;
                //Message am = aqueue.EndReceive(e.AsyncResult);

                Message oMsg = StopAsynReceive(e.AsyncResult);
                if (oMsg != null)
                {
                    XmlMessageFormatter afm = new XmlMessageFormatter(new Type[] { typeof(String) });
                    OnReceived((string)afm.Read(oMsg));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
            }
            finally
            {
                StartAsynReceive();
            }
        }

        public delegate void MessageRceiveCompletedEventHandler(object sender, string pBody);
        public event MessageRceiveCompletedEventHandler AsyncReadCompleted;
        protected virtual void OnReceived(string pBody)
        {
            if (AsyncReadCompleted != null)
            {
                AsyncReadCompleted(this, pBody);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool pDisposing)
        {
            if (pDisposing)
            {
                Close();
            }
        }

        ~MsgQueLib()
        {
            Dispose(false);
        }
    }
}
