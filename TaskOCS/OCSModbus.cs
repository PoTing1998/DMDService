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

        private void AssignFromByteArray(byte[] byteArray)
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                NumberOfPlatforms = reader.ReadInt16();
                Spare1 = reader.ReadInt16();
                PlatformID = reader.ReadInt32();
                PreArrival = reader.ReadInt16();
                Arrival = reader.ReadInt16();
                PreDeparture = reader.ReadInt16();
                Departure = reader.ReadInt16();
                Skip = reader.ReadInt16();
                Hold = reader.ReadInt16();
                NumberOfJourneyData = reader.ReadInt16();
                Spare2 = reader.ReadInt16();
                ValidityField1 = reader.ReadInt16();
                NumberOfCars1 = reader.ReadInt16();
                TrainUnitID1 = reader.ReadInt32();
                ServiceNumber1 = reader.ReadInt32();
                TripNumber1 = reader.ReadInt32();
                DestinationNumber1 = reader.ReadInt32();
                ArrivalTime1 = reader.ReadInt64();
                DepartureTime1 = reader.ReadInt64();
                DelayAtArrival1 = reader.ReadInt32();
                DelayAtDeparture1 = reader.ReadInt32();
                CancelledTrain1 = reader.ReadInt16();
                NextTrainWillNotStop1 = reader.ReadInt16();
                TrainEndOfService1 = reader.ReadInt16();
                TrainWillNotOpenDoor1 = reader.ReadInt16();
                LastTrainOfTheOperatingDay1 = reader.ReadInt16();
                TrainNotInService1 = reader.ReadInt16();
                LineOperationMode1 = reader.ReadInt16();
                TestTrain1 = reader.ReadInt16();
                TrainDirection1 = reader.ReadInt16();
                Spare3 = reader.ReadInt16();
                ValidityField2 = reader.ReadInt16();
                NumberOfCars2 = reader.ReadInt16();
                TrainUnitID2 = reader.ReadInt32();
                ServiceNumber2 = reader.ReadInt32();
                TripNumber2 = reader.ReadInt32();
                DestinationNumber2 = reader.ReadInt32();
                ArrivalTime2 = reader.ReadInt64();
                DepartureTime2 = reader.ReadInt64();
                DelayAtArrival2 = reader.ReadInt32();
                DelayAtDeparture2 = reader.ReadInt32();
                CancelledTrain2 = reader.ReadInt16();
                NextTrainWillNotStop2 = reader.ReadInt16();
                TrainEndOfService2 = reader.ReadInt16();
                TrainWillNotOpenDoor2 = reader.ReadInt16();
                LastTrainOfTheOperatingDay2 = reader.ReadInt16();
                TrainNotInService2 = reader.ReadInt16();
                LineOperationMode2 = reader.ReadInt16();
                TestTrain2 = reader.ReadInt16();
                TrainDirection2 = reader.ReadInt16();
                Spare4 = reader.ReadInt16();
            }
        }

        private void WriteLog(ushort startAddress, ushort[] ocsData)
        {
            string logMessage = $"Current Address {startAddress}:\n";
            int i = 1;
            foreach (var data in ocsData)
            {
                logMessage += $"Received data {i} (ushort): {data}\n";
                i++;
            }
            ASI.Lib.Log.DebugLog.Log("To_OCS_Data", logMessage);
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
