using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Queue
{
    /// <summary>
    /// 操作System.Collections.Generic.Queue的Lib物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueLib<T>
    {

        /// <summary>
        /// 容器超過最大筆數的處理方式
        /// </summary>
        private ASI.Lib.Queue.OverflowControlType m_OverflowControl = OverflowControlType.Clear;


        /// <summary>
        /// 訊息超過最大數量的事件
        /// </summary>
        public event ASI.Lib.Queue.OverflowEventHandler MessageOverflow;

        /// <summary>
        /// 是否停止工作
        /// </summary>
        private bool m_IsStop = false;

        /// <summary>
        /// 初始容量
        /// </summary>
        private int m_Capacity = 500;

        /// <summary>
        /// 最大限制容量
        /// </summary>
        private int m_MaxSize = 5000;

        /// <summary>
        /// 容器超過最大筆數的處理方式，預設為全部清除
        /// </summary>
        public ASI.Lib.Queue.OverflowControlType OverflowControl
        {
            get
            {
                return m_OverflowControl;
            }
            set
            {
                m_OverflowControl = value;
            }
        }


        /// <summary>
        /// 設定Queue的Size最大大小，0表示不限制(限制MaxSize必須大於10)
        /// </summary>
        public int MaxSize
        {
            set
            {
                if (value < 0)
                {
                    m_MaxSize = 0;
                }
                else if (value < 10)
                {
                    m_MaxSize = 10;
                }
                else
                {
                    m_MaxSize = value;
                }
            }
            get
            {
                return m_MaxSize;
            }
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public QueueLib()
        {
            m_DataList = new System.Collections.Generic.Queue<T>(m_Capacity);
            m_EventWaitHandles = new System.Threading.EventWaitHandle[2] { m_WaitEnque, m_StopWork };
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="size"></param>
        public QueueLib(int size)
        {
            if (size < 5)
            {
                m_Capacity = 5;
            }
            else
            {
                m_Capacity = size;
            }
            m_DataList = new System.Collections.Generic.Queue<T>(m_Capacity);
            m_EventWaitHandles = new System.Threading.EventWaitHandle[2] { m_WaitEnque, m_StopWork };
        }


        /// <summary>
        /// EventWaitHandle
        /// </summary>
        private System.Threading.EventWaitHandle m_WaitEnque = new System.Threading.AutoResetEvent(false);


        /// <summary>
        /// EventWaitHandle
        /// </summary>
        private System.Threading.EventWaitHandle m_StopWork = new System.Threading.AutoResetEvent(false);


        /// <summary>
        /// EventWaitHandle
        /// </summary>
        private System.Threading.EventWaitHandle[] m_EventWaitHandles = null;

        /// <summary>
        /// 中斷執行緒直到有資料Enque、TimeOut、StopWork後才執行
        /// </summary>
        /// <param name="msec">TimeOut時間長度(ms)，-1表示不TimeOut</param>
        /// <returns>當回傳值為System.Threading.WaitHandle.WaitTimeout則表示TimeOut</returns>
        public int WaitEnque(int msec)
        {
            if (m_IsStop)
            {
                System.Threading.WaitHandle.WaitAny(m_EventWaitHandles, 0, false);
            }
            else
            {
                System.Threading.WaitHandle.WaitAny(m_EventWaitHandles, msec, false);
            }
            return 0;
        }

        private object m_LockedReSet = new object();

        /// <summary>
        /// Dequeue時是否Locked
        /// </summary>
        private bool m_IsLockDequeue = true;

        /// <summary>
        /// Enqueue時是否Locked
        /// </summary>
        private bool m_IsLockEnqueue = true;

        /// <summary>
        /// 主要存放資料的物件
        /// </summary>
        System.Collections.Generic.Queue<T> m_DataList = null;

        /// <summary>
        /// 取得或設定Dequeue時是否Locked
        /// </summary>
        public bool IsLockDequeue
        {
            get
            {
                return m_IsLockDequeue;
            }
            set
            {
                m_IsLockDequeue = value;
            }
        }

        /// <summary>
        /// 重設物件
        /// </summary>
        private void ReSet()
        {
            lock (m_LockedReSet)
            {
                if (m_DataList == null)
                {
                    m_DataList = new System.Collections.Generic.Queue<T>(m_Capacity);
                }
            }
        }

        /// <summary>
        /// 停止工作
        /// (Queue的Wait TimeOut強制為0，立即TimeOut)
        /// </summary>
        public void StopWork()
        {
            m_IsStop = true;
            m_StopWork.Set();
        }

        /// <summary>
        /// 開始工作
        /// (Queue的Wait TimeOut由使用者自訂)
        /// </summary>
        public void StartWork()
        {
            m_IsStop = false;
            m_StopWork.Set();
        }


        /// <summary>
        /// 取得或設定Enqueue時是否Locked
        /// </summary>
        public bool IsLockEnqueue
        {
            get
            {
                return m_IsLockEnqueue;
            }
            set
            {
                m_IsLockEnqueue = value;
            }
        }

        /// <summary>
        /// 目前資料筆數
        /// </summary>
        public int Count
        {
            get
            {
                int iCount = 0;
                if (m_DataList != null)
                {
                    iCount = m_DataList.Count;
                }
                else
                {
                    ReSet();
                }
                return iCount;
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        {
            if (m_DataList != null)
            {
                if (m_IsLockDequeue)
                {
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        m_DataList.Clear();
                    }
                }
                else
                {
                    m_DataList.Clear();
                }
            }
            else
            {
                ReSet();
            }

        }

        /// <summary>
        /// 傳回一筆資料不移除
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (m_DataList != null)
            {
                T oItem;
                if (m_IsLockDequeue)
                {
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        if (m_DataList.Count > 0)
                        {
                            oItem = m_DataList.Peek();
                            return oItem;
                        }
                        else
                        {
                            throw new System.IndexOutOfRangeException("DataQueue物件Count = 0 無法Peek!!");
                        }
                    }
                }
                else
                {
                    if (m_DataList.Count > 0)
                    {
                        oItem = m_DataList.Peek();
                        return oItem;
                    }
                    else
                    {
                        throw new System.IndexOutOfRangeException("DataQueue物件Count = 0 無法Peek!!");
                    }
                }
            }
            else
            {
                ReSet();
                throw new System.IndexOutOfRangeException("DataQueue物件因不明原因變為null!!");
            }
        }

        /// <summary>
        /// 移除並傳回一筆資料
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (m_DataList != null)
            {
                T oItem;
                if (m_IsLockDequeue)
                {
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        if (m_DataList.Count > 0)
                        {
                            oItem = m_DataList.Dequeue();
                            return oItem;
                        }
                        else
                        {
                            throw new System.IndexOutOfRangeException("DataQueue物件Count = 0 無法Dequeue!!");
                        }
                    }
                }
                else
                {
                    if (m_DataList.Count > 0)
                    {
                        oItem = m_DataList.Dequeue();
                        return oItem;
                    }
                    else
                    {
                        throw new System.IndexOutOfRangeException("DataQueue物件Count = 0 無法Dequeue!!");
                    }
                }
            }
            else
            {
                ReSet();
                throw new System.IndexOutOfRangeException("DataQueue物件因不明原因變為null!!");
            }

        }

        /// <summary>
        /// 移除並傳回目前所有資料
        /// </summary>
        /// <returns></returns>
        public T[] DequeueAll()
        {
            T[] oItems = null;
            if (m_DataList != null)
            {
                if (m_IsLockDequeue)
                {
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        oItems = m_DataList.ToArray();
                        m_DataList.Clear();
                    }
                }
                else
                {
                    oItems = m_DataList.ToArray();
                    if (oItems != null && oItems.Length > 0)
                    {
                        foreach (T item in oItems)
                        {
                            m_DataList.Dequeue();
                        }
                    }
                }
            }
            else
            {
                ReSet();
            }
            return oItems;

        }

        /// <summary>
        /// 將目前資料轉成陣列
        /// </summary>
        public T[] ToArray()
        {
            T[] oList = null;
            if (m_DataList != null)
            {
                if (m_IsLockDequeue)
                {
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        oList = m_DataList.ToArray();
                    }
                }
                else
                {
                    oList = m_DataList.ToArray();
                }
            }
            else
            {
                ReSet();
                oList = new T[0];
            }
            return oList;
        }

        private void OnMessageOverflow(int maxsize, ASI.Lib.Queue.OverflowControlType overflowControl, int count, object message)
        {
            OverflowEventArgs oOverflowEventArgs = null;
            if (MessageOverflow != null)
            {
                oOverflowEventArgs = new OverflowEventArgs(maxsize, overflowControl, count, message);
                MessageOverflow(this, oOverflowEventArgs);
            }
            oOverflowEventArgs = null;
        }


        /// <summary>
        ///加入一筆資料至最後的位置
        /// </summary>
        /// <returns></returns>
        public void Enqueue(T item)
        {
            int iRemoveCount = 0;
            int iCount = 0;
            int iMaxSize = m_MaxSize;
            if (m_DataList == null)
            {
                ReSet();
            }

            if (m_IsLockEnqueue)
            {

                if (iMaxSize > 0)
                {
                    //限制Queue大小
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        iCount = m_DataList.Count;
                        iRemoveCount = (iCount + 1) - iMaxSize;
                        if (iRemoveCount > 0)
                        {
                            //超過最大值
                            if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.Clear)
                            {
                                m_DataList.Clear();
                                m_DataList.Enqueue(item);
                                OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                            }
                            else if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.Keep)
                            {
                                for (int ii = 0; ii < iRemoveCount; ii++)
                                {
                                    m_DataList.Dequeue();
                                }
                                m_DataList.Enqueue(item);
                                OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                            }
                            else if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.None)
                            {
                                m_DataList.Enqueue(item);
                                OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                            }
                        }
                        else
                        {
                            m_DataList.Enqueue(item);
                        }
                    }
                }
                else
                {
                    //不限制Queue大小
                    lock (((System.Collections.ICollection)m_DataList).SyncRoot)
                    {
                        m_DataList.Enqueue(item);
                    }
                }
                m_WaitEnque.Set();
            }
            else
            {
                if (iMaxSize > 0)
                {
                    iCount = m_DataList.Count;
                    iRemoveCount = iMaxSize - (iCount + 1);
                    if (iRemoveCount > 0)
                    {

                        //超過最大值
                        if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.Clear)
                        {
                            m_DataList.Clear();
                            m_DataList.Enqueue(item);
                            OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                        }
                        else if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.Keep)
                        {
                            for (int ii = 0; ii < iRemoveCount; ii++)
                            {
                                m_DataList.Dequeue();
                            }
                            m_DataList.Enqueue(item);
                            OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                        }
                        else if (m_OverflowControl == ASI.Lib.Queue.OverflowControlType.None)
                        {
                            m_DataList.Enqueue(item);
                            OnMessageOverflow(iMaxSize, m_OverflowControl, iCount, item);
                        }
                    }
                    else
                    {
                        m_DataList.Enqueue(item);
                    }
                }
                else
                {
                    m_DataList.Enqueue(item);
                }
                m_WaitEnque.Set();
            }

        }
    }

    public enum OverflowControlType
    {
        /// <summary>
        /// 超過最大值則清除
        /// </summary>
        Clear,

        /// <summary>
        /// 移除過多的資料，使其保持最大值
        /// </summary>
        Keep,

        /// <summary>
        /// 不進行任何動作
        /// </summary>
        None
    }

    /// <summary>
    /// Queue超過上線時的EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="fe"></param>
    public delegate void OverflowEventHandler(object sender, OverflowEventArgs fe);

    /// <summary>
    /// Queue超過上線時的EventArgs
    /// </summary>
    public class OverflowEventArgs : System.EventArgs
    {
        private int m_MaxSize = 0;
        private ASI.Lib.Queue.OverflowControlType m_OverflowControlType = OverflowControlType.None;
        private int m_Count = 0;
        private object m_Message = null;

        /// <summary>
        /// 設定的最大容量
        /// </summary>
        public int MaxSize
        {
            get
            {
                return m_MaxSize;
            }
        }

        /// <summary>
        /// 超過最大值的處理方式
        /// </summary>
        public ASI.Lib.Queue.OverflowControlType OverflowControl
        {
            get
            {
                return m_OverflowControlType;
            }
        }

        /// <summary>
        /// 資料筆數
        /// </summary>
        public int Count
        {
            get
            {
                return m_Count;
            }
        }


        /// <summary>
        /// 欲加入的資料
        /// </summary>
        public object Message
        {
            get
            {
                return m_Message;
            }
        }
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="maxsize"></param>
        /// <param name="overflowControl"></param>
        /// <param name="count"></param>
        /// <param name="message"></param>
        public OverflowEventArgs(int maxsize, ASI.Lib.Queue.OverflowControlType overflowControl, int count, object message)
        {
            m_MaxSize = maxsize;
            m_OverflowControlType = overflowControl;
            m_Count = count;
            m_Message = message;
        }


        /// <summary>
        ///  釋放資源
        /// </summary>
        public void Dispose()
        {
            m_Message = null;
        }

        /// <summary>
        /// 
        /// </summary>
        ~OverflowEventArgs()
        {
            m_Message = null;
        }
    }
}
