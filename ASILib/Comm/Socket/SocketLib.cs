using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm.Socket
{
    /// <summary>
    /// 用以處理 Socket 之通訊介面。
    /// </summary>
    public class SocketLib :
        ASI.Lib.Comm.ReceivedEvents,
        ASI.Lib.Comm.ICommunication
    {
        #region =====[Private]Sub-class=====

        /// <summary>
        /// 資料封包的定義與操作方式。
        /// </summary>
        private class Packet
        {
            #region =====[Public]Property=====

            #region CurrentSocket//System.Net.Sockets.Socket
            /// <summary>
            /// 取得或設定目前作用中的 Socket。
            /// </summary>
            public System.Net.Sockets.Socket CurrentSocket
            {
                get
                {
                    return this.m_CurrentSocket;
                }
                set
                {
                    this.m_CurrentSocket = value;
                }
            }
            #endregion

            #region ClientID//int
            /// <summary>
            /// 取得或設定此 Socket clinet 的 ID。
            /// </summary>
            public int ClientID
            {
                get
                {
                    return this.m_ClientID;
                }
                set
                {
                    this.m_ClientID = value;
                }
            }
            #endregion

            #region Buffers//byte[]
            /// <summary>
            /// 取得或設定接收資料的緩衝區。
            /// </summary>
            public byte[] Buffers
            {
                get
                {
                    return this.m_MsgBuffers;
                }
                set
                {
                    this.m_MsgBuffers = value;
                }
            }
            #endregion

            #endregion


            #region =====[Private]Variable=====

            /// <summary>
            /// Socket物件
            /// </summary>
            private System.Net.Sockets.Socket m_CurrentSocket;

            /// <summary>
            /// Client編號
            /// </summary>
            private int m_ClientID;

            /// <summary>
            /// 訊息長度
            /// </summary>
            private byte[] m_MsgBuffers = new byte[1024];

            #endregion


            #region =====(Built-in Operation)=====

            #region Packet(System.Net.Sockets.Socket currentSocket, int clientID)
            /// <summary>
            /// 類別建構子，Socket Server用。
            /// </summary>
            /// <param name="currentSocket">Socket Server.</param>
            /// <param name="clientID">Client's ID.</param>
            public Packet(System.Net.Sockets.Socket currentSocket, int clientID)
            {
                this.m_CurrentSocket = currentSocket;
                this.m_ClientID = clientID;
            }
            #endregion

            #region Packet()
            /// <summary>
            /// 類別建構子，Socket Client用。
            /// </summary>
            public Packet()
            {
                // 不處理。
            }

            /// <summary>
            /// 類別解構子。
            /// </summary>
            ~Packet()
            {
                this.m_CurrentSocket = null;
                this.m_MsgBuffers = null;
            }

            #endregion

            #endregion
        }

        #endregion


        #region =====[Public]Event=====

        /// <summary>
        /// 用以接收資料的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler ReceivedEvent;

        /// <summary>
        /// 當 Socket client 連接後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ConnectedEventHandler ConnectedEvent;

        /// <summary>
        /// 當通訊發生錯誤後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.ErrorEventHandlers ErrorEvent;

        /// <summary>
        /// 當 Socket clinet 斷線後的事件宣告。
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler DisconnectedEvent;

        /// <summary>
        /// Trigger on Server Opened (For Socket Server only).
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.OpenEventHandlers OpenEvent;

        /// <summary>
        /// Trigger on Server Closed (For Socket Server only).
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.CloseEventHandlers CloseEvent;

        /// <summary>
        /// 對方主動斷線事件
        /// </summary>
        public event ASI.Lib.Comm.ReceivedEvents.OtherSideDisconnectEventHandler OtherSideDisconnectEvent;

        #endregion


        #region =====[Private]Default Event Body=====

        /// <summary>
        /// 預設處理 ReceivedEvent 之方法。
        /// </summary>
        /// <param name="dataBytes">Received data in bytes.</param>
        /// <param name="source">Source of dataBytes.</param>
        void Socket_ReceivedEvent(byte[] dataBytes, string source)
        {
            try
            {                
                if (dataBytes != null)
                {
                    if (dataBytes.Length == 0)
                    {
                        //對方正常斷線
                        if (OtherSideDisconnectEvent != null)
                        {
                            OtherSideDisconnectEvent(source);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }

        /// <summary>
        /// 預設處理 ErrorEvent 之方法。
        /// </summary>
        /// <param name="exception">Exception.</param>
        void Socket_ErrorEvent(Exception exception)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }

        /// <summary>
        /// 預設處理 ConnectedEvent 之方法。
        /// </summary>
        /// <param name="source">Connected target.</param>
        void Socket_ConnectedEvent(string source)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }

        /// <summary>
        /// 預設處理 DisconnectedEvent 之方法。
        /// </summary>
        /// <param name="source">Disconnected target.</param>
        void Socket_DisconnectedEvent(string source)
        {
            try
            {
                // 無處理。
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }

        /// <summary>
        /// Extended event process of the ErrorEvent.
        /// </summary>
        /// <param name="ex">Exception</param>
        private void OnErrorEvent(System.Exception ex)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEvent(ex);
            }
            else
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"ErrorEvent is null! ex=[{ex}].");
            }
        }

        /// <summary>
        /// Extended event process of the ErrorEvent.
        /// </summary>
        /// <param name="source">Source of the ErrorEvent.</param>
        private void OnErrorEvent(string source)
        {
            System.Exception oEX;

            if (this.ErrorEvent != null)
            {
                oEX = new Exception(source);
                this.ErrorEvent(oEX);
            }
            else
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"ErrorEvent is null! source=[{source}].");
            }
        }

        /// <summary>
        /// Server opened the socket.
        /// </summary>
        /// <param name="source">Source of the opened server.</param>
        private void OnOpenEvent(string source)
        {
            if (this.m_Type == 1)
            {
                if (this.OpenEvent != null)
                {
                    this.OpenEvent(source);
                }
                else
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"OnOpenEvent is null! source=[{source}].");
                }
            }
        }

        /// <summary>
        /// Extended event process of the CloseEvent.
        /// </summary>
        /// <param name="source">Source of the closed server.</param>
        private void OnCloseEvent(string source)
        {
            if (this.m_Type == 1)
            {
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(source);
                }
                else
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"OnCloseEvent is null! source=[{source}].");
                }
            }
        }

        /// <summary>
        /// Extended event process of the DisconnectedEvent.
        /// </summary>
        /// <param name="source">Source of the disconnected socket.</param>
        private void OnDisconnectedEvent(string source)
        {
            if (this.DisconnectedEvent != null)
            {
                this.DisconnectedEvent(source);
            }
            else
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"DisconnectedEvent is null! source=[{source}].");
            }
        }

        /// <summary>
        /// Extended event process of the ConnectedEvent.
        /// </summary>
        /// <param name="source">Source of the connected socket.</param>
        private void OnConnectedEvent(string source)
        {
            if (this.ConnectedEvent != null)
            {
                this.ConnectedEvent(source);
            }
            else
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), $"OnConnectedEvent is null! source=[{source}].");
            }
        }

        #endregion


        #region =====[Private]Variable=====

        /// <summary>
        /// 使用 .NET 提供的 Socket 物件進行操作。
        /// </summary>
        private System.Net.Sockets.Socket m_Socket = null;

        /// <summary>
        /// 通訊裝置的連線字串。
        /// 例如："IP=192.168.5.231;Port=8000;Type=Server"。
        /// </summary>
        private string m_ConnectionString;

        /// <summary>
        /// 與 Socket 對應的參數。
        /// </summary>
        private System.Net.IPAddress m_IPAddress;

        /// <summary>
        /// IPAddress
        /// </summary>
        private string m_IPAddressString;

        /// <summary>
        /// Port
        /// </summary>
        private int m_Port;

        /// <summary>
        /// 用以辨別 Socket 是 (1)server 或 (2)client。
        /// </summary>
        private int m_Type;

        /// <summary>
        /// Object to control the socket lock.
        /// </summary>
        private object m_LockedControlSocket = new object();

        /// <summary>
        /// 計算已有多少 client 進行連接，
        /// 使用安全執行緒方式進行遞增，
        /// 主要可用於賦予每個 clinet 一個 ID。
        /// </summary>
        private int m_ClientConnectedCount = 0;

        /// <summary>
        /// 用於記錄正連接的 Socket clients。
        /// </summary>
        private System.Collections.Generic.Dictionary<string, System.Net.Sockets.Socket> m_ClientLists =
            new System.Collections.Generic.Dictionary<string, System.Net.Sockets.Socket>();

        /// <summary>
        /// 用於記錄正連接的 Socket client IDs。
        /// </summary>
        private System.Collections.Generic.Dictionary<int, string> m_ClientListIDs =
            new System.Collections.Generic.Dictionary<int, string>();

        /// <summary>
        /// 記錄非同步接收資料的 callback function。
        /// </summary>
        private System.AsyncCallback m_OnSocketBeginReceiveCallback = null;

        /// <summary>
        /// 最後一次紀錄連線失敗的時間
        /// </summary>
        private System.DateTime m_LastLogConnectErr = System.DateTime.Now;

        /// <summary>
        /// 重新嘗試連線連續失敗的次數
        /// </summary>
        private long m_KeepConnectErr = 0;

        /// <summary>
        /// The socket is started.
        /// </summary>
        private bool m_IsStart = false;

        /// <summary>
        /// Thread to keep connection.
        /// </summary>
        private System.Threading.Thread m_KeepThread = null;

        /// <summary>
        /// 當對方主動斷線後是否自動重新連線
        /// </summary>
        //private bool m_AutoReconnect = false;

        #endregion


        #region =====[Public]Property=====

        /// <summary>
        /// 存取通訊裝置之連線字串。例如："IP=192.168.5.231;Port=8000;Type=Server"。
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.m_ConnectionString;
            }
            set
            {
                lock (this.m_LockedControlSocket)
                {
                    if (!this.m_IsStart)
                    {
                        this.m_ConnectionString = value;
                        this.ParseConnectString();
                    }
                }
            }
        }

        /// <summary>
        /// 設定要繫結的 IP 位址。
        /// </summary>
        public string IPAddress
        {
            get
            {
                return this.m_IPAddressString;
            }
            set
            {
                lock (this.m_LockedControlSocket)
                {
                    if (!this.m_IsStart)
                    {
                        this.m_IPAddressString = value;
                        this.ComposeConnectionString();
                    }
                }
            }
        }

        /// <summary>
        /// 設定要繫結的 Port。
        /// </summary>
        public int Port
        {
            get
            {
                return this.m_Port;
            }
            set
            {

                lock (this.m_LockedControlSocket)
                {
                    if (!this.m_IsStart)
                    {
                        this.m_Port = value;
                        this.ComposeConnectionString();
                    }
                }
            }
        }

        /// <summary>
        /// 設定 Socket 的種類；Server or Client。
        /// </summary>
        public string Type
        {
            get
            {
                if (this.m_Type == 1)
                {
                    return "Server";
                }
                else if (this.m_Type == 2)
                {
                    return "Client";
                }

                return null;
            }
            set
            {
                lock (this.m_LockedControlSocket)
                {
                    if (!this.m_IsStart)
                    {
                        if (value.ToUpper() == "Server".ToUpper())
                        {
                            this.m_Type = 1;
                        }
                        else if (value.ToUpper() == "Client".ToUpper())
                        {
                            this.m_Type = 2;
                        }
                        this.ComposeConnectionString();
                    }
                }
            }
        }

        /// <summary>
        ///連線是否建立 
        /// </summary>
        public bool IsConnect
        {
            get
            {
                lock (this.m_LockedControlSocket)
                {
                    if (this.m_Type == 1)
                    {
                        if (this.m_Socket != null && this.SocketIsOpen(this.m_Socket) == 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (this.m_Socket != null && this.SocketIsConnect(this.m_Socket) == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public System.Collections.Generic.Dictionary<string, System.Net.Sockets.Socket> ClientList
        {
            get 
            {
                return this.m_ClientLists;
            }
        }

        public System.Collections.Generic.Dictionary<int,string> ClientIDList
        {
            get
            {
                return this.m_ClientListIDs;
            }
        }

        #endregion


        #region =====[Public]Method=====

        #region Open()//int
        /// <summary>
        /// 開啟已設定之的通訊埠。
        /// </summary>
        /// <returns>
        /// 0：成功；-1：例外錯誤；-2：未成功開啟；-3：剖析連線字串發生錯誤；-4：初始化 Socket 相關屬性發生錯誤；-5：關閉所有 Sockets 時發生錯誤；-6：Socket server 無法正常繫結通訊埠
        /// </returns>
        public int Open()
        {
            int iRtn = 0;

            try
            {
                lock (this.m_LockedControlSocket)
                {
                    iRtn = OpenSocket();

                    Start();
                }

                return iRtn;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region Close()//int
        /// <summary>
        /// 將通訊埠關閉。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：關閉 Sockets 時發生錯誤。</returns>
        public int Close()
        {
            try
            {
                // 關閉本物件所有的 Sockets。
                this.Stop();

                string sSource = this.m_IPAddress.ToString() + ":" + this.m_Port.ToString();
                this.OnCloseEvent(sSource);

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region Send(string dataText)//int
        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataText"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：本機端 Socket 未準備好進行通訊。</returns>
        /// <returns>-3：遠端 Socket 未準備好進行通訊。</returns>
        public int Send(string dataText)
        {
            try
            {
                // 辨別 server 或 client，決定須送出資料之目標。
                if (this.m_Type == 1)
                {
                    // 若為 server，則送出資料至所有的 client。
                    foreach (System.Net.Sockets.Socket socket in this.m_ClientLists.Values)
                    {
                        if (SocketIsOpen(socket) == 0)
                        {
                            byte[] byteBuffers = System.Text.Encoding.UTF8.GetBytes(dataText);
                            socket.Send(byteBuffers);
                        }
                    }
                }
                else if (this.m_Type == 2)
                {
                    // 若為 client，則送出資料至 server。
                    if (SocketIsOpen(this.m_Socket) == 0)
                    {
                        byte[] byteBuffers = System.Text.Encoding.UTF8.GetBytes(dataText);
                        this.m_Socket.Send(byteBuffers);
                    }
                    else
                    {
                        return -2; 
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region ParseTarget(string target)//string[]
        /// <summary>
        /// Parse target IP and Port.
        /// </summary>
        /// <param name="target">Target IP:Port.</param>
        /// <returns>If success, return parsed target IP(array index=0) and Port(array index=1), or return null if failed.</returns>
        public string[] ParseTarget(string target)
        {
            try
            {
                string[] sTargetIDs = new string[] { null, null };

                sTargetIDs = target.Split(new char[] { ':' }, 2);

                // Check parsed IP and Port.
                System.Net.IPAddress oAddr = null;
                if (!System.Net.IPAddress.TryParse(sTargetIDs[0], out oAddr))
                {
                    sTargetIDs[0] = null;
                }

                int iPort = -1;
                if (!int.TryParse(sTargetIDs[1], out iPort))
                {
                    sTargetIDs[1] = null;
                }

                if (sTargetIDs[0] == null &&
                    sTargetIDs[1] == null)
                {
                    sTargetIDs = null;
                }

                return sTargetIDs;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return null;
            }
        }
        #endregion

        #region Send(string dataText, string target)//int
        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataText"></param>
        /// <param name="target"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：本機端 Socket 未準備好進行通訊。</returns>
        /// <returns>-3：遠端 Socket 未準備好進行通訊。</returns>
        public int Send(string dataText, string target)
        {
            try
            {
                // 辨別 server 或 client，決定須送出資料之目標。
                if (this.m_Type == 1)
                {
                    // 若為 server，則送出資料至指定的 client。
                    string[] sTargetSocketParseds = this.ParseTarget(target);

                    // Verified target.
                    if (sTargetSocketParseds != null &&
                        sTargetSocketParseds[0] != null &&
                        sTargetSocketParseds[1] != null)
                    {
                        System.Net.Sockets.Socket socket =
                            this.m_ClientLists[$"{sTargetSocketParseds[0]}:{sTargetSocketParseds[1]}"];

                        if (SocketIsOpen(socket) == 0)
                        {
                            byte[] byteBuffers = System.Text.Encoding.UTF8.GetBytes(dataText);
                            socket.Send(byteBuffers);
                        }
                        else
                        {
                            this.OnErrorEvent($"Target=[{target}] is not open.");
                        }
                    }
                    else
                    {
                        this.OnErrorEvent($"Target=[{target}] is not valid.");
                    }
                }
                else if (this.m_Type == 2)
                {
                    // 若為 client，則送出資料至 server。
                    return Send(dataText);
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region Send(byte[] dataBytes)//int
        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：本機端 Socket 未準備好進行通訊。</returns>
        public int Send(byte[] dataBytes)
        {
            try
            {
                // 辨別 server 或 client，決定須送出資料之目標。
                if (this.m_Type == 1)
                {
                    // 若為 server，則送出資料至所有的 client。
                    foreach (System.Net.Sockets.Socket socket in this.m_ClientLists.Values)
                    {
                        try
                        {
                            if (SocketIsOpen(socket) == 0)
                            {
                                socket.Send(dataBytes);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            this.OnErrorEvent(ex);
                        }
                    }
                }
                else if (this.m_Type == 2)
                {
                    // 若為 client，則送出資料至 server。
                    if (SocketIsOpen(this.m_Socket) == 0)
                    {
                        this.m_Socket.Send(dataBytes);
                    }
                    else
                    {
                        return -2;
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region Send(byte[] dataBytes, string target)//int
        /// <summary>
        /// 送出資料至已開啟的通訊埠。
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="target"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：本機端 Socket 未準備好進行通訊。</returns>
        /// <returns>-3：遠端 Socket 未準備好進行通訊。</returns>
        public int Send(byte[] dataBytes, string target)
        {
            try
            {
                // 辨別 server 或 client，決定須送出資料之目標。
                if (this.m_Type == 1)
                {
                    // 若為 server，則送出資料至指定的 client。
                    string[] sTargetSocketParseds = this.ParseTarget(target);

                    // Verified target.
                    if (sTargetSocketParseds != null &&
                        sTargetSocketParseds[0] != null &&
                        sTargetSocketParseds[1] != null)
                    {
                        System.Net.Sockets.Socket socket =
                            this.m_ClientLists[$"{sTargetSocketParseds[0]}:{sTargetSocketParseds[1]}"];

                        if (SocketIsOpen(socket) == 0)
                        {
                            socket.Send(dataBytes);
                        }
                        else
                        {
                            this.OnErrorEvent($"Target=[{target}] is not open.");
                        }
                    }
                    else
                    {
                        this.OnErrorEvent($"Target=[{target}] is not valid.");
                    }
                }
                else if (this.m_Type == 2)
                {
                    // 若為 client，則送出資料至 server。
                    return Send(dataBytes);
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #endregion


        #region =====(Built-in Operation)=====

        #region Socket()
        /// <summary>
        /// 類別建構子
        /// </summary>
        public SocketLib()
        {
            try
            {
                // 指定事件之委派常式。
                ReceivedEvent += new ReceivedEventHandler(Socket_ReceivedEvent);
                ConnectedEvent += new ConnectedEventHandler(Socket_ConnectedEvent);
                ErrorEvent += new ErrorEventHandlers(Socket_ErrorEvent);
                DisconnectedEvent += new DisconnectedEventHandler(Socket_DisconnectedEvent);
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion

        #region ~Socket()
        /// <summary>
        /// 類別解構子
        /// </summary>
        ~SocketLib()
        {
            try
            {
                // 關閉 Sockets。
                Close();

                // 刪除事件之委派常式。
                ReceivedEvent = null;
                ConnectedEvent = null;
                ErrorEvent = null;
                DisconnectedEvent = null;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion 

        #endregion


        #region =====[Private]Thread=====

        #region KeepThread(object target)
        /// <summary>
        /// 監視並保持連線
        /// </summary>
        private void KeepThread(object target)
        {
            string sConnectionName = null;
            bool bIsClose = false;
            bool bNeedReconn = false;

            if (SocketIsOpen(this.m_Socket) != 0)
            {
                bIsClose = true;
            }

            while (this.m_IsStart)
            {
                try
                {
                    lock (this.m_LockedControlSocket)
                    {
                        if (!this.m_IsStart)
                        {
                            break;
                        }

                        if (!bNeedReconn)
                        {
                            System.Threading.Thread.Sleep(100);
                            continue;
                        }

                        sConnectionName = this.m_IPAddressString + ":" + this.m_Port;

                        // 檢查是否成功開啟 Socket。
                        if (!IsConnect)
                        {
                            if (!bIsClose)
                            {
                                if (this.m_Type == 2)
                                {
                                    this.OnDisconnectedEvent(sConnectionName);
                                }
                            }

                            if (this.OpenSocket() < 0)
                            {
                                //開啟失敗必須重連
                                bNeedReconn = true;

                                //開啟失敗時先暫停3秒再重試
                                System.Threading.Thread.Sleep(3000);
                                continue;
                            }
                            else
                            {
                                //開啟成功則無須再重連
                                bNeedReconn = false;
                            }

                            if (!IsConnect)
                            {
                                bIsClose = true;
                                if (this.m_KeepConnectErr >= long.MaxValue)
                                {
                                    this.m_KeepConnectErr = 1;
                                }
                                else
                                {
                                    this.m_KeepConnectErr++;
                                }

                                this.m_LastLogConnectErr = System.DateTime.Now;
                                this.OnErrorEvent(sConnectionName);
                            }
                            else
                            {
                                if (bIsClose)
                                {
                                    this.m_KeepConnectErr = 0;
                                }

                                bIsClose = false;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    this.OnErrorEvent("發生錯誤 " + sConnectionName + " 無法正常連接!! 已連續嘗試 " + this.m_KeepConnectErr.ToString() + " 次!!");
                    this.OnErrorEvent(ex);
                }

                System.Threading.Thread.Sleep(3000);
            }

            System.Threading.Thread.Sleep(100);
            this.m_KeepThread = null;
        }
        #endregion

        #endregion


        #region =====[Private]Function=====

        #region Keep()//void
        /// <summary>
        /// 維持連線或開啟
        /// </summary>
        private void Keep()
        {
            if (this.m_IsStart)
            {
                System.Threading.ParameterizedThreadStart oParameterizedThreadStart = new System.Threading.ParameterizedThreadStart(this.KeepThread);
                this.m_KeepThread = new System.Threading.Thread(oParameterizedThreadStart);
                this.m_KeepThread.IsBackground = true;
                this.m_KeepThread.Start();
                oParameterizedThreadStart = null;
            }
        }
        #endregion

        #region Start()//int
        /// <summary>
        /// 啟動
        /// </summary>
        private int Start()
        {
            this.m_IsStart = true;
            this.Keep();

            return 0;
        }
        #endregion

        #region OpenSocket()//int
        /// <summary>
        /// 開啟Socket
        /// </summary>
        /// <returns></returns>
        private int OpenSocket()
        {
            try
            {
                lock (this.m_LockedControlSocket)
                {
                    // 若所有 Sockets 尚未被關閉則關閉之。
                    this.CloseSockets();

                    // 若已設定 ConnectionString 則需剖析其設定值至特定屬性。
                    if (this.m_ConnectionString.Length > 0)
                    {
                        if (this.ParseConnectString() != 0)
                        {
                            this.OnErrorEvent("Socket剖析其設定值失敗");
                            return -3;
                        }
                    }

                    // 初始化 Socket 相關屬性。
                    if (InitialSocket() != 0)
                    {
                        this.OnErrorEvent("Socket初始化失敗");
                        return -4;
                    }

                    // 開啟 Socket。
                    System.Net.IPEndPoint oIPEP = new System.Net.IPEndPoint(this.m_IPAddress, this.m_Port);
                    this.m_Socket = new System.Net.Sockets.Socket(
                        System.Net.Sockets.AddressFamily.InterNetwork,
                        System.Net.Sockets.SocketType.Stream,
                        System.Net.Sockets.ProtocolType.Tcp);

                    // 區分 server 或 client 的處理方式。
                    if (this.m_Type == 1)
                    {
                        // 若為 server 則 listen。
                        this.m_Socket.Bind(oIPEP);

                        if (IsConnect)
                        {
                            string sSource = this.m_IPAddress.ToString() + ":" + this.m_Port.ToString();
                            this.OnOpenEvent(sSource);

                            // 正常完成繫結，開始監聽與接收資料。
                            this.m_Socket.Listen(int.MaxValue);
                            this.m_Socket.BeginAccept(new System.AsyncCallback(OnSocketBeginAccpt), null);
                        }
                        else
                        {
                            this.OnErrorEvent("無法開啟");
                            return -2;
                        }
                    }
                    else if (this.m_Type == 2)
                    {
                        try
                        {
                            this.m_Socket.Connect(oIPEP);

                            if (IsConnect)
                            {
                                OnConnectedEvent(this.m_IPAddressString + ":" + this.m_Port.ToString());
                                WaitingForData();
                            }
                            else
                            {
                                this.OnErrorEvent("無法與Server(" + this.m_IPAddressString + ":" + this.m_Port.ToString() + "建立連線");
                                return -6;
                            }
                        }
                        catch
                        {
                            this.OnErrorEvent("無法與Server(" + this.m_IPAddressString + ":" + this.m_Port.ToString() + "建立連線");
                            return -6;
                        }
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }

        }
        #endregion

        #region Stop()//void
        /// <summary>
        /// 關閉
        /// </summary>
        private void Stop()
        {
            lock (this.m_LockedControlSocket)
            {
                this.m_IsStart = false;
                this.CloseSockets();
            }
        }
        #endregion

        #region SocketIsConnect(System.Net.Sockets.Socket socket)//int
        /// <summary>
        /// 檢查 Socket 是否與其建立連線。
        /// </summary>
        /// <param name="socket"></param> 
        /// <returns>0：Socket 已繫結。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：Socket 未繫結或開啟。</returns>
        /// <returns>-3：Socket 未初始化。</returns>
        private int SocketIsConnect(System.Net.Sockets.Socket socket)
        {
            bool blockingState = socket.Blocking;

            try
            {
                if (socket == null)
                {
                    return -3;
                }

                if (!socket.Connected)
                {
                    return -2;
                }

                try
                {
                    byte[] tmp = new byte[1];

                    socket.Blocking = false;
                    socket.Send(tmp, 0, 0);                 
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (e.NativeErrorCode.Equals(10035))
                    {
                        //Console.WriteLine("Still Connected, but the Send would block");
                        return 0;
                    }
                    else
                    {
                        //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                        return -2;
                    }                    
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
            finally
            {
                socket.Blocking = blockingState;
            }
        }
        #endregion

        #region SocketIsOpen(System.Net.Sockets.Socket socket)//int
        /// <summary>
        /// 檢查 Socket 是否開啟。
        /// </summary>
        /// <param name="socket"></param> 
        /// <returns>0：Socket 已繫結。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：Socket 未繫結或開啟。</returns>
        /// <returns>-3：Socket 未初始化。</returns>
        private int SocketIsOpen(System.Net.Sockets.Socket socket)
        {
            try
            {
                if (socket == null)
                {
                    return -3;
                }

                if (!socket.IsBound)
                {
                    return -2;
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region CloseSockets()//int
        /// <summary>
        /// 關閉此物件所有的 Sockets。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int CloseSockets()
        {
            int[] iKeys = null;

            try
            {
                lock (this.m_LockedControlSocket)
                {
                    // 釋放所有的 Socket client。 
                    if (this.m_ClientListIDs != null &&
                        this.m_ClientListIDs.Count > 0)
                    {
                        iKeys = new int[this.m_ClientListIDs.Count];
                        m_ClientListIDs.Keys.CopyTo(iKeys, 0);
                        foreach (int key in iKeys)
                        {
                            this.RemoveSocketByID(key);
                        }
                    }

                    // 關閉本物件的 Socket。
                    if (IsConnect)
                    {
                        try
                        {
                            this.m_Socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                        }
                        catch (System.Exception ex)
                        {
                            this.OnErrorEvent(ex);
                        }
                        this.m_Socket.Close();
                        this.m_Socket = null;

                        string sSrouce = $"{this.m_IPAddress}:{this.m_Port}";

                        // Trigger events for socket closed.
                        if (this.m_Type == 1)
                        {
                            this.OnCloseEvent(sSrouce);
                        }
                        else
                        {
                            this.OnDisconnectedEvent(sSrouce);
                        }
                    }

                    if (this.m_ClientListIDs != null && this.m_ClientListIDs.Count > 0)
                    {
                        this.m_ClientListIDs.Clear();
                    }

                    if (this.m_ClientLists != null && this.m_ClientLists.Count > 0)
                    {
                        this.m_ClientLists.Clear();
                    }
                }
                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
            finally
            {
            }
        }
        #endregion

        #region OnSocketBeginReceive(System.IAsyncResult asyn)//void
        /// <summary>
        /// 處理 Socket 有收到資料時的程序。
        /// </summary>
        /// <param name="asyn"></param>
        private void OnSocketBeginReceive(System.IAsyncResult asyn)
        {
            try
            {
                // 取得目前封包的 clinet 連線。
                Packet oPacket = (Packet)asyn.AsyncState;

                try
                {
                    // 先暫止接收資料，並取得目前緩衝區的資料長度。
                    int iBufferLen = oPacket.CurrentSocket.EndReceive(asyn);

                    // 以下的任何後製的程式碼不可因錯誤而退出，以確保可以繼續接收資料。
                    try
                    {
                        // 取出本封包的資料，並觸發接收資料的事件通知後端程式。
                        byte[] dataBuffers = new byte[iBufferLen];

                        for (int ii = 0; ii < iBufferLen; ii++)
                        {
                            dataBuffers[ii] = oPacket.Buffers[ii];
                        }

                        if (ReceivedEvent != null)
                        {
                            ReceivedEvent(dataBuffers, oPacket.CurrentSocket.RemoteEndPoint.ToString());
                        }
                    }
                    catch (System.Exception ex)
                    {
                        this.OnErrorEvent(ex);
                    }

                    // 等待接收後續資料。
                    if (iBufferLen <= 0)
                    {
                        this.RemoveSocketByID(oPacket.ClientID);
                    }
                    else
                    {
                        if (this.m_Type == 1)
                        {
                            WaitingForData(oPacket.CurrentSocket, oPacket.ClientID);
                        }
                        else if (this.m_Type == 2)
                        {
                            WaitingForData();
                        }
                    }
                }
                catch (System.ObjectDisposedException ode)
                {
                    // 觸發事件通知後端程式。 
                    if (IsConnect)
                    {
                        this.OnErrorEvent(ode.Message);
                    }
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    if (se.ErrorCode == 10054)
                    {
                        // 10054: Connection reset by peer.
                        // 設定為 null 後則交由 garbage collection 處理。
                        if (this.m_ClientLists != null && this.m_ClientLists.Count > 0)
                        {
                            this.RemoveSocketByID(oPacket.ClientID);
                        }
                    }
                    else
                    {
                        // 觸發事件通知後端程式。
                        this.OnErrorEvent(se);
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion

        #region AddSocketByID(int clientID, System.Net.Sockets.Socket socket)//void
        /// <summary>
        /// Add new socket by client's ID.
        /// </summary>
        /// <param name="clientID">Client's ID.</param>
        /// <param name="socket">Socket.</param>
        private void AddSocketByID(int clientID, System.Net.Sockets.Socket socket)
        {
            try
            {
                lock (m_LockedControlSocket)
                {
                    string sRemoteEndPoint = socket.RemoteEndPoint.ToString();
                    this.m_ClientListIDs.Add(clientID, sRemoteEndPoint);
                    this.m_ClientLists.Add(sRemoteEndPoint, socket);
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion

        #region RemoveSocketByID(int clientID)//void
        /// <summary>
        /// Remove socket by client ID.
        /// </summary>
        /// <param name="clientID">Client ID.</param>
        private void RemoveSocketByID(int clientID)
        {
            try
            {
                lock (m_LockedControlSocket)
                {
                    string sClientKey = this.GetClientKeyByID(clientID);
                    if (sClientKey != null)
                    {
                        if (this.m_ClientLists[sClientKey] != null)
                        {
                            try
                            {
                                this.m_ClientLists[sClientKey].Close();
                            }
                            catch
                            {
                            }
                            finally
                            {
                                // 觸發事件通知後端程式。
                                this.OnDisconnectedEvent(sClientKey);
                            }
                        }
                        this.m_ClientLists[sClientKey] = null;
                        this.m_ClientLists.Remove(sClientKey);
                        this.m_ClientListIDs.Remove(clientID);
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion

        #region GetClientKeyByID(int clientID)//string
        /// <summary>
        /// Get client key by client ID.
        /// </summary>
        /// <param name="clientID">Client ID.</param>
        /// <returns>Clinet's key or null.</returns>
        private string GetClientKeyByID(int clientID)
        {
            try
            {
                if (this.m_ClientListIDs.ContainsKey(clientID))
                {
                    return this.m_ClientListIDs[clientID];
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return null;
            }
        }
        #endregion

        #region GetSocketByID(int clientID)//System.Net.Sockets.Socket
        /// <summary>
        /// Get connected client by client ID.
        /// </summary>
        /// <param name="clientID">Client ID.</param>
        /// <returns>Socket or null.</returns>
        private System.Net.Sockets.Socket GetSocketByID(int clientID)
        {
            System.Net.Sockets.Socket oReturnSocket = null;

            try
            {
                string sClientKey = this.GetClientKeyByID(clientID);

                if (sClientKey != null)
                {
                    if (this.m_ClientLists.ContainsKey(sClientKey))
                    {
                        oReturnSocket = this.m_ClientLists[sClientKey];
                    }
                }

                return oReturnSocket;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return null;
            }
            finally
            {
                oReturnSocket = null;
            }
        }
        #endregion

        #region WaitingForData()//int
        /// <summary>
        /// 等待連線的 Socket 傳入資料。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int WaitingForData()
        {
            try
            {
                // 指定當 Socket 開始接收資料時的 callback function。
                if (this.m_OnSocketBeginReceiveCallback == null)
                {
                    this.m_OnSocketBeginReceiveCallback = new System.AsyncCallback(OnSocketBeginReceive);
                }

                // 建立處理接收資料的物件，並開始接收資料。
                Packet oPacket = new Packet();
                oPacket.CurrentSocket = this.m_Socket;
                this.m_Socket.BeginReceive(
                    oPacket.Buffers,
                    0,
                    oPacket.Buffers.Length,
                    System.Net.Sockets.SocketFlags.None,
                    this.m_OnSocketBeginReceiveCallback,
                    oPacket);

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region WaitingForData(System.Net.Sockets.Socket sourceSocket, int clientID)//int
        /// <summary>
        /// 等待連線的 Socket 傳入資料。
        /// </summary>
        /// <param name="sourceSocket"></param>
        /// <param name="clientID"></param>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int WaitingForData(System.Net.Sockets.Socket sourceSocket, int clientID)
        {
            try
            {
                // 指定當 Socket 開始接收資料時的 callback function。
                if (this.m_OnSocketBeginReceiveCallback == null)
                {
                    this.m_OnSocketBeginReceiveCallback = new System.AsyncCallback(OnSocketBeginReceive);
                }

                // 建立處理接收資料的物件，並開始接收資料。
                Packet oPacket = new Packet(sourceSocket, clientID);
                sourceSocket.BeginReceive(
                    oPacket.Buffers,
                    0,
                    oPacket.Buffers.Length,
                    System.Net.Sockets.SocketFlags.None,
                    this.m_OnSocketBeginReceiveCallback,
                    oPacket);

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region OnSocketBeginAccpt(System.IAsyncResult asyn)//void
        /// <summary>
        /// 當 client 準備連接時所執行的 callback function。
        /// </summary>
        /// <param name="asyn"></param>
        private void OnSocketBeginAccpt(System.IAsyncResult asyn)
        {
            string sSource = null;

            try
            {
                lock (this.m_LockedControlSocket)
                {
                    if (this.m_Socket != null)
                    {
                        // 以下後製的程式碼必須忽略所有錯誤，以防止無法開始接受連線請求。
                        try
                        {
                            // 取得目前連線請求的 Socket client 並暫止接受連線請求。
                            System.Net.Sockets.Socket oAcceptSocket = this.m_Socket.EndAccept(asyn);

                            // 識別連線的來源。
                            sSource = oAcceptSocket.RemoteEndPoint.ToString();

                            // 以安全執行緒方式將已連接過的 client 數目加一。
                            System.Threading.Interlocked.Increment(ref this.m_ClientConnectedCount);

                            // 記錄這個 Socket client。
                            this.AddSocketByID(this.m_ClientConnectedCount, oAcceptSocket);

                            // 設定目前的連線請求，其後續處理資料的方法。
                            WaitingForData(oAcceptSocket, this.m_ClientConnectedCount);

                            // 通知後端程式有連線請求已處理完成。
                            if (this.ConnectedEvent != null)
                            {
                                this.ConnectedEvent(sSource);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            this.OnErrorEvent(ex);
                        }
                        finally
                        {
                            // 重新開始接受連線的請求。
                            this.m_Socket.BeginAccept(new System.AsyncCallback(OnSocketBeginAccpt), null);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
            }
        }
        #endregion

        #region InitialSocket()//int
        /// <summary>
        /// 初始化 Socket 的相關屬性與設定值。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        /// <returns>-2：無法正常初始化 Socket 的 IP。</returns>
        private int InitialSocket()
        {
            try
            {
                try
                {
                    this.m_IPAddress = null;

                    if (this.m_IPAddressString == null ||
                        this.m_IPAddressString.Trim() == "" ||
                        this.m_IPAddressString.Trim().ToLower() == "localhost")
                    {
                        this.m_IPAddress = System.Net.IPAddress.Any;
                    }
                    else
                    {
                        if (!System.Net.IPAddress.TryParse(this.m_IPAddressString, out this.m_IPAddress))
                        {
                            //判斷m_IPAddressString為DNS
                            string sIP = this.GetHostAddrFromDNS(this.m_IPAddressString);

                            if (!System.Net.IPAddress.TryParse(sIP, out this.m_IPAddress))
                            {
                                this.OnErrorEvent($"由DNS: {this.m_IPAddressString} 所取得的IP: {sIP} ，並非有效的IP位址...");
                                return -2;
                            }
                        }
                    }
                }
                catch
                {
                    // Skip...
                }

                // 若未正確初始化 Socket 的 IP，則回傳錯誤。
                if (this.m_IPAddress == null)
                {
                    this.OnErrorEvent($"IPAddressString: {this.m_IPAddressString} 所初始化的IPAddress物件為null...");
                    return -2;
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region ParseConnectString()//int
        /// <summary>
        /// 剖析 ConnectionString，並寫入相對應的 Socket 屬性。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int ParseConnectString()
        {
            try
            {
                string[] sParams = ASI.Lib.Text.Parsing.String.Split(this.m_ConnectionString.Trim(), ";");
                this.m_IPAddressString = null;
                this.m_Port = -1;
                this.m_Type = -1;
                foreach (string ss in sParams)
                {
                    string[] sLRs = ASI.Lib.Text.Parsing.String.Split(ss.Trim(), "=");

                    if (sLRs.Length >= 2)
                    {
                        string sLvalue = null;
                        string sRvalue = null;
                        if (sLRs[0] != null)
                        {
                            sLvalue = sLRs[0].Trim().ToUpper();
                        }
                        if (sLRs[1] != null)
                        {
                            sRvalue = sLRs[1].Trim();
                        }

                        if (sLvalue == "IP".ToUpper())
                        {
                            this.m_IPAddressString = sRvalue;
                        }
                        else if (sLvalue == "Port".ToUpper())
                        {
                            this.m_Port = int.Parse(sRvalue);
                        }
                        else if (sLvalue == "Type".ToUpper())
                        {
                            if (sRvalue.ToUpper() == "Server".ToUpper())
                            {
                                this.m_Type = 1;
                            }
                            else if (sRvalue.ToUpper() == "Client".ToUpper())
                            {
                                this.m_Type = 2;
                            }
                            else
                            {
                                this.m_Type = -1;
                            }
                        }
                    }
                }

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region ComposeConnectionString()//int
        /// <summary>
        /// 從此物件的屬性重組出連線字串。
        /// </summary>
        /// <returns>0：成功。</returns>
        /// <returns>-1：例外錯誤。</returns>
        private int ComposeConnectionString()
        {
            try
            {
                System.Text.StringBuilder ss = new System.Text.StringBuilder();

                ss.Length = 0;
                ss.Append("IP=" + this.m_IPAddressString + ";");
                ss.Append("Port=" + this.m_Port.ToString() + ";");
                if (this.m_Type == 1)
                {
                    ss.Append("Type=Server;");
                }
                else if (this.m_Type == 2)
                {
                    ss.Append("Type=Client;");
                }
                this.m_ConnectionString = ss.ToString();

                return 0;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return -1;
            }
        }
        #endregion

        #region GetHostAddrFromDNS(string dns) //string
        /// <summary>
        /// 取得指定DNS所代表的IP Address
        /// </summary>
        /// <param name="dns">DNS</param>
        /// <returns></returns>
        private string GetHostAddrFromDNS(string dns)
        {
            string sIP = "";

            try
            {
                System.Net.IPAddress[] oAddressArray = System.Net.Dns.GetHostAddresses(dns);

                if (oAddressArray != null &&
                    oAddressArray.Length > 0)
                {
                    sIP = oAddressArray[0].ToString();
                }

                return sIP;
            }
            catch (System.Exception ex)
            {
                this.OnErrorEvent(ex);
                return "";
            }
        }
        #endregion

        #endregion
    }
}
