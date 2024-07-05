using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD
{
    public class DMD_API
    {
        private string mProcName = "DMD_API";

        private ASI.Lib.Comm.Socket.SocketLib mSocket = null;

        private string mSocketConnStr = "";

        private ASI.Lib.Msg.Parsing.ByteMessage mByteMessage = new Lib.Msg.Parsing.ByteMessage(0xAC, 0xA9);

        private System.Threading.Thread mThread = null;

        private bool mThreadRun = false;

        public delegate void ReceivedEventHandler(ASI.Wanda.DMD.Message.Message DMDmessage);
        /// <summary>
        /// 接收資料的事件
        /// </summary>
        public event ReceivedEventHandler ReceivedEvent;

        public delegate void OpenedEventHandler(string source);
        /// <summary>
        /// Socket Server專用，當Socket Server開啟的事件
        /// </summary>
        public event OpenedEventHandler OpenedEvent;

        public delegate void ClientConnectedEventHandler(string source);
        /// <summary>
        /// Socket Server專用，當Socket Client連接的事件
        /// </summary>
        public event ClientConnectedEventHandler ConnectedEvent;

        public delegate void DisconnectedEventHandler(string source);
        /// <summary>
        /// Socket Server專用，Socket Client斷線的事件
        /// </summary>
        public event DisconnectedEventHandler DisconnectedEvent;

        /// <summary>
        /// server端判斷 目前連線的client的IP以及Port
        /// </summary>
        private readonly Dictionary<int, string> clientIDList = new Dictionary<int, string>();

        /// <summary>
        /// 已經連線的Client列表 key=連線順序；value=IP:port
        /// </summary>
        public Dictionary<int, string> ClientIDList
        {
            get
            {
                if (mSocket != null)
                {
                    return clientIDList;
                }

                return new Dictionary<int, string>();
            }
        }

        /// <summary>
        /// API初始化
        /// </summary>
        /// <param name="connStr">存取通訊裝置之連線字串。例如，Server端設定："IP=192.168.0.1;Port=8000;Type=Server"；Client端設定："IP=192.168.0.1;Port=8000;Type=Client"</param>
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：未成功開啟；-3：剖析連線字串發生錯誤；-4：初始化 Socket 相關屬性發生錯誤；-5：關閉所有 Sockets 時發生錯誤；-6：Socket server 無法正常繫結通訊埠
        /// </returns> 
        public int Initial(string connStr)
        {
            var iRtn = 0;
            try
            {
                if (mThread != null) 
                {
                    //若為重新初始化，先停止原有執行緒 
                    mThreadRun = false;
                    System.Threading.Thread.Sleep(100);
                    mThread.Abort();
                    mThread = null;
                }

                mThreadRun = true;
                mThread = new System.Threading.Thread(new System.Threading.ThreadStart(MsgParsingThread));
                mThread.Start();

                mSocketConnStr = connStr;
                iRtn = SocketConnect(connStr);
                return iRtn;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// 送出訊息    
        /// </summary>
        /// <param name="DMDmessage">ASI.Wanda.DMD.Message.Message物件</param> 
        /// <returns></returns> 
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：轉成byte[]封包時發生錯誤；-3：未連線  
        /// </returns>
        public int Send(ASI.Wanda.DMD.Message.Message DMDmessage)
        {
            byte[] arrSendBytes = null;

            try
            {
                if (mSocket != null && mSocket.IsConnect)
                { 
                    arrSendBytes = ASI.Wanda.DMD.Message.Helper.Pack(DMDmessage);
                    if (arrSendBytes != null)
                    {
                        return mSocket.Send(arrSendBytes);
                    }
                    else
                    {
                        //轉成byte[]封包時發生錯誤
                        return -2;
                    }
                }
                else
                {
                    //未連線   
                    return -3;
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                return -1;
            }
        }
        /// <summary>
        /// 指定對象送出訊息(Server端專用) 
        /// </summary>
        /// <param name="CMFTmessage">ASI.Wanda.CMFT.Message.Message物件</param>
        /// <param name="target">指定傳送的Client對象IP:port</param>
        /// <returns></returns>
        public int Send(ASI.Wanda.DMD.Message.Message DMDmessage, string target)
        {
            byte[] arrSendBytes = null;

            try
            {
                if (mSocket != null && mSocket.IsConnect)
                {
                    arrSendBytes = ASI.Wanda.DMD.Message.Helper.Pack(DMDmessage);
                    if (arrSendBytes != null)
                    {
                        return mSocket.Send(arrSendBytes, target);
                    }
                    else
                    {
                        //轉成byte[]封包時發生錯誤 
                        return -2;
                    }
                }
                else
                {
                    //未連線
                    return -3;
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                return -1;
            }
        }

        /// <summary> 
        /// 中斷連線並停止處理執行緒
        /// </summary>
        /// <returns>
        /// 0：成功；-1：例外錯誤
        /// </returns>
        public int Dispose()
        {
            try
            {
                this.SocketDisConnect();
                mThreadRun = false;
                System.Threading.Thread.Sleep(100);
                return 0;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                return -1;
            }
        }

        private void MsgParsingThread()
        {
            Queue<byte[]> oRcvQueue = null;

            while (mThreadRun)
            {
                try
                {
                    lock (mByteMessage)
                    {
                        oRcvQueue = mByteMessage.GetAllFinalMessage();
                    }

                    while (mThreadRun &&
                        oRcvQueue.Count > 0)
                    {
                        try
                        {
                            byte[] arrRcvByte = oRcvQueue.Dequeue();
                            ASI.Wanda.DMD.Message.Message oDMDMsg = ASI.Wanda.DMD.Message.Helper.UnPack(arrRcvByte);
                            ReceivedEvent(oDMDMsg);
                        }
                        catch (System.Exception ex)
                        {
                            ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                }

                System.Threading.Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 開啟已設定之Socket通訊埠
        /// </summary>
        /// <param name="connStr">連線字串，例如："IP=192.168.0.1;Port=8000;Type=Server"</param>   
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：未成功開啟；-3：剖析連線字串發生錯誤；-4：初始化 Socket 相關屬性發生錯誤；-5：關閉所有 Sockets 時發生錯誤；-6：Socket server 無法正常繫結通訊埠
        /// </returns>
        private int SocketConnect(string connStr)
        {
            int iOpenResult = 0;
            try
            {
                mSocket = new Lib.Comm.Socket.SocketLib();

                mSocket.ConnectionString = connStr;
                mSocket.ReceivedEvent += Socket_ReceivedEvent;
                mSocket.DisconnectedEvent += Socket_DisconnectedEvent;
                mSocket.ErrorEvent += Socket_ErrorEvent;
                mSocket.OtherSideDisconnectEvent += Socket_OtherSideDisconnectEvent;

                ASI.Lib.Log.DebugLog.Log(mProcName, "Socket 嘗試連線，ConnectionString = " + mSocket.ConnectionString);

                iOpenResult = mSocket.Open();
                if (iOpenResult == 0 &&
                    mSocket.IsConnect)
                {
                    ASI.Lib.Log.DebugLog.Log(mProcName, "Socket 連線成功! ConnectionString = " + mSocket.ConnectionString);
                }
                else
                {   
                    ASI.Lib.Log.ErrorLog.Log(mProcName, string.Format("Socket 連線失敗! 失敗碼:{0} ； ConnectionString = {1}", iOpenResult, mSocket.ConnectionString));
                } 

                return iOpenResult;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
                return -1;
            }
        }

        private void SocketDisConnect()
        {
            if (mSocket != null)
            {
                mSocket.ReceivedEvent -= Socket_ReceivedEvent;
                mSocket.ErrorEvent -= Socket_ErrorEvent;
                mSocket.ConnectedEvent -= Socket_ConnectedEvent;
                mSocket.DisconnectedEvent -= Socket_DisconnectedEvent;
                mSocket.OtherSideDisconnectEvent -= Socket_OtherSideDisconnectEvent;
                mSocket.Close();
                mSocket = null;
            }
        }

        private void Socket_ReceivedEvent(byte[] dataBytes, string source)
        {
            try
            {
                if (dataBytes != null)
                {
                    lock (mByteMessage)
                    {
                        mByteMessage.InputMessage(dataBytes);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex.StackTrace);
            }
        }

        private void Socket_OtherSideDisconnectEvent(string source)
        {
            //對方正常斷線，嘗試重新連線
            ASI.Lib.Log.DebugLog.Log(mProcName, string.Format("{0}主動斷線，嘗試重新連線...", source));
            SocketDisConnect();
            SocketConnect(mSocketConnStr); 
        }

        private void Socket_OpenEvent(string source)
        {
            OpenedEvent(source);
        }
        
        private void Socket_ConnectedEvent(string source)
        {
            ConnectedEvent(source);
        }

        private void Socket_DisconnectedEvent(string source)
        {
            //DisconnectedEvent(source);
        }

        private void Socket_ErrorEvent(Exception exception)
        {

        }

    }
}
