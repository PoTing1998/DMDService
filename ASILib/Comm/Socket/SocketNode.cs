using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ASI.Lib.Comm;

namespace ASI.Lib.Comm.Socket
{
    /// <summary>
    /// SocketEventArgs資料儲存實體
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// SocketEventArgs程式進入點
        /// </summary>
        public SocketEventArgs()
        {

        }

        /// <summary>
        /// SocketEventArgs程式進入點
        /// </summary>
        /// <param name="InfoType"></param>
        /// <param name="Info"></param>
        public SocketEventArgs(CommType InfoType, string Info)
        {
            this.InfoType = InfoType;
            this.Info = Info;
        }

        /// <summary>
        /// SocketEventArgs程式進入點
        /// </summary>
        /// <param name="Message"></param>
        public SocketEventArgs(byte[] Message)
        {
            this.Message = Message;
        }

        /// <summary>
        /// SocketEventArgs程式進入點
        /// </summary>
        /// <param name="Mtype"></param>
        /// <param name="Message"></param>
        public SocketEventArgs(int Mtype, byte[] Message)
        {
            this.MessageType = Mtype;
            this.Message = Message;
        }

        // The fire event will have two pieces of information-- 
        // 1) Where the fire is, and 2) how "ferocious" it is.  

        /// <summary>
        /// Communication Type
        /// </summary>
        public CommType InfoType;
        /// <summary>
        /// Communication Info
        /// </summary>
        public string Info;
        /// <summary>
        /// Message Type
        /// </summary>
        public int MessageType;
        /// <summary>
        /// Server端傳入之Message
        /// </summary>
        public byte[] Message;

    }	//end of class FireEventArgs


    /// <summary>
    /// ClientSocket資料儲存實體
    /// </summary>
    class ClientSocket
    {
        /// <summary>
        /// Server端接收訊息之編號
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// Client端Socket物件
        /// </summary>
        public System.Net.Sockets.Socket CltSocket = null;
        /// <summary>
        /// ClientSocket程式進入點
        /// </summary>
        /// <param name="AID"></param>
        /// <param name="theSocket"></param>
        public ClientSocket(int AID, System.Net.Sockets.Socket theSocket)
        {
            this.ID = AID;
            CltSocket = theSocket;
        }
    }

    /// <summary>
    /// SocketEvent委派處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SocketEventHandler(object sender, SocketEventArgs e);


    /// <summary>
    /// SocketNode資料儲存實體
    /// </summary>
    class SocketNode
    {
        /// <summary>
        /// Server端接收訊息之編號
        /// </summary>
        private int AssignID = 0;
        /// <summary>
        /// Server端Socket物件
        /// </summary>
        public System.Net.Sockets.Socket SrvSocket = null;
        /// <summary>
        /// Client端Socket物件
        /// </summary>
        public System.Net.Sockets.Socket CltSocket = null;
        /// <summary>
        /// 存放所有Client端之ArrayList
        /// </summary>
        public ArrayList Clients = new ArrayList();
        /// <summary>
        /// 存放Client端節點之Hashtable
        /// </summary>
        public Hashtable ClientNodes = new Hashtable();

        /// <summary>
        /// Server端之執行緒
        /// </summary>
        private System.Threading.Thread threadSrv;
        /// <summary>
        /// Client端之執行緒
        /// </summary>
        private System.Threading.Thread threadClt;

        /// <summary>
        /// ConnectResult事件處理
        /// </summary>
        public event SocketEventHandler ConnectResultEvent;
        /// <summary>
        /// SrvAccept事件處理
        /// </summary>
        public event SocketEventHandler SrvAcceptEvent;

        /// <summary>
        /// Disconnect事件處理
        /// </summary>
        public event SocketEventHandler DisconnectEvent;
        /// <summary>
        /// ReceiveCompleted事件處理
        /// </summary>
        public event SocketEventHandler ReceiveCompletedEvent;
        /// <summary>
        /// ErrorException事件處理
        /// </summary>
        public event SocketEventHandler ErrorExceptionEvent;

        //public event ReceiveCompletedEventHandler ReceiveCompleted;
        /// <summary>
        /// 例外訊息參數
        /// </summary>
        public string ExceptionMessage;

        //Client Socket

        /// <summary>
        /// Socket連線
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public void ConnectSocket(string server, int port)
        {
            //Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            Boolean connStatus = false;
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                //Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                System.Net.Sockets.Socket tempSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    tempSocket.Connect(ipe);
                }
                catch (SocketException e)
                {
                    //ExceptionMessage = "SocketException: " + e.SocketErrorCode.ToString() + " " + e.Message.ToString();
                    //byte[] outValue=System.Text.ASCIIEncoding.UTF8.GetBytes(ExceptionMessage);
                    //SocketEventArgs Args = new SocketEventArgs(outValue);
                    OnErrorException(e);
                }

                if (tempSocket.Connected)
                {
                    CltSocket = tempSocket;
                    if (threadClt != null) { threadClt.Abort(); }
                    threadClt = new System.Threading.Thread(new System.Threading.ThreadStart(CltReceiveAction));
                    threadClt.Start();
                    connStatus = true;
                    OnConnectResult(CommType.ConnSuccessful, "Connected");
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (connStatus) OnConnectResult(CommType.ConnSuccessful, "Success to try connect");
            else OnConnectResult(CommType.ConnFailure, "Failure to try connect");
            //return s;
        }


        //SrvSocket

        /// <summary>
        /// 使Socket與Client端建立關聯並置於接聽狀態
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public void ListenSocket(string server, int port)
        {

            IPHostEntry hostEntry = Dns.GetHostEntry(server);

            // create the socket
            SrvSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            foreach (IPAddress hostIP in hostEntry.AddressList)
            {
                // bind the listening socket to the port
                //IPAddress hostIP = (Dns.Resolve(IPAddress.Any.ToString())).AddressList[0];
                IPEndPoint ep = new IPEndPoint(hostIP, port);
                SrvSocket.Bind(ep);

                // start listening
                SrvSocket.Listen(100);
                //SrvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontFragment, true);
            }
            //if (threadSrv != null) { threadSrv.Abort(); }
            threadSrv = new System.Threading.Thread(new System.Threading.ThreadStart(SrvReceiveAction));
            threadSrv.Start();
        }

        /// <summary>
        /// Server端接收訊息處理
        /// </summary>
        public void SrvReceiveAction()
        {
            //if (SrvSocket == null) return;
            //CltSocket = SrvSocket.Accept();
            ////AssignID++;
            ////Clients.Add(new ClientSocket(AssignID,CltSocket));
            ////ClientNodes.Add(AssignID, CltSocket);
            //OnAccept();
            //while (true){
            //    if (CltSocket.Available > 0){
            //        //CltSocket.Connected
            //        //byte[] outValue = new byte[CltSocket.Available];
            //        //CltSocket.Receive(outValue, CltSocket.Available, SocketFlags.None);
            //        //SocketEventArgs Args = new SocketEventArgs(outValue);
            //        OnReceiveCompleted();
            //    }
            //    Thread.Sleep(100);
            //}

            if (SrvSocket == null) return;
            System.Net.Sockets.Socket theSocket = SrvSocket.Accept();
            CltSocket = theSocket; // ((ClientSocket)ClientNodes[Ikey]).CltSocket;
            OnAccept(theSocket);
            int hashtablekey = AssignID;
            while (true)
            {
                if (theSocket.Available > 0)
                {
                    //byte[] outValue = new byte[theSock.Available];
                    //theSock.Receive(outValue, theSock.Available, SocketFlags.None);
                    //SocketEventArgs Args = new SocketEventArgs(outValue);
                    OnReceiveCompleted();
                }
                else if (!theSocket.Connected)
                {
                    theSocket.Close();
                    ClientNodes.Remove(hashtablekey);
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Client端接收訊息處理
        /// </summary>
        public void CltReceiveAction()
        {
            if (CltSocket == null) return;
            //CltSocket.Receive( = SrvSocket.Accept();
            while (true)
            {
                if (CltSocket.Available > 0)
                {
                    //byte[] outValue = new byte[CltSocket.Available];
                    //CltSocket.Receive(outValue, CltSocket.Available, SocketFlags.None);
                    //SocketEventArgs Args = new SocketEventArgs(outValue);
                    OnReceiveCompleted();
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 關閉Client端通訊
        /// </summary>
        public void CltClose()
        {
            if (CltSocket != null)
            {
                CltSocket.Shutdown(SocketShutdown.Both);
                CltSocket.Disconnect(false);
                OnDisconnect();
                CltSocket.Close();
                threadClt.Abort();
            }
        }

        /// <summary>
        /// 關閉Server端通訊
        /// </summary>
        public void SrvClose()
        {
            if (SrvSocket != null)
            {
                SrvSocket.Shutdown(SocketShutdown.Both);
                OnDisconnect();
                SrvSocket.Close();
                threadSrv.Abort();
            }
        }

        /// <summary>
        /// 處理Socket連線事件
        /// </summary>
        /// <param name="status"></param>
        /// <param name="Info"></param>
        public void OnConnectResult(CommType status, string Info)
        {
            if (ConnectResultEvent != null)
            {
                SocketEventArgs Args = new SocketEventArgs(status, Info);
                ConnectResultEvent(this, Args);
            }
        }

        /// <summary>
        /// 處理Socket斷線事件
        /// </summary>
        public void OnDisconnect()
        {
            if (ConnectResultEvent != null)
            {
                SocketEventArgs Args = new SocketEventArgs(CommType.Unconnect, System.Threading.Thread.CurrentThread.Name);
                DisconnectEvent(this, null);
            }
        }

        /// <summary>
        /// Server端接收訊息處理
        /// </summary>
        /// <param name="theNewSocket"></param>
        public void OnAccept(System.Net.Sockets.Socket theNewSocket)
        {
            AssignID++;
            System.Threading.Thread.CurrentThread.Name = "Thr" + AssignID;
            ClientSocket Sock = new ClientSocket(AssignID, theNewSocket);
            ClientNodes.Add(AssignID, Sock);
            if (SrvAcceptEvent != null) SrvAcceptEvent(this, null);
            threadSrv = new System.Threading.Thread(new System.Threading.ThreadStart(SrvReceiveAction));
            threadSrv.Start();
            //if (SrvAcceptEvent != null) SrvAcceptEvent(this,null);
        }

        /// <summary>
        /// 處理Socket連線時發生的例外事件
        /// </summary>
        /// <param name="e"></param>
        public void OnErrorException(SocketException e)
        {
            if (ErrorExceptionEvent != null)
            {
                ExceptionMessage = "SocketException: " + e.SocketErrorCode.ToString() + " " + e.Message.ToString();
                byte[] outValue = System.Text.ASCIIEncoding.UTF8.GetBytes(ExceptionMessage);
                SocketEventArgs Args = new SocketEventArgs(outValue);
                ErrorExceptionEvent(this, Args);
            }
        }

        //public void OnReceiveCompleted(object Sender, SocketEventArgs e) {
        /// <summary>
        /// 已接收的完整訊息
        /// </summary>
        public void OnReceiveCompleted()
        {
            // Now, raise the event by invoking the delegate. Pass in 
            // the object that initated the event (this) as well as FireEventArgs. 
            // The call must match the signature of FireEventHandler.
            if (ReceiveCompletedEvent != null)
            {
                byte[] outValue = new byte[CltSocket.Available];
                CltSocket.Receive(outValue, CltSocket.Available, SocketFlags.None);
                SocketEventArgs Args = new SocketEventArgs(outValue);
                ReceiveCompletedEvent(this, Args);
            }
        }

    }
}
