using ASI.Lib.Config;
using ASI.Lib.Log;
using ASI.Lib.Process;

using NModbus;

using OCS.Modbus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ASI.Wanda.DMD.TaskOCS
{

    #region construct
    /// <summary>
    /// 處理OCS模組執行程序
    /// </summary>
    public class ProcTaskOCS : ProcBase
    {
        private System.DateTime LastHeartbeatTime = System.DateTime.Now;

        private List<string> Description = new List<string>() {
            "No Of Platforms" , "Platform ID", "Arrival", "Departure",
            "Hold", "NbJourneyData","Validity = 1,NbCars = 2","Train ID","Service","Trip","Destination_ID",
            "ArrivalTime","ArrivalTime","DepartureTime","DepartureTime","DelayAtArrival","DelayAtDeparture",
            "CancelledTrain & NextTrainWillnotSto","TrainEnd Of Service & TrainWillNotOpenDoor" ,"Train Not in service",
            "Time table mode & Normal Train" ,"Train Direction"," Validity = 1 & NbCars = 2" ,"Train ID","Service ",
            "Trip","Destination_ID", "ArrivalTime","ArrivalTime","DepartureTime","DepartureTime","DelayAtArrival",
            "DelayAtDeparture","NextTrainWillnotStop","Train End of service","Last Train of Operating Day & Train Not in service " ,
            "Time table mode &  Normal Train","Train Direction"};


        ASI.Wanda.DMD.DMD_API mDMD_API = new DMD_API();

        public string mOCSServerConnStr = "";

        ASI.Lib.Comm.SerialPort.SerialPortLib serial = null;
        private byte[] arrPacketByte;
        #endregion
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
            if (base.ProcTimerEvent(pMessage) <= 0)
            {
                return -1;
            }
            //ping OCS Server
            if (this.mOCSServerConnStr != "")
            {
                try
                {
                    string sOCSServerIP = ASI.Lib.Config.ConfigApp.Instance.GetIPFromConnStr(this.mOCSServerConnStr);

                    bool bStatus = ASI.Lib.Comm.Network.NetworkLib.TryPing(sOCSServerIP, 300, 4);
                    ASI.Lib.Log.DebugLog.Log(mProcName, $"嘗試ping OCS Server IP = {sOCSServerIP}，連線狀態:{bStatus}");
                }
                catch (System.Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
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
            mProcName = "TaskOCS";

            // 初始化 Serial Port 並處理可能的例外狀況
            try
            {
                InitializeSerialPort();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"初始化 Serial Port 失敗: {ex.Message}");
                return -1; // 異常狀態回傳 -1
            }

            // 初始化 OCS 資料並處理可能的例外狀況
            try
            {
                var oCSData = InitializeOCSData();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"初始化 OCS 資料失敗: {ex.Message}");
                return -1; // 異常狀態回傳 -1
            }

            // 初始化資料庫連線
            if (!InitializeDatabaseConnection())
            {
                ErrorLog.Log(mProcName, $"資料庫連線失敗! {ConfigApp.Instance.GetConfigSetting("DMD_DB_IP")}:" +
                                        $"{ConfigApp.Instance.GetConfigSetting("DMD_DB_Port")}; userid=postgres");
                return -1; // 異常狀態回傳 -1
            }

            return base.StartTask(pComputer, pProcName);
        }

        /// <summary>
        /// 初始化 Serial Port 並註冊事件處理程序
        /// </summary>
        private void InitializeSerialPort()
        {
            try
            {
                serial = new ASI.Lib.Comm.SerialPort.SerialPortLib();
                serial.ReceivedEvent += new ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler(SerialPort_ReceivedEvent);
                serial.DisconnectedEvent += new ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler(SerialPort_DisconnectedEvent);
            }
            catch (Exception ex)
            {
                // 記錄 Serial Port 初始化失敗的例外狀況
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"初始化 Serial Port 失敗: {ex.Message}");
                throw; // 將例外狀況拋出以便上層方法處理
            }
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
                var tcpClientIP = ConfigApp.Instance.GetConfigSetting("TcpClientIP");
                var tcpClientPort = int.Parse(ConfigApp.Instance.GetConfigSetting("TcpClientPort"));

                oCSData.ModbusFactory = new ModbusFactory();
                oCSData.Master = oCSData.ModbusFactory.CreateMaster(new TcpClient(tcpClientIP, tcpClientPort));
                oCSData.Master.Transport.ReadTimeout = oCSData.TransactionTimeout;
                oCSData.Master.Transport.Retries = oCSData.ConnectionTries;
                oCSData.Master.Transport.WaitToRetryMilliseconds = oCSData.WaitToRetryMilliseconds;

                return oCSData;
            }
            catch (FormatException ex)
            {
                // 捕捉並處理字串轉數字的格式錯誤
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"TCP 客戶端 Port 格式錯誤: {ex.Message}");
                throw;
            }
            catch (SocketException ex)
            {
                // 捕捉並處理 TCP 連線的 Socket 錯誤
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"初始化 TCP 客戶端失敗: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // 捕捉其他所有潛在的例外狀況
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"初始化 OCS 資料失敗: {ex.Message}");
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
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗! {sDBIP}:{sDBPort}; userid={sUserID}; ex={ex}");
                return false;
            }
        }

        #endregion
        #region Method
        /// <summary>
        /// 判斷當前索引是否為特殊索引。
        /// </summary>
        /// <param name="index">當前索引</param>
        /// <returns>若為特殊索引則返回 true，否則返回 false</returns>
        bool IsSpecialIndex(int index) // 定義特殊索引集合
        {
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }
        void Process(ushort[] registerBuffer, List<byte> newByteList)
        {

            for (int i = 0; i < registerBuffer.Length; i++)
            {
                // 如果索引為特殊索引，進行特殊資料處理
                if (IsSpecialIndex(i))
                {
                    ushort firstValue = registerBuffer[i];
                    ushort secondValue = registerBuffer[i + 1];
                    // 將兩個 ushort 組合成 byte 數組並加入到 newByteList 中
                    byte[] combinedBytes = CombineBytes(firstValue, secondValue);
                    newByteList.AddRange(combinedBytes);
                    i++; // 跳過下一個索引
                }
                else
                {
                    // 將單個 ushort 轉換為 byte 數組
                    byte[] ushortBytes = BitConverter.GetBytes(registerBuffer[i]);
                    newByteList.AddRange(ushortBytes);
                }
            }
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
        /// 處理號誌的資料流
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="ID"></param>
        private void SerialPort_ReceivedEvent(byte[] dataBytes, string ID)
        {
            OCSData oCS_Data = new OCSData();
            try
            {
                // 初始化 Modbus 連線
                InitializeModbus(oCS_Data);

                ushort startAddress = 30001;
                int numIterations = 18; // 要讀取的迴圈次數 (以要抓取月台數量為準)

                for (int iteration = 0; iteration < numIterations; iteration++)
                {
                    // 讀取 Modbus 資料
                    var registerBuffer = ReadModbusRegisters(oCS_Data, startAddress, 38);
                    if (registerBuffer == null) continue; // 若讀取失敗則跳過本次迴圈

                    // 將原始資料記錄到日誌中
                    LogRawData(registerBuffer);

                    // 根據地址範圍進行處理
                    List<byte> newByteList = new List<byte>();
                    ProcessDataBasedOnAddress(registerBuffer, startAddress, newByteList); 

                    // 更新起始地址
                    startAddress += 100;

                    // 將處理過的資料記錄到日誌中
                    LogProcessedData(newByteList, startAddress);
                }
            }
            catch (Exception ex)
            {
                // 紀錄例外狀況
                ASI.Lib.Log.ErrorLog.Log("Processed_OCS_Data", ex);
            }
        }

        /// <summary>
        /// 初始化 Modbus 連線
        /// </summary>
        private void InitializeModbus(OCSData oCS_Data)
        {
            oCS_Data.ModbusFactory = new ModbusFactory();
            oCS_Data.Master = oCS_Data.ModbusFactory.CreateMaster(new TcpClient("10.107.26.99", 502));
            oCS_Data.Master.Transport.ReadTimeout = oCS_Data.TransactionTimeout;
            oCS_Data.Master.Transport.Retries = oCS_Data.ConnectionTries;
            oCS_Data.Master.Transport.WaitToRetryMilliseconds = oCS_Data.WaitToRetryMilliseconds;
        }

        /// <summary>
        /// 讀取 Modbus 的輸入寄存器
        /// </summary>
        /// <param name="oCS_Data">OCSData 物件</param>
        /// <param name="startAddress">開始讀取的寄存器位址</param>
        /// <param name="numRegisters">要讀取的寄存器數量</param>
        /// <returns>讀取到的輸入寄存器陣列</returns>
        private ushort[] ReadModbusRegisters(OCSData oCS_Data, ushort startAddress, ushort numRegisters)
        {
            try
            {
                return oCS_Data.Master.ReadInputRegisters(0, startAddress, numRegisters);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ReadModbusRegisters_Error", ex);
                return null; // 若發生例外則回傳 null
            }
        }
       
        /// <summary>
        /// 紀錄原始 Modbus 資料到日誌中  
        /// </summary>
        /// <param name="registerBuffer">原始寄存器資料</param>
        private void LogRawData(ushort[] registerBuffer) 
        {
            var registerBufferData = string.Join("\n", registerBuffer.Select((value, index) => $"原始資料第 {index + 1} 個資料: {value}"));
            ASI.Lib.Log.DebugLog.Log("Raw_OCS_Data", registerBufferData);
        }
        
        /// <summary>
        /// 根據地址範圍進行資料處理 
        /// </summary> 
        /// <param name="registerBuffer">原始寄存器資料</param>
        /// <param name="startAddress">當前讀取的起始地址</param>
        /// <param name="newByteList">處理後的 Byte 資料列表</param>
        private void ProcessDataBasedOnAddress(ushort[] registerBuffer, ushort startAddress, List<byte> newByteList)
        { 
            // 轉換 startAddress 為 int 型別，避免型別不匹配錯誤 
            int address = startAddress;
            
            switch (address)
            {
                case int addr when addr > 30000 && addr < 31800:
                    // 處理正常號誌範圍內的資料
                    Process(registerBuffer, newByteList);
                    break;
                default:
                    // 處理其他範圍內的資料
                    Process(registerBuffer, newByteList);
                    break;
            }
        }

        /// <summary>
        /// 紀錄處理過的資料到日誌中
        /// </summary>
        /// <param name="newByteList">處理過的 Byte 資料列表</param>
        /// <param name="currentAddress">當前處理的地址</param>
        private void LogProcessedData(List<byte> newByteList, ushort currentAddress)
        {
            var logMessage = $"目前的 Address {currentAddress}：\n" +
                             string.Join("\n", newByteList.Select((value, index) => $"接收後整理第 {index + 1} 個資料: {value}"));
            ASI.Lib.Log.DebugLog.Log("Processed_OCS_Data", logMessage);
        }

        void SerialPort_DisconnectedEvent(string source) //斷線處理  
        {
            try
            {
                this.serial.Close();
                this.serial = null;
                serial = new ASI.Lib.Comm.SerialPort.SerialPortLib();
                serial.ReceivedEvent += new ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler(SerialPort_ReceivedEvent);
                serial.DisconnectedEvent += new ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler(SerialPort_DisconnectedEvent);
              
            }
            catch (Exception)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, "斷線處理錯誤");
            }
        }
        #endregion
    }
}
