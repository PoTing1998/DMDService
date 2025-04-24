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

using TaskOCS;

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
                var oCSData = InitializeOCSData();
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
        private OCSData InitializeOCSData()
        {
            try
            {
                var oCSData = new OCSData();

                // 從配置中獲取 TCP 客戶端 IP 地址 
                var tcpClientIP = ConfigApp.Instance.GetConfigSetting("TcpClientIP");

                // 從配置中獲取 TCP 客戶端埠號並將其解析為整數  
                var tcpClientPort = int.Parse(ConfigApp.Instance.GetConfigSetting("TcpClientPort"));

                // 初始化 ModbusFactory 以建立 Modbus 通訊物件 
                oCSData.ModbusFactory = new NModbus.ModbusFactory();

                // 使用指定的 IP 和埠號創建 Modbus 主站 
                oCSData.Master = oCSData.ModbusFactory.CreateMaster(new TcpClient(tcpClientIP, tcpClientPort));

                // 設定 Modbus 通訊的讀取逾時時間 
                oCSData.Master.Transport.ReadTimeout = oCSData.TransactionTimeout;

                // 設定通訊失敗時的重試次數  
                oCSData.Master.Transport.Retries = oCSData.ConnectionTries;

                // 設定重試之間的等待時間（以毫秒為單位） 
                oCSData.Master.Transport.WaitToRetryMilliseconds = oCSData.WaitToRetryMilliseconds;

                // 啟動背景執行緒持續讀取 Modbus 資料 
                Task.Run(() => ContinuousDataRead(oCSData, 1000));

                var pollingService = new ModbusPollingService(oCSData, 10000); // 每10秒讀一次
                pollingService.Start();
                return oCSData;
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
        #endregion


        #region Method

  

        // 持續讀取資料的邏輯
        private void ContinuousDataRead(OCSData oCSData, int intervalMilliseconds)
        {
            ushort[] previousData = null; // 用於存儲上一次的資料 
            while (true)
            {
                try
                {
                    // 從 Modbus 主站讀取資料 (此處以讀取保持暫存器為例)
                    var currentData = oCSData.Master.ReadHoldingRegisters(1, 0, 38);

                    // 如果有上一次的資料，進行 XOR 比對 
                    if (previousData != null)
                    {
                        if (!ModbusPollingService.AreArraysEqual(previousData, currentData))
                        {
                            ASI.Lib.Log.DebugLog.Log("ContinuousRead", $"資料變更: 新資料 = {string.Join(", ", currentData)}");
                        }
                    }
                    else
                    {
                        ASI.Lib.Log.DebugLog.Log("ContinuousRead", $"首次讀取資料: {string.Join(", ", currentData)}");
                    }

                    // 更新暫存的資料
                    previousData = currentData;

                    // 避免過度讀取，設定每次讀取間隔
                    Thread.Sleep(intervalMilliseconds);
                }
                catch (Exception ex)
                {
                    // 記錄讀取過程中的錯誤
                    ASI.Lib.Log.ErrorLog.Log("ContinuousRead", $"讀取 Modbus 資料時發生錯誤: {ex.Message}");
                }
            }
        }

        // 使用 XOR 比較兩個 ushort 陣列是否不同 


        /// <summary>
        /// 處理註冊緩衝區的資料，並依據特殊索引進行不同的顯示和資料組合操作。
        /// </summary>
        /// <param name="registerBuffer">包含 ushort 數據的註冊緩衝區</param>
        /// <param name="newByteList">儲存轉換後 byte 數據的列表</param>
        void Process(ushort[] registerBuffer, List<byte> newByteList)
        {
            if (registerBuffer == null || newByteList == null) return;

            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i) && i + 1 < registerBuffer.Length)
                {
                    ushort high = registerBuffer[i];
                    ushort low = registerBuffer[i + 1];
                    byte[] combined = BitConverter.GetBytes((high << 16) | low); // 合併成 int，再轉 byte[]
                    newByteList.AddRange(combined);
                    i++; // 跳過下一個
                }
                else
                {
                    byte[] bytes = BitConverter.GetBytes(registerBuffer[i]);
                    newByteList.AddRange(bytes);
                }
            }
        }

        /// <summary>
        /// 判斷當前索引是否為特殊索引。 
        /// </summary>
        /// <param name="index">當前索引</param>
        /// <returns>若為特殊索引則返回 true，否則返回 false</returns>
        bool IsSpecialIndex(int index)
        {
            // 定義特殊索引集合
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }


        /// <summary>
        /// 將兩個 ushort 的高低位組合為 4 個 byte 數組。
        /// </summary>
        /// <param name="highOrder">高位 ushort 數值</param> 
        /// <param name="lowOrder">低位 ushort 數值</param>
        /// <returns>組合後的 byte 數組</returns>
        byte[] CombineBytes(ushort highOrder, ushort lowOrder)
        {
            // 創建 4 個 byte 的數組來表示組合的結果
            byte[] bytes = new byte[4];
            // 將兩個 ushort 數值的高低位分別放入 byte 數組中  
            bytes[3] = (byte)(lowOrder >> 8);
            bytes[2] = (byte)lowOrder;
            bytes[1] = (byte)(highOrder >> 8);
            bytes[0] = (byte)highOrder;
            return bytes;
        }
        /// <summary>
        /// 將兩個 ushort 組合為一個 int 整數值。
        /// </summary>   
        /// <param name="highOrder">高位 ushort 數值</param>
        /// <param name="lowOrder">低位 ushort 數值</param>
        /// <returns>組合後的整數值</returns>
        int CombineUshortToInt(ushort highOrder, ushort lowOrder)
        {
            // 創建 4 個 byte 的數組來表示 int
            byte[] bytes = new byte[4];
            // 將兩個 ushort 數值的高低位分別放入 byte 數組中
            bytes[3] = (byte)(lowOrder >> 8);
            bytes[2] = (byte)lowOrder;
            bytes[1] = (byte)(highOrder >> 8);
            bytes[0] = (byte)highOrder;
            // 使用 BitConverter 將 byte 數組轉換為 int 
            return BitConverter.ToInt32(bytes, 0);
        }
        #endregion
    }
}