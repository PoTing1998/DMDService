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
        public int Port { get; set; } = 502;
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
    }

    /// <summary>
    /// OCS 月台資料模型。
    /// 資料來源：OCS-COM-004 Rev.D，Section 8.1 / 8.2。
    /// 38 個 Modbus Input Register（76 bytes），所有多 byte 欄位為 Little-Endian。
    /// </summary>
    public class OCSPlatform
    {
        #region Properties — Header（bytes 0–11）
        public int NumberOfPlatforms { get; set; }  // byte 0
        public int Spare1            { get; set; }  // byte 1
        public int PlatformID        { get; set; }  // bytes 2–3  (2 bytes LE)
        public int PreArrival        { get; set; }  // byte 4
        public int Arrival           { get; set; }  // byte 5
        public int PreDeparture      { get; set; }  // byte 6
        public int Departure         { get; set; }  // byte 7
        public int Skip              { get; set; }  // byte 8
        public int Hold              { get; set; }  // byte 9
        public int NumberOfJourneyData { get; set; } // byte 10
        public int Spare2            { get; set; }  // byte 11
        #endregion

        #region Properties — Journey 1（bytes 12–43）
        public int  ValidityField1          { get; set; }  // byte 12
        public int  NumberOfCars1           { get; set; }  // byte 13
        public int  TrainUnitID1            { get; set; }  // bytes 14–15 (2 bytes LE)
        public int  ServiceNumber1          { get; set; }  // bytes 16–17 (2 bytes LE)
        public int  TripNumber1             { get; set; }  // bytes 18–19 (2 bytes LE)
        public int  DestinationNumber1      { get; set; }  // bytes 20–21 (2 bytes LE)
        public long ArrivalTime1            { get; set; }  // bytes 22–25 (4 bytes LE, seconds since 2018-01-01)
        public long DepartureTime1          { get; set; }  // bytes 26–29 (4 bytes LE)
        public int  DelayAtArrival1         { get; set; }  // bytes 30–31 (2 bytes LE)
        public int  DelayAtDeparture1       { get; set; }  // bytes 32–33 (2 bytes LE)
        public int  CancelledTrain1         { get; set; }  // byte 34
        public int  NextTrainWillNotStop1   { get; set; }  // byte 35
        public int  TrainEndOfService1      { get; set; }  // byte 36
        public int  TrainWillNotOpenDoor1   { get; set; }  // byte 37
        public int  LastTrainOfTheOperatingDay1 { get; set; } // byte 38
        public int  TrainNotInService1      { get; set; }  // byte 39
        public int  LineOperationMode1      { get; set; }  // byte 40
        public int  TestTrain1              { get; set; }  // byte 41
        public int  TrainDirection1         { get; set; }  // byte 42
        public int  Spare3                  { get; set; }  // byte 43
        #endregion

        #region Properties — Journey 2（bytes 44–75）
        public int  ValidityField2          { get; set; }  // byte 44
        public int  NumberOfCars2           { get; set; }  // byte 45
        public int  TrainUnitID2            { get; set; }  // bytes 46–47 (2 bytes LE)
        public int  ServiceNumber2          { get; set; }  // bytes 48–49 (2 bytes LE)
        public int  TripNumber2             { get; set; }  // bytes 50–51 (2 bytes LE)
        public int  DestinationNumber2      { get; set; }  // bytes 52–53 (2 bytes LE)
        public long ArrivalTime2            { get; set; }  // bytes 54–57 (4 bytes LE)
        public long DepartureTime2          { get; set; }  // bytes 58–61 (4 bytes LE)
        public int  DelayAtArrival2         { get; set; }  // bytes 62–63 (2 bytes LE)
        public int  DelayAtDeparture2       { get; set; }  // bytes 64–65 (2 bytes LE)
        public int  CancelledTrain2         { get; set; }  // byte 66
        public int  NextTrainWillNotStop2   { get; set; }  // byte 67
        public int  TrainEndOfService2      { get; set; }  // byte 68
        public int  TrainWillNotOpenDoor2   { get; set; }  // byte 69
        public int  LastTrainOfTheOperatingDay2 { get; set; } // byte 70
        public int  TrainNotInService2      { get; set; }  // byte 71
        public int  LineOperationMode2      { get; set; }  // byte 72
        public int  TestTrain2              { get; set; }  // byte 73
        public int  TrainDirection2         { get; set; }  // byte 74
        public int  Spare4                  { get; set; }  // byte 75
        #endregion

        #region Methods

        /// <summary>
        /// 從 38 個 Modbus Input Register 解析所有欄位。
        /// 每個 register 為 big-endian ushort，多 byte 欄位為 Little-Endian。
        /// 依據 OCS-COM-004 Rev.D Section 8.1/8.2。
        /// </summary>
        public void UpdateFromUShortArray(ushort[] data)
        {
            if (data == null || data.Length < 38)
                throw new ArgumentException("資料長度不足（需要至少 38 個 ushort）");

            // 轉成 byte[76]，每個 ushort 高位元組在前
            byte[] b = new byte[data.Length * 2];
            for (int j = 0; j < data.Length; j++)
            {
                b[j * 2]     = (byte)(data[j] >> 8);
                b[j * 2 + 1] = (byte)(data[j] & 0xFF);
            }

            // ── Header ────────────────────────────────────────────
            NumberOfPlatforms  = b[0];
            Spare1             = b[1];
            PlatformID         = ReadLE16(b, 2);
            PreArrival         = b[4];
            Arrival            = b[5];
            PreDeparture       = b[6];
            Departure          = b[7];
            Skip               = b[8];
            Hold               = b[9];
            NumberOfJourneyData = b[10];
            Spare2             = b[11];

            // ── Journey 1 ─────────────────────────────────────────
            ValidityField1         = b[12];
            NumberOfCars1          = b[13];
            TrainUnitID1           = ReadLE16(b, 14);
            ServiceNumber1         = ReadLE16(b, 16);
            TripNumber1            = ReadLE16(b, 18);
            DestinationNumber1     = ReadLE16(b, 20);
            ArrivalTime1           = ReadLE32(b, 22);
            DepartureTime1         = ReadLE32(b, 26);
            DelayAtArrival1        = ReadLE16(b, 30);
            DelayAtDeparture1      = ReadLE16(b, 32);
            CancelledTrain1        = b[34];
            NextTrainWillNotStop1  = b[35];
            TrainEndOfService1     = b[36];
            TrainWillNotOpenDoor1  = b[37];
            LastTrainOfTheOperatingDay1 = b[38];
            TrainNotInService1     = b[39];
            LineOperationMode1     = b[40];
            TestTrain1             = b[41];
            TrainDirection1        = b[42];
            Spare3                 = b[43];

            // ── Journey 2 ─────────────────────────────────────────
            ValidityField2         = b[44];
            NumberOfCars2          = b[45];
            TrainUnitID2           = ReadLE16(b, 46);
            ServiceNumber2         = ReadLE16(b, 48);
            TripNumber2            = ReadLE16(b, 50);
            DestinationNumber2     = ReadLE16(b, 52);
            ArrivalTime2           = ReadLE32(b, 54);
            DepartureTime2         = ReadLE32(b, 58);
            DelayAtArrival2        = ReadLE16(b, 62);
            DelayAtDeparture2      = ReadLE16(b, 64);
            CancelledTrain2        = b[66];
            NextTrainWillNotStop2  = b[67];
            TrainEndOfService2     = b[68];
            TrainWillNotOpenDoor2  = b[69];
            LastTrainOfTheOperatingDay2 = b[70];
            TrainNotInService2     = b[71];
            LineOperationMode2     = b[72];
            TestTrain2             = b[73];
            TrainDirection2        = b[74];
            Spare4                 = b[75];
        }

        /// <summary>Little-Endian 16-bit unsigned integer</summary>
        private static int ReadLE16(byte[] b, int offset)
        {
            return b[offset] | (b[offset + 1] << 8);
        }

        /// <summary>Little-Endian 32-bit unsigned integer（時間欄位用）</summary>
        private static long ReadLE32(byte[] b, int offset)
        {
            return (long)b[offset]
                 | ((long)b[offset + 1] << 8)
                 | ((long)b[offset + 2] << 16)
                 | ((long)b[offset + 3] << 24);
        }

        /// <summary>XOR 兩個 ushort 陣列（用於資料比對）</summary>
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
