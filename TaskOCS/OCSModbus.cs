using NModbus;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;

namespace OCS.Modbus
{
    public class OCSData
    {
        public Configuration Config { get; set; }
        public ModbusFactory ModbusFactory { get; set; }
        public IModbusMaster Master { get; set; }
        public ushort[] RegisterBuffer { get; set; } = new ushort[38];
        public byte SlaveAddress { get; set; } = 0;
        public ushort NumberOfPoints { get; set; } = 38;
        public  int Port { get; set; } = 502;
        public string ClientIP { get; set; }
        public int TransactionTimeout { get; set; } = 10;
        public int ConnectionTries { get; set; }
        public int WaitToRetryMilliseconds { get; set; }

        public OCSData() { }

        public OCSData(string clientIP, byte slaveAddress)
        {
            ClientIP = clientIP;
            SlaveAddress = slaveAddress;
        }
        public OCSData(string clientIP, byte slaveAddress, int connectionTries = 3, int timeout = 10, int retryMilliseconds = 1000)
        {
            ClientIP = clientIP;
            SlaveAddress = slaveAddress;
            ConnectionTries = connectionTries;
            TransactionTimeout = timeout;
            WaitToRetryMilliseconds = retryMilliseconds;
            RegisterBuffer = new ushort[NumberOfPoints];
        }
    }

    public class OCSPlatform
    {
        #region Properties
        public int NumberOfPlatforms { get; set; }
        public int Spare1 { get; set; }
        public int PlatformID { get; set; }
        public int PreArrival { get; set; }
        public int Arrival { get; set; }
        public int PreDeparture { get; set; }
        public int Departure { get; set; }
        public int Skip { get; set; }
        public int Hold { get; set; }
        public int NumberOfJourneyData { get; set; }
        public int Spare2 { get; set; }
        public int ValidityField1 { get; set; }
        public int NumberOfCars1 { get; set; }
        public int TrainUnitID1 { get; set; }
        public int ServiceNumber1 { get; set; }
        public int TripNumber1 { get; set; }
        public int DestinationNumber1 { get; set; }
        public long ArrivalTime1 { get; set; }
        public long DepartureTime1 { get; set; }
        public int DelayAtArrival1 { get; set; }
        public int DelayAtDeparture1 { get; set; }
        public int CancelledTrain1 { get; set; }
        public int NextTrainWillNotStop1 { get; set; }
        public int TrainEndOfService1 { get; set; }
        public int TrainWillNotOpenDoor1 { get; set; }
        public int LastTrainOfTheOperatingDay1 { get; set; }
        public int TrainNotInService1 { get; set; }
        public int LineOperationMode1 { get; set; }
        public int TestTrain1 { get; set; }
        public int TrainDirection1 { get; set; }
        public int Spare3 { get; set; }
        public int ValidityField2 { get; set; }
        public int NumberOfCars2 { get; set; }
        public int TrainUnitID2 { get; set; }
        public int ServiceNumber2 { get; set; }
        public int TripNumber2 { get; set; }
        public int DestinationNumber2 { get; set; }
        public long ArrivalTime2 { get; set; }
        public long DepartureTime2 { get; set; }
        public int DelayAtArrival2 { get; set; }
        public int DelayAtDeparture2 { get; set; }
        public int CancelledTrain2 { get; set; }
        public int NextTrainWillNotStop2 { get; set; }
        public int TrainEndOfService2 { get; set; }
        public int TrainWillNotOpenDoor2 { get; set; }
        public int LastTrainOfTheOperatingDay2 { get; set; }
        public int TrainNotInService2 { get; set; }
        public int LineOperationMode2 { get; set; }
        public int TestTrain2 { get; set; }
        public int TrainDirection2 { get; set; }
        public int Spare4 { get; set; }
        #endregion

        #region Fields
        private ushort _startAddress;
        private ushort[] _registerBuffer; // OCS 的原始資料
        private static List<byte[]> _platformByteData;
        private byte[] _processByteArray;
        #endregion

        #region Constructor
        public OCSPlatform(ushort startAddress, ushort[] registerBuffer)
        {
            _startAddress = startAddress;
            _registerBuffer = registerBuffer;
            _platformByteData = new List<byte[]>();
            _processByteArray = new byte[] { };
        }
        #endregion

        #region Methods


        public bool TryConnectAndReadData(OCSData data)
        {
            int attempt = 0;

            while (attempt < data.ConnectionTries)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        var result = client.BeginConnect(data.ClientIP, data.Port, null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(data.TransactionTimeout));

                        if (!success)
                            throw new TimeoutException("Modbus connection timed out.");

                        client.EndConnect(result); // complete connection 

                        // 建立 Modbus master
                        var factory = new ModbusFactory();
                        var master = factory.CreateMaster(client);
                        data.Master = master;

                        // 讀取資料
                        data.RegisterBuffer = master.ReadHoldingRegisters(data.SlaveAddress, 0, data.NumberOfPoints);

                        ASI.Lib.Log.DebugLog.Log("OCS_Connection", $"Successfully connected and read data from {data.ClientIP} on attempt {attempt + 1}.");
                        return true; // 成功
                    }
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.DebugLog.Log("OCS_Error", $"Connection attempt {attempt + 1} failed: {ex.Message}");
                    System.Threading.Thread.Sleep(data.WaitToRetryMilliseconds); // 等待後重試
                    attempt++;
                }
            }

            ASI.Lib.Log.DebugLog.Log("OCS_Error", $"Failed to connect to {data.ClientIP} after {data.ConnectionTries} attempts.");
            return false; // 全部嘗試失敗
        }


        private byte[] Process(ushort[] registerBuffer)
        {
            var memoryStream = new MemoryStream(); 
            try
            {
                var binaryWriter = new BinaryWriter(memoryStream);
                try
                {
                    for (int i = 0; i < registerBuffer.Length; i++)
                    {
                        if (IsSpecialIndex(i))
                        {
                            var combinedBytes = CombineByte(registerBuffer[i], registerBuffer[i + 1]);
                            binaryWriter.Write(combinedBytes);
                            i++; // Skip next index
                        }
                        else
                        {
                            binaryWriter.Write(BitConverter.GetBytes(registerBuffer[i]));
                        }
                    }
                }
                finally
                {
                    binaryWriter.Dispose(); // Dispose BinaryWriter
                }
            }
            finally
            {
                memoryStream.Dispose(); // Dispose MemoryStream
            }
            return memoryStream.ToArray();

        }
        /// <summary>
        /// 位元轉移  
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        private int CombineBytesToInt(ushort byte1, ushort byte2)
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(byte2 >> 8);
            bytes[2] = (byte)byte2;
            bytes[1] = (byte)(byte1 >> 8);
            bytes[0] = (byte)byte1;
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// 位移轉移 
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        private byte[] CombineByte(ushort byte1, ushort byte2)
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(byte2 >> 8);
            bytes[2] = (byte)byte2;
            bytes[1] = (byte)(byte1 >> 8);
            bytes[0] = (byte)byte1;
            return bytes;
        }
        /// <summary>
        ///特定 index 的判斷  
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsSpecialIndex(int index) 
        {
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }
        /// <summary>
        /// xor 的判斷
        /// </summary>
        /// <param name="hex1"></param>
        /// <param name="hex2"></param>
        /// <returns></returns>
        public static ushort[] XOR(ushort[] hex1, ushort[] hex2)
        {
            ushort[] hexOut = new ushort[hex1.Length];
            for (int i = 0; i < hex1.Length; i++)
            {
                hexOut[i] = (ushort)(hex1[i] ^ hex2[i]);
            }
            return hexOut;
        }
        #endregion
    }
}
