using ASI.Lib.Config;
using ASI.Lib.Log;
using ASI.Lib.Process;

using NModbus;

using OCS.Modbus;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static OCSClientPoller;

namespace ASI.Wanda.DMD.TaskOCS
{

    /// <summary>
    /// 處理OCS模組執行程序
    /// </summary>
    public class ProcTaskOCS : ProcBase
    {
        public string mOCSServerConnStr = "";
        #region  Task開啟處理

        /// <summary>
        /// 處理OCS模組執行程序所收到之訊息 
        /// </summary>
        /// <param name="pLabel"></param>
        /// <param name="pBody"></param>
        /// <returns></returns>
        public override int ProcEvent(string pLabel, string pBody)
        {
            LogFile.Display(pBody);

            if (pLabel == MSGFinish.Label)
            {
                return 0;
            }
            return base.ProcEvent(pLabel, pBody);
        }

        /// <summary>
        /// 處理OCS模組執行程序所收到之定時訊息 
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
        public override int ProcTimerEvent(string pMessage) // handle timer message
        {
            //定時回報TaskMain
            if (base.ProcTimerEvent(pMessage) <= 0) return -1;

            //ping OCS Server
            if (this.mOCSServerConnStr != "")
            {
                try
                {
                    string sOCSServerIP = ASI.Lib.Config.ConfigApp.Instance.GetIPFromConnStr(this.mOCSServerConnStr);

                    bool bStatus = ASI.Lib.Comm.Network.NetworkLib.TryPing(sOCSServerIP, 300, 4);
                    ASI.Lib.Log.DebugLog.Log(_mProcName, $"嘗試ping OCS Server IP = {sOCSServerIP}，連線狀態:{bStatus}");
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_mProcName, ex);
                }
            }
            //讀取資料庫
            //送出訊息播放命令給DMD Server 
            return 1;
        }

        /// <summary> 
        /// 啟始處理OCS模組執行程序  
        /// </summary>
        /// <param name="pComputer"></param>
        /// <param name="pProcName"></param>
        /// <returns></returns>
        public override int StartTask(string pComputer, string pProcName)
        {
            mTimerTick = 30;
            _mProcName = "TaskOCS";

            // 初始化 OCS 資料並處理可能的例外狀況  
            try
            {
                InitializeOCSData();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"初始化 OCS 資料失敗: {ex.Message}");
                return -1; // 異常狀態回傳 -1 
            }

            // 初始化資料庫連線    
            if (!InitializeDatabaseConnection())
            {
                ErrorLog.Log(_mProcName, $"資料庫連線失敗! {ConfigApp.Instance.GetConfigSetting("DMD_DB_IP")}:" +
                                        $"{ConfigApp.Instance.GetConfigSetting("DMD_DB_Port")}; userid=postgres");
                return -1; // 異常狀態回傳 -1
            }

            return base.StartTask(pComputer, pProcName);
        }
        /// <summary>
        /// 初始化 OCS 資料及 Modbus 設定
        /// </summary>
        /// <returns>回傳初始化後的 OCSData 物件</returns>
        private OCSModbusReader InitializeOCSData()
        {
            try
            {
                var ocsDataInstance = new OCSModbusReader();

                // 從配置中獲取 TCP 客戶端 IP 地址 
                var tcpClientIP = ConfigApp.Instance.GetConfigSetting("TcpClientIP");

                // 從配置中獲取 TCP 客戶端埠號並將其解析為整數  
                var tcpClientPort = int.Parse(ConfigApp.Instance.GetConfigSetting("TcpClientPort"));

                // 初始化 ModbusFactory 以建立 Modbus 通訊物件 
                ocsDataInstance.ModbusFactory = new NModbus.ModbusFactory();

                // 使用指定的 IP 和埠號創建 Modbus 主站 
                ocsDataInstance._master = ocsDataInstance.ModbusFactory.CreateMaster(new TcpClient(tcpClientIP, tcpClientPort));

                // 設定 Modbus 通訊的讀取逾時時間 
                ocsDataInstance._master.Transport.ReadTimeout = ocsDataInstance.TransactionTimeout;

                // 設定通訊失敗時的重試次數  
                ocsDataInstance._master.Transport.Retries = ocsDataInstance.ConnectionTries;

                // 設定重試之間的等待時間（以毫秒為單位） 
                ocsDataInstance._master.Transport.WaitToRetryMilliseconds = ocsDataInstance.WaitToRetryMilliseconds;

                // 啟動背景執行緒持續讀取 Modbus 資料 
                var clients = new Dictionary<string, ClientModbusConfig>
{
    { "Client1", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 30001, 30101, 30201 , 30301 , 30401 , 30501 } } },
    { "Client2", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 30601, 30701, 30801 , 30901 , 31001 , 31101 } } },
    { "Client3", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 31201, 31301, 31401 , 31501 , 31601 , 31701 } } }
};

                var poller = new OCSClientPoller(clients, SendToTaskDCU);
                poller.StartPollingAllClients();
                return ocsDataInstance;
            }
            catch (FormatException ex)
            {
                // 捕捉並處理字串轉數字的格式錯誤
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"TCP 客戶端 Port 格式錯誤: {ex.Message}");
                throw;
            }
            catch (SocketException ex)
            {
                // 捕捉並處理 TCP 連線的 Socket 錯誤 
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"初始化 TCP 客戶端失敗: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // 捕捉其他所有潛在的例外狀況
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"初始化 OCS 資料失敗: {ex.Message}");
                throw;
            }
        }
        #endregion
        #region Method
        /// <summary>
        /// 初始化資料庫連線 
        /// </summary>
        /// <returns>若連線成功回傳 true，否則回傳 false</returns>
        private bool InitializeDatabaseConnection()
        {
            var sDBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            var sDBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            var sDBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");
            var sUserID = "postgres";
            var sPassword = "postgres";
            var sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");
            try
            {
                return ASI.Wanda.DMD.DB.Manager.Initializer(sDBIP, sDBPort, sDBName, sUserID, sPassword, sCurrentUserID);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"資料庫連線失敗! {sDBIP}:{sDBPort}; userid={sUserID}; ex={ex}");
                return false;
            }
        }

        private void SendToTaskDCU(int msgType, int msgID, string jsonData)
        {
            try
            {
                var MSGFromTaskOCS = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskOCS(new MSGFrameBase("TaskOCS", "dmdserverTaskDCU"));
                //組相對應的封包
                MSGFromTaskOCS.MessageType = msgType;
                MSGFromTaskOCS.MessageID = msgID;
                MSGFromTaskOCS.JsonData = jsonData;
                ASI.Lib.Process.ProcMsg.SendMessage(MSGFromTaskOCS);
                ASI.Lib.Log.DebugLog.Log("SendToTaskDCU", jsonData);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("FromTaskCMFT", ex);
            }
        }
        #endregion
    }
}