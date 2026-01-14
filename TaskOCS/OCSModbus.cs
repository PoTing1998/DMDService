using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using TaskOCS;

namespace OCS.Modbus
{
   
    public class OCSModbusReader
    {
        public Configuration Config { get; set; }
        public ModbusTcpClient _master { get; set; }
        public ushort[] RegisterBuffer { get; set; } = new ushort[38];
        public byte SlaveAddress { get; set; } = 0;
        public byte _slaveId { get; set; }
        public ushort NumberOfPoints { get; set; } = 38;
        public  int Port { get; set; } = 502;
        public string ClientIP { get; set; }
        public int TransactionTimeout { get; set; } = 1000;
        public int ConnectionTries { get; set; } = 3;
        public int WaitToRetryMilliseconds { get; set; } = 1000;

        public OCSModbusReader() { }

        public OCSModbusReader(ModbusTcpClient master, byte slaveId = 0)
        {
            _master = master;
            _slaveId = slaveId;
        }

        public ushort[] ReadData(ushort startAddress, ushort numRegisters)
        {
            return _master.ReadHoldingRegisters(_slaveId, startAddress, numRegisters);
        }

        public byte[] ToByteArray(ushort[] data)
        {
            byte[] byteArray = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byteArray[i * 2] = (byte)(data[i] >> 8);
                byteArray[i * 2 + 1] = (byte)(data[i] & 0xFF);
            }
            return byteArray;
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
     
        #endregion

        #region Methods

        public void UpdateFromUShortArray(ushort[] data)
        {
            if (data == null || data.Length < 38)
                throw new ArgumentException("資料長度不足（需要至少 38 個 ushort）");

            // 轉換成 byte[76]
            byte[] byteArray = new byte[data.Length * 2];
            for (int j = 0; j < data.Length; j++)
            {
                byteArray[j * 2] = (byte)(data[j] >> 8);
                byteArray[j * 2 + 1] = (byte)(data[j] & 0xFF);
            }

            int i = 0;

            NumberOfPlatforms = byteArray[i++];
            Spare1 = byteArray[i++];
            PlatformID = byteArray[i++];
            PreArrival = byteArray[i++];
            Arrival = byteArray[i++];
            PreDeparture = byteArray[i++];
            Departure = byteArray[i++];
            Skip = byteArray[i++];
            Hold = byteArray[i++];
            NumberOfJourneyData = byteArray[i++];
            Spare2 = byteArray[i++];
            ValidityField1 = byteArray[i++];
            NumberOfCars1 = byteArray[i++];
            TrainUnitID1 = byteArray[i++];
            ServiceNumber1 = byteArray[i++];
            TripNumber1 = byteArray[i++];
            DestinationNumber1 = byteArray[i++];
            ArrivalTime1 = ((long)byteArray[i++] << 16) | byteArray[i++]; // 若使用 2 個 ushort 合成 long
            DepartureTime1 = ((long)byteArray[i++] << 16) | byteArray[i++];
            DelayAtArrival1 = byteArray[i++];
            DelayAtDeparture1 = byteArray[i++];
            CancelledTrain1 = byteArray[i++];
            NextTrainWillNotStop1 = byteArray[i++];
            TrainEndOfService1 = byteArray[i++];
            TrainWillNotOpenDoor1 = byteArray[i++];
            LastTrainOfTheOperatingDay1 = byteArray[i++];
            TrainNotInService1 = byteArray[i++];
            LineOperationMode1 = byteArray[i++];
            TestTrain1 = byteArray[i++];
            TrainDirection1 = byteArray[i++];
            Spare3 = byteArray[i++];
            ValidityField2 = byteArray[i++];
            NumberOfCars2 = byteArray[i++];
            TrainUnitID2 = byteArray[i++];
            ServiceNumber2 = byteArray[i++];
            TripNumber2 = byteArray[i++];
            DestinationNumber2 = byteArray[i++];
            ArrivalTime2 = ((long)byteArray[i++] << 16) | byteArray[i++];
            DepartureTime2 = ((long)byteArray[i++] << 16) | byteArray[i++];
            DelayAtArrival2 = byteArray[i++];
            DelayAtDeparture2 = byteArray[i++];
            CancelledTrain2 = byteArray[i++];
            NextTrainWillNotStop2 = byteArray[i++];
            TrainEndOfService2 = byteArray[i++];
            TrainWillNotOpenDoor2 = byteArray[i++];
            LastTrainOfTheOperatingDay2 = byteArray[i++];
            TrainNotInService2 = byteArray[i++];
            LineOperationMode2 = byteArray[i++];
            TestTrain2 = byteArray[i++];
            TrainDirection2 = byteArray[i++];
            Spare4 = byteArray[i++];
        }


        public bool TryConnectAndReadData(OCSModbusReader data)
        {
            int attempt = 0;

            while (attempt < data.ConnectionTries)
            {
                try
                {
                    // 建立 Modbus TCP 客戶端
                    var master = new ModbusTcpClient();
                    master.ReadTimeout = data.TransactionTimeout;
                    master.Connect(data.ClientIP, data.Port);

                    data._master = master;

                    // 讀取資料
                    data.RegisterBuffer = master.ReadHoldingRegisters(data.SlaveAddress, 0, data.NumberOfPoints);

                    ASI.Lib.Log.DebugLog.Log("OCS_Connection", $"Successfully connected and read data from {data.ClientIP} on attempt {attempt + 1}.");
                    return true; // 成功
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
