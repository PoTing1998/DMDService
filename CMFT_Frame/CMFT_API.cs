using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.CMFT
{
    /// <summary>
    /// CMFT API物件
    /// </summary>
    public class CMFT_API
    {
        private string mProcName = "CMFT_API";

        private ASI.Lib.Comm.Socket.SocketLib mSocket = null;

        private string mSocketConnStr = "";

        private ASI.Lib.Msg.Parsing.ByteMessage mByteMessage = new Lib.Msg.Parsing.ByteMessage(0xAC, 0xA9);

        private System.Threading.Thread mThread = null;

        private bool mThreadRun = false;

        /// <summary>
        /// 已經連線的Client列表 key=連線順序；value=IP:port
        /// </summary>
        public System.Collections.Generic.Dictionary<int, string> ClientIDList
        {
            get
            {
                if (mSocket != null)
                {
                    return new System.Collections.Generic.Dictionary<int, string>(mSocket.ClientIDList);
                }

                return new System.Collections.Generic.Dictionary<int, string>();
            }
        }

        /// <summary>
        /// 是否為 Socket Server 模式
        /// </summary>
        public bool IsServer
        {
            get { return mSocket != null && mSocket.Type == "Server"; }
        }

        /// <summary>
        /// 取得目前已連線 Client (IP:Port) 的清單（Server端專用）
        /// </summary>
        public System.Collections.Generic.List<string> GetClientEndpoints()
        {
            var oSocket = mSocket;
            if (oSocket == null) return new System.Collections.Generic.List<string>();
            return oSocket.GetClientEndpoints();
        }

        /// <summary>
        /// 強制斷開指定 Client（Server端專用），會觸發 DisconnectedEvent
        /// </summary>
        /// <param name="endpoint">Client 的 IP:Port</param>
        /// <returns>0：成功；-1：例外錯誤；-2：非 Server 模式；-3：找不到該 Client；-4：Socket 未建立</returns>
        public int DisconnectClient(string endpoint)
        {
            var oSocket = mSocket;
            if (oSocket == null) return -4;
            return oSocket.DisconnectClient(endpoint);
        }

        public delegate void ClientDataReceivedEventHandler(string source, int length);
        /// <summary>
        /// 收到原始資料時觸發（尚未組成完整訊息），source 為 IP:Port，供連線監控使用
        /// </summary>
        public event ClientDataReceivedEventHandler ClientDataReceivedEvent;

        public delegate void ReceivedEventHandler(ASI.Wanda.CMFT.Message.Message CMFTmessage);
        /// <summary>
        /// 接收資料的事件
        /// </summary>
        public event ReceivedEventHandler ReceivedEvent;

        public delegate void OpenedEventHandler(string source);
        /// <summary>
        /// Socket Server專用，當Socket Server開啟的事件
        /// </summary>
        public event OpenedEventHandler OpenedEvent;

        public delegate void ClosedEventHandler(string source);
        /// <summary>
        /// Socket Server專用，當Socket Server關閉的事件
        /// </summary>
        public event ClosedEventHandler ClosedEvent;

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

        public delegate void ErrorEventHandler(Exception exception);
        /// <summary>
        /// Socket發生錯誤的事件
        /// </summary>
        public event ErrorEventHandler ErrorEvent;

        /// <summary>
        /// 最近一次Socket錯誤。Initial/SocketConnect回傳-1時，由此取得實際的例外原因
        /// （例如Bind失敗：位址非本機IP、通訊埠已被占用）。
        /// </summary>
        public Exception LastError { get; private set; }

        /// <summary>
        /// 連線是否建立
        /// </summary>
        /// <returns></returns>
        public bool IsConnect
        {
            get
            {
                if (mSocket != null)
                {
                    return mSocket.IsConnect;
                }

                return false;
            }
        }

        /// <summary>
        /// API初始化，將連線Socket，並重啟收送執行緒
        /// </summary>
        /// <param name="connStr">連線字串，例如："IP=192.168.0.1;Port=8000;Type=Server"</param>
        /// <param name="connTo">連線對象，例如：HMI、DMD...</param>
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：未成功開啟；-3：剖析連線字串發生錯誤；-4：初始化 Socket 相關屬性發生錯誤；-5：關閉所有 Sockets 時發生錯誤；-6：Socket server 無法正常繫結通訊埠
        /// </returns>
        public int Initial(string connStr, string connTo)
        {
            int iRtn = 0;
            try
            {
                mProcName = $"CMFT_API_{connTo}";

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
                LastError = ex;
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                return -1;
            }
        }

        /// <summary>
        /// 送出訊息
        /// </summary>
        /// <param name="CMFTmessage">ASI.Wanda.CMFT.Message.Message物件</param>
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：轉成byte[]封包時發生錯誤；-3：未連線
        /// </returns>
        public int Send(ASI.Wanda.CMFT.Message.Message CMFTmessage)
        {
            byte[] arrSendBytes = null;

            try
            {
                if (mSocket != null && mSocket.IsConnect)
                {
                    arrSendBytes = ASI.Wanda.CMFT.Message.Helper.Pack(CMFTmessage);
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
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                return -1;
            }
        }

        /// <summary>
        /// 指定對象送出訊息(Server端專用)
        /// </summary>
        /// <param name="CMFTmessage">ASI.Wanda.CMFT.Message.Message物件</param>
        /// <param name="target">指定傳送的Client對象IP:port</param>
        /// <returns></returns>
        public int Send(ASI.Wanda.CMFT.Message.Message CMFTmessage,string target)
        {
            byte[] arrSendBytes = null;

            try
            {
                if (mSocket != null && mSocket.IsConnect)
                {
                    arrSendBytes = ASI.Wanda.CMFT.Message.Helper.Pack(CMFTmessage);
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
        /// 送出byte[]訊息
        /// </summary>
        /// <param name="byteArray">訊息內容，不可為null</param>
        /// <returns></returns>
        public int SendByte(byte[] byteArray)
        {
            try
            {
                if (mSocket != null && mSocket.IsConnect)
                {
                    if (byteArray != null)
                    {
                        return mSocket.Send(byteArray);
                    }
                    else
                    {
                        //byte[]為null
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
        /// 依據收到的訊息送出Ack
        /// </summary>
        /// <param name="CMFTmessage">收到的訊息</param>
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：轉成byte[]封包時發生錯誤；-3：未連線
        /// </returns>
        public int SendAck(ASI.Wanda.CMFT.Message.Message CMFTmessage)
        {
            ASI.Wanda.CMFT.Message.Message oAckMsg = new ASI.Wanda.CMFT.Message.Message();
            byte[] arrSendBytes = null;

            try
            {
                // TSAI 2023/01/10 Edit Start

                //oAckMsg.MessageType = 0x01;
                //oAckMsg.ID = CMFTmessage.ID;
                //CMFTmessage.LEN = null;
                //CMFTmessage.Content = null;
                //CMFTmessage.CRC16 = null;
                //if (mSocket != null && mSocket.IsConnect)
                //{
                //    arrSendBytes = ASI.Wanda.CMFT.Message.Helper.Pack(CMFTmessage);
                //    if (arrSendBytes != null)
                //    {
                //        return mSocket.Send(arrSendBytes);
                //    }
                //    else
                //    {
                //        //轉成byte[]封包時發生錯誤
                //        return -2;
                //    }
                //}
                //else
                //{
                //    //未連線
                //    return -3;
                //}

                oAckMsg.MessageType = ASI.Wanda.CMFT.Message.Message.eMessageType.Ack;
                oAckMsg.ID = CMFTmessage.ID;
                oAckMsg.LEN = null;
                oAckMsg.Content = null;
                oAckMsg.CRC16 = null;

                if (mSocket != null && mSocket.IsConnect)
                {
                    arrSendBytes = ASI.Wanda.CMFT.Message.Helper.Pack(oAckMsg);
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


                // TSAI 2023/01/10 Edit End
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
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                return -1;
            }
        }

        private void MsgParsingThread()
        {
            Queue<byte[]> oRcvQueue = null;
            System.DateTime lastCheckConnTime = System.DateTime.Now;

            while (mThreadRun)
            {
                try
                {
                    //Socket Client 每10秒檢查是否已斷線，若斷線則自動重連                    
                    if (mSocket != null && mSocket.Type == "Client")
                    {
                        if (System.DateTime.Now.Subtract(lastCheckConnTime).TotalSeconds >= 10) 
                        {
                            if (!mSocket.IsConnect)
                            {
                                SocketDisConnect();
                                SocketConnect(mSocketConnStr);
                            }
                        }
                    }

                    lock (mByteMessage)
                    {
                        oRcvQueue = mByteMessage.GetAllFinalMessage();
                    }

                    while (mThreadRun &&
                        oRcvQueue.Count > 0)
                    {
                        byte[] arrRcvByte = null;
                       // arrRcvByte = oRcvQueue.Dequeue();
                       // ASI.Wanda.CMFT.Message.Message oCMFTMsg = ASI.Wanda.CMFT.Message.Helper.UnPack(arrRcvByte);
                        try
                        {
                            arrRcvByte = oRcvQueue.Dequeue();
                            ASI.Wanda.CMFT.Message.Message oCMFTMsg = ASI.Wanda.CMFT.Message.Helper.UnPack(arrRcvByte);
                            ReceivedEvent(oCMFTMsg);
                        }
                        catch (System.Exception ex)
                        {

                            ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
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
                LastError = null;
                mSocket = new Lib.Comm.Socket.SocketLib();

                mSocket.ConnectionString = connStr;
                mSocket.ReceivedEvent += Socket_ReceivedEvent;
                mSocket.ErrorEvent += Socket_ErrorEvent;                

                if (mSocket.Type == "Server")
                {
                    mSocket.OpenEvent += Socket_OpenEvent;
                    mSocket.CloseEvent += Socket_CloseEvent;
                    mSocket.ConnectedEvent += Socket_ConnectedEvent;
                    mSocket.DisconnectedEvent += Socket_DisconnectedEvent;                    

                    ASI.Lib.Log.DebugLog.Log(mProcName, "Socket Server嘗試開啟，ConnectionString = " + mSocket.ConnectionString);
                    iOpenResult = mSocket.Open();
                    if (iOpenResult == 0 &&
                        mSocket.IsConnect)
                    {
                        ASI.Lib.Log.DebugLog.Log(mProcName, "Socket Server開啟成功! ConnectionString = " + mSocket.ConnectionString);
                    }
                    else
                    {
                        ASI.Lib.Log.DebugLog.Log(mProcName, $"Socket Server開啟失敗! 失敗碼:{iOpenResult} ； ConnectionString = {mSocket.ConnectionString} ； 錯誤訊息 = {LastError?.Message}");
                    }
                }
                else
                {
                    //Socket Client
                    mSocket.OtherSideDisconnectEvent += Socket_OtherSideDisconnectEvent;

                    ASI.Lib.Log.DebugLog.Log(mProcName, "Socket Client嘗試連線，ConnectionString = " + mSocket.ConnectionString);
                    iOpenResult = mSocket.Open();
                    if (iOpenResult == 0 &&
                        mSocket.IsConnect)
                    {
                        ASI.Lib.Log.DebugLog.Log(mProcName, "Socket 連線成功! ConnectionString = " + mSocket.ConnectionString);
                    }
                    else
                    {
                        ASI.Lib.Log.DebugLog.Log(mProcName, $"Socket 連線失敗! 失敗碼:{iOpenResult} ； ConnectionString = {mSocket.ConnectionString} ； 錯誤訊息 = {LastError?.Message}");
                    }
                }

                return iOpenResult;
            }
            catch (System.Exception ex)
            {
                LastError = ex;
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
                return -1;
            }
        }

        private void SocketDisConnect()
        {
            if (mSocket != null)
            {
                mSocket.ReceivedEvent -= Socket_ReceivedEvent;
                mSocket.ErrorEvent -= Socket_ErrorEvent;
                if (mSocket.Type == "Server")
                {
                    mSocket.OpenEvent -= Socket_OpenEvent;
                    mSocket.CloseEvent -= Socket_CloseEvent;
                    mSocket.ConnectedEvent -= Socket_ConnectedEvent;
                    mSocket.DisconnectedEvent -= Socket_DisconnectedEvent;
                }
                else
                {
                    mSocket.OtherSideDisconnectEvent -= Socket_OtherSideDisconnectEvent;
                }

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
                    ClientDataReceivedEvent?.Invoke(source, dataBytes.Length);
                    lock (mByteMessage)
                    {
                        mByteMessage.InputMessage(dataBytes);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
            }
        }

        private void Socket_OtherSideDisconnectEvent(string source)
        {
            //對方正常斷線，嘗試重新連線
            ASI.Lib.Log.DebugLog.Log(mProcName, $"{source}主動斷線，嘗試重新連線...");
            SocketDisConnect();
            SocketConnect(mSocketConnStr);
        }

        private void Socket_OpenEvent(string source)
        {
            OpenedEvent?.Invoke(source);
        }
        private void Socket_ConnectedEvent(string source)
        {
            ConnectedEvent?.Invoke(source);
        }
        private void Socket_DisconnectedEvent(string source)
        {
            DisconnectedEvent?.Invoke(source);
        }

        private void Socket_ErrorEvent(Exception exception)
        {
            LastError = exception;
            ASI.Lib.Log.ErrorLog.Log(mProcName, exception);
            ErrorEvent?.Invoke(exception);
        }
        private void Socket_CloseEvent(string source)
        {
            ClosedEvent?.Invoke(source);
        }

    }
}