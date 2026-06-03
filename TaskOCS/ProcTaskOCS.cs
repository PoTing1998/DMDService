using ASI.Lib.Config;
using ASI.Lib.Log;
using ASI.Lib.Process;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskOCS;
using static OCSClientPoller;

namespace ASI.Wanda.DMD.TaskOCS
{

    /// <summary>
    /// 處理OCS模組執行程序
    /// </summary>
    public class ProcTaskOCS : ProcBase
    {
        public string mOCSServerConnStr = "";
        private OCSClientPoller poller;
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
        private void InitializeOCSData()
        {
            try
            {
                var tcpClientIP = ConfigApp.Instance.GetConfigSetting("TcpClientIP");
                var tcpClientPort = int.Parse(ConfigApp.Instance.GetConfigSetting("TcpClientPort"));

                var clients = new Dictionary<string, ClientModbusConfig>
                {
                    { "Client1", new ClientModbusConfig { IP = tcpClientIP, Port = tcpClientPort, StartAddresses = new List<ushort> { 0 } } }
                };

                this.poller = new OCSClientPoller(clients, SendToTaskDCU);
                this.poller.StartPollingAllClients();
            }
            catch (FormatException ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"TCP 客戶端 Port 格式錯誤: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// 停止任务并清理资源
        /// </summary>
        public override void StopTask()
        {
            try
            {
                ASI.Lib.Log.DebugLog.Log(_mProcName, "正在停止 TaskOCS...");

                if (poller != null)
                {
                    poller.Stop();
                    ASI.Lib.Log.DebugLog.Log(_mProcName, "OCSClientPoller 已停止");
                }

                ASI.Lib.Log.DebugLog.Log(_mProcName, "TaskOCS 已成功停止");
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_mProcName, $"停止 TaskOCS 时发生错误: {ex}");
            }

            base.StopTask();
        }
        #endregion

        #region 
        /// <summary>
        /// 將收到的數值轉成標準時間
        /// </summary>
        public class UnixTimeConverter
        {
            public static DateTime UnixToUTC(long unixTimestamp)
            {
                DateTime baseDate= new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return baseDate.AddSeconds(unixTimestamp);
            } 
            public static DateTime StandardUnixToUTC(long unixTimestamp)
            {
                DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return baseDate.AddSeconds(unixTimestamp);
            }
        }
        #endregion

    }
}