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
            OCS_Data  oCS_Data = new OCS_Data();
            string sDBIP = ConfigApp.Instance.GetConfigSetting("DMD_DB_IP");
            string sDBPort = ConfigApp.Instance.GetConfigSetting("DMD_DB_Port");
            string sDBName = ConfigApp.Instance.GetConfigSetting("DMD_DB_Name");
            string sUserID = "postgres";
            string sPassword = "postgres";
            string sCurrentUserID = ConfigApp.Instance.GetConfigSetting("Current_User_ID");
            serial.ReceivedEvent += new Lib.Comm.ReceivedEvents.ReceivedEventHandler(SerialPort_ReceivedEvent);
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
        /// 來自號誌的資料流
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="ID"></param>
        private void SerialPort_ReceivedEvent(byte[]dataBytes, string ID)
        {
           
        }
        /// <summary>
        /// 處理額外的index 11 13 27 29
        /// </summary>
        bool IsSpecialIndex(int index)
        {
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            
            return specialIndices.Contains(index);
        }
        void Process(ushort[] registerBuffer, List<byte> newByteList)
        {
            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i))
                {
                    ushort value1 = registerBuffer[i];
                    ushort value2 = registerBuffer[i + 1];
                    i++; // 跳過下一個索引
                }

                else
                {
                    byte[] ushortBytes = BitConverter.GetBytes(registerBuffer[i]);
                    newByteList.Add(ushortBytes[0]);
                    newByteList.Add(ushortBytes[1]);
                  
                }
            }

        }

       
    }
}
