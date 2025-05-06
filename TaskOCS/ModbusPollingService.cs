using ASI.Lib.Process;
using ASI.Wanda.DMD.JsonObject.DCU.FromDMD;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskOCS
{
    public class ModbusPollingService
    {
        private readonly OCSData _ocsData;
        private CancellationTokenSource _cts;
        private Task _pollingTask;
        private readonly int _pollingIntervalMs;
        private readonly string _procName = "ModbusPollingService";

        public ModbusPollingService(OCSData ocsData, int pollingIntervalMs = 10000)
        {
            _ocsData = ocsData;
            _pollingIntervalMs = pollingIntervalMs;
        }

        public void Start()
        {
            if (_pollingTask != null && !_pollingTask.IsCompleted)
                return;

            _cts = new CancellationTokenSource();
            _pollingTask = Task.Run(() => PollingLoop(_cts.Token));
        }

        public async Task StopAsync()
        {
            if (_cts == null)
                return;

            _cts.Cancel();

            if (_pollingTask != null)
            {
                try
                {
                    await _pollingTask;
                }
                catch (OperationCanceledException)
                {
                    // 任務被取消，這是正常行為，可以略過
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, $"停止輪詢時發生例外: {ex.Message}");
                }
                finally
                {
                    _pollingTask = null;
                    _cts.Dispose();
                    _cts = null;
                }
            }
        }


        private ushort[] _previousData = null;
        private OCSPlatform _ocsPlatform = new OCSPlatform();


        private async Task PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    ushort startAddress = 0;
                    ushort numRegisters = 38;  // 讀取 38 筆 
                    byte slaveId = 1;
                    List<byte> newByteList = new List<byte>();
                    ushort[] currentData = _ocsData.Master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                    if (_previousData == null || !AreArraysEqual(_previousData, currentData))
                    {
                        ASI.Lib.Log.DebugLog.Log(_procName, $"資料變更: 新資料 = {string.Join(", ", currentData)}");

                        byte[] byteArray = new byte[currentData.Length * 2];
                        for (int i = 0; i < currentData.Length; i++)
                        {
                            byteArray[i * 2] = (byte)(currentData[i] >> 8);
                            byteArray[i * 2 + 1] = (byte)(currentData[i] & 0xFF);
                        }

                        string result = Encoding.ASCII.GetString(byteArray);  // 或使用 UTF8 / Big5 視資料編碼而定

                        // 更新 _ocsPlatform 物件
                        _ocsPlatform.UpdateFromUShortArray(currentData);

                        // 如果需要，將資料存入 TrainMSG 物件 組封包
                        var oJsonObject = new TrainMSG(ASI.Wanda.DMD.Enum.Station.OCC)
                        {
                            Type = "Train",
                            Command = "Update",
                            Platform_id = _ocsPlatform.PlatformID.ToString(),
                            Arrive_time1 = _ocsPlatform.ArrivalTime1.ToString(),
                            Depart_time1 = _ocsPlatform.DelayAtDeparture1.ToString(),
                            Destination1 = _ocsPlatform.DestinationNumber1.ToString(),
                            Arrive_time2 = _ocsPlatform.ArrivalTime2.ToString(),
                            Depart_time2 = _ocsPlatform.DelayAtDeparture2.ToString(),
                            Destination2 = _ocsPlatform.DestinationNumber2.ToString()
                        };

                        // 顯示更新後資料
                        ASI.Lib.Log.DebugLog.Log(_procName, $"平台ID: {_ocsPlatform.PlatformID}, 到站: {_ocsPlatform.Arrival}, 離站: {_ocsPlatform.Departure}");

                        var MSG = new ASI.Wanda.DMD.Message.Message(ASI.Wanda.DMD.Message.Message.eMessageType.Command, 0, ASI.Lib.Text.Parsing.Json.SerializeObject(oJsonObject));
                        // 傳送不同的格式
                        SendToTaskDCU(2, 0, ASI.Lib.Text.Parsing.Json.SerializeObject(oJsonObject));

                        _previousData = (ushort[])currentData.Clone();
                    }
                    else
                    {
                        ASI.Lib.Log.DebugLog.Log(_procName, $"首次讀取資料: {string.Join(", ", currentData)}");

                        Process(currentData, newByteList);
                        _previousData = (ushort[])currentData.Clone(); // 資料有變才更新 
                    }
                }
                catch (IOException ioEx)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, $"IO 錯誤: {ioEx.Message}，嘗試重新連線...");
                    TryReconnect();
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, $"Modbus 讀取錯誤: {ex.Message}");
                }

                await Task.Delay(_pollingIntervalMs, token);
            }

            ASI.Lib.Log.DebugLog.Log(_procName, "Modbus 輪詢已停止。");
        }



        private void TryReconnect()
        {
            for (int i = 0; i < _ocsData.ConnectionTries; i++)
            {
                try
                {
                    var tcpClient = new TcpClient(_ocsData.ClientIP, _ocsData.Port);
                    _ocsData.Master = _ocsData.ModbusFactory.CreateMaster(tcpClient);
                    _ocsData.Master.Transport.ReadTimeout = _ocsData.TransactionTimeout;
                    _ocsData.Master.Transport.Retries = _ocsData.ConnectionTries;
                    _ocsData.Master.Transport.WaitToRetryMilliseconds = _ocsData.WaitToRetryMilliseconds;

                    ASI.Lib.Log.DebugLog.Log(_procName, "重新連線成功");
                    return;
                }
                catch
                {
                    Thread.Sleep(_ocsData.WaitToRetryMilliseconds);
                }
            }

            ASI.Lib.Log.ErrorLog.Log(_procName, "無法重新連線 Modbus slave");
        }
        public static bool AreArraysEqual<T>(T[] array1, T[] array2) where T : IEquatable<T>
        {
            if (array1 == null || array2 == null) return false;
            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                    return false;
            }

            return true;
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
        public void Process(ushort[] registerBuffer, List<byte> newByteList)
        {
            if (registerBuffer == null || newByteList == null ) return;

            StringBuilder logBuilder = new StringBuilder();

            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i) && i + 1 < registerBuffer.Length)
                {
                    ProcessSpecialIndex(registerBuffer, newByteList, logBuilder, ref i);
                }
                else
                {
                    ProcessNormalIndex(registerBuffer[i], newByteList, logBuilder, i);
                }
            }

        }

        private void ProcessSpecialIndex(ushort[] buffer, List<byte> byteList, StringBuilder builder, ref int index)
        {
            ushort high = buffer[index];
            ushort low = buffer[index + 1];

            // DisplaySpecialValue(index, high, low, builder);
            byteList.AddRange(CombineBytes(high, low));

            index++; // 因為用了兩個 ushort，所以手動 +1
        }

        private void ProcessNormalIndex(ushort value, List<byte> byteList, StringBuilder builder, int index)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byteList.AddRange(bytes);

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
    }

 

}
