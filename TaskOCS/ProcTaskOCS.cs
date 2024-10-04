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
        #region  ttaskocs資料處理

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
          
            serial = new ASI.Lib.Comm.SerialPort.SerialPortLib();
            serial = new ASI.Lib.Comm.SerialPort.SerialPortLib();
            serial.ReceivedEvent += new ASI.Lib.Comm.ReceivedEvents.ReceivedEventHandler(SerialPort_ReceivedEvent);
            serial.DisconnectedEvent += new ASI.Lib.Comm.ReceivedEvents.DisconnectedEventHandler(SerialPort_DisconnectedEvent);  

            OCSData oCS_Data = new OCSData();
            var sDBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            var sDBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            var sDBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");
            var sUserID = "postgres";
            var sPassword = "postgres";
            var sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID"); 
            //設定modbus的初始資料   
            var TcpClientIP = ConfigApp.Instance.GetConfigSetting("TcpClientIP");
            var TcpClientPort = ConfigApp.Instance.GetConfigSetting("TcpClientPort");
            oCS_Data.ModbusFactory = new ModbusFactory();
            oCS_Data.Master = oCS_Data.ModbusFactory.CreateMaster(new TcpClient(TcpClientIP, int.Parse(TcpClientPort)));
            oCS_Data.Master.Transport.ReadTimeout = oCS_Data.TransactionTimeout;
            oCS_Data.Master.Transport.Retries = oCS_Data.ConnectionTries; 
            oCS_Data.Master.Transport.WaitToRetryMilliseconds = oCS_Data.WaitToRetryMilliseconds; 
            try
            {
                //"Server='localhost'; Port='5432'; Database='DMDDB'; User Id='postgres'; Password='postgres'";  
                if (!ASI.Wanda.DMD.DB.Manager.Initializer(sDBIP, sDBPort, sDBName, sUserID, sPassword, sCurrentUserID))
                {
                    ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID}");  
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(mProcName, $"資料庫連線失敗!{sDBIP}:{sDBPort};userid={sUserID};ex={ex}");
            }

            return base.StartTask(pComputer, pProcName);
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
                ///設定modbus的初始資料 
                oCS_Data.ModbusFactory = new ModbusFactory();
                oCS_Data.Master = oCS_Data.ModbusFactory.CreateMaster(new TcpClient("10.107.26.99", 502));
                oCS_Data.Master.Transport.ReadTimeout = oCS_Data.TransactionTimeout;
                oCS_Data.Master.Transport.Retries = oCS_Data.ConnectionTries;
                oCS_Data.Master.Transport.WaitToRetryMilliseconds = oCS_Data.WaitToRetryMilliseconds;
                ushort startAddress = 30001;
                int numIterations = 18;/// 要讀取的迴圈次數  (以要抓取月台數量為準)

                for (int iteration = 0; iteration < numIterations; iteration++)
                {
                    var newByteList = new List<byte>(); 
                    ///讀取salve的資料
                    var registerBuffer = oCS_Data.Master.ReadInputRegisters(0, 30001, (ushort)38);
                    ///獲得原始資料後，就寫log檔案
                    string registerBufferData = "";  
                    int b = 1;
                    foreach (var a in registerBuffer)
                    {
                        registerBufferData += $"原始資料第 {b} 個資料:{a}\n";
                        b++;
                    }
                    ASI.Lib.Log.DebugLog.Log("Raw_OCS_Data", registerBufferData);
                    switch (30001)
                    {
                        ///正常號誌範圍內 
                        case int address when address > 30000 && address < 31800:
                            ///進行拆解
                            Process(registerBuffer, newByteList);
                            break;
                        default:
                            /// 在沒有符合的條件下，執行正常的處理邏輯 
                            registerBuffer = oCS_Data.Master.ReadInputRegisters(0, 30001, 38);
                            Process(registerBuffer, newByteList);
                            break;
                    }
                    startAddress += 100; /// 每次迴圈遞增的寄存器位址間距
                    string logMessage = $"目前的Address{startAddress}：\n";
                    int i = 1;
                    ///寫進log檔的資料 時間 名稱代號 原本值ushort  拆解後(byte) 
                    foreach (var a in newByteList)
                    {
                        logMessage += $"接收後整理第 {i} 個資料:{a} \n";
                        i++;
                    }
                    ASI.Lib.Log.DebugLog.Log("Processed_OCS_Data", logMessage);
                }
            }
            catch (System.Exception ex )
            {
                ASI.Lib.Log.ErrorLog.Log("Processed_OCS_Data",ex);
            }
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
