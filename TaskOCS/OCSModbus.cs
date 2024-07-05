using NModbus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;



namespace OCS.Modbus
{
    public class OCS_Data
    {

        
        public Configuration _mConfig;
        public ModbusFactory _mModbusFactory;
        public IModbusMaster _mMaster;
        public ushort[] _mRegisterBuffer;
        public byte _mSlaveAddress = 0;
        public ushort _mMumberOfPoints = 38;
        public int _mPort = 502;
        public string _Client1IP;
        public int _mTransactionTimeout = 10;
        public int _mConnectionTries;
        public int _mWaitToRetryMilliseconds;

        public OCS_Data()
        {

        }

        public OCS_Data( string Client1IP , byte SlaveAddress)
        {
            _Client1IP = Client1IP;
            _mSlaveAddress = SlaveAddress;
        }
    }

    public class OCSPlatform
    {
        #region property
        private int mNumberofPlatform; 
        public int NumberofPlatform
        {
            get { return mNumberofPlatform; }
            set { mNumberofPlatform = value; }
        }

        private int mSpare1; 
        public int Spare1
        {
            get { return mSpare1; }
            set { mSpare1 = value; }
        }

        private int mPlatofrmID;
        public int PlatofrmID
        {
            get { return mPlatofrmID; }
            set { mPlatofrmID = value; }
        }
        private int mPre_Arrival;
        public int Pre_Arrival
        {
            get { return mPre_Arrival; }
            set { mPre_Arrival = value; }
        }
        private int mArrival;
        public int Arrival
        {
            get { return mArrival; }
            set { mArrival = value; }
        }
        private int mPre_Departure;
        public int Pre_Departure
        {
            get { return mPre_Departure; }
            set { mPre_Departure = value; }
        }
        private int mDeparture;
        public int Departure
        {
            get { return mDeparture; }
            set { mDeparture = value; }
        }
        private int mSkip;
        public int Skip
        {
            get { return mSkip; }
            set { mSkip = value; }
        }
        private int mHold;
        public int Hold
        {
            get { return mHold; }
            set { mHold = value; }
        }
        private int mNumberOfJourneydata;
        public int NumberOfJourneydata
        {
            get { return mNumberOfJourneydata; }
            set { mNumberOfJourneydata = value; }
        }
        private int mSpare2;
        public int Spare2
        {
            get { return mSpare2; }
            set { mSpare2 = value; }
        }

        private int mValidity_Field_1;
        public int Validity_Field_1
        {
            get { return mValidity_Field_1; }
            set { mValidity_Field_1 = value; }
        }
        private int mNumber_Of_Cars_1;
        public int Number_Of_Cars_1
        {
            get { return mNumber_Of_Cars_1; }
            set { mNumber_Of_Cars_1 = value; }
        }

        private int mTrain_Unit_ID_1;
        public int Train_Unit_ID_1
        {
            get { return mTrain_Unit_ID_1; }
            set { mTrain_Unit_ID_1 = value; }
        }

        private int mService_Number_1;
        public int Service_Number_1
        {
            get { return mService_Number_1; }
            set { mService_Number_1 = value; }
        }
        private int mTrip_Number_1;
        public int Trip_Number_1
        {
            get { return mTrip_Number_1; }
            set { mTrip_Number_1 = value; }
        }
        private int mDestination_Number_1;
        public int Destination_Number_1
        {
            get { return mDestination_Number_1; }
            set { mDestination_Number_1 = value; }
        }
        private long mArrivalTime_1;
        public long ArrivalTime_1
        {
            get { return mArrivalTime_1; }
            set { mArrivalTime_1 = value; }
        }

        private long mDepartureTime_1;
        public long DepartureTime_1
        {
            get { return mDepartureTime_1; }
            set { mDepartureTime_1 = value; }
        }
        private int mDelayAtArrival_1;
        public int DelayAtArrival_1
        {
            get { return mDelayAtArrival_1; }
            set { mDelayAtArrival_1 = value; }
        }

        private int mDelayAtDeparture_1;
        public int DelayAtDeparture_1
        {
            get { return mDelayAtDeparture_1; }
            set { mDelayAtDeparture_1 = value; }
        }
        private int mCancelledTrain_1;
        public int CancelledTrain_1
        {
            get { return mCancelledTrain_1; }
            set { mCancelledTrain_1 = value; }
        }
        private int mNextTrainWillnotStop_1;
        public int NextTrainWillnotStop_1
        {
            get { return mNextTrainWillnotStop_1; }
            set { mNextTrainWillnotStop_1 = value; }
        }
        private int mTrainEnd_Of_Service_1;
        public int TrainEnd_Of_Service_1
        {
            get { return mTrainEnd_Of_Service_1; }
            set { mTrainEnd_Of_Service_1 = value; }
        }
        private int mTrainWillNotOpenDoor_1;
        public int TrainWillNotOpenDoor_1
        {
            get { return mTrainWillNotOpenDoor_1; }
            set { mTrainWillNotOpenDoor_1 = value; }
        }
        private int mLastTrainOfTheOperatingDay_1;
        public int LastTrainOfTheOperatingDay_1
        {
            get { return mLastTrainOfTheOperatingDay_1; }
            set { mLastTrainOfTheOperatingDay_1 = value; }
        }
        private int mTrain_Not_in_service_1;
        public int Train_Not_in_service_1
        {
            get { return mTrain_Not_in_service_1; }
            set { mTrain_Not_in_service_1 = value; }
        }
        private int mLine_Operation_Mode_1;
        public int Line_Operation_Mode_1
        {
            get { return mLine_Operation_Mode_1; }
            set { mLine_Operation_Mode_1 = value; }
        }

        private int mTest_Train_1;
        public int Test_Train_1
        {
            get { return mTest_Train_1; }
            set { mTest_Train_1 = value; }
        }
        private int mTrain_Direction_1;
        public int Train_Direction_1
        {
            get { return mTrain_Direction_1; }
            set { mTrain_Direction_1 = value; }
        }
        private int mSpare3;
        public int Spare3
        {
            get { return mSpare3; }
            set { mSpare3 = value; }
        }

        /// 剩下第二台車輛的資訊 0718
        private int mValidity_Field_2;
        public int Validity_Field_2
        {
            get { return mValidity_Field_2; }
            set { mValidity_Field_2 = value; }
        }

        private int mNumber_Of_Cars_2;
        public int Number_Of_Cars_2
        {
            get { return mNumber_Of_Cars_2; }
            set { mNumber_Of_Cars_2 = value; }
        }
        private int mTrain_Unit_ID_2;
        public int Train_Unit_ID_2
        {
            get { return mTrain_Unit_ID_2; }
            set { mTrain_Unit_ID_2 = value; }
        }
        private int mService_Number_2;
        public int Service_Number_2
        {
            get { return mService_Number_2; }
            set { mService_Number_2 = value; }
        }
        private int mTrip_Number_2;
        public int Trip_Number_2
        {
            get { return mTrip_Number_2; }
            set { mTrip_Number_2 = value; }
        }
        private int mDestination_Number_2;
        public int Destination_Number_2
        {
            get { return mDestination_Number_2; }
            set { mDestination_Number_2 = value; }
        }
        private long mArrivalTime_2;
        public long ArrivalTime_2
        {
            get { return mArrivalTime_2; }
            set { mArrivalTime_2 = value; }
        }
        private long mDepartureTime_2;
        public long DepartureTime_2
        {
            get { return mDepartureTime_2; }
            set { mDepartureTime_2 = value; }
        }
        private int mDelayAtArrival_2;
        public int DelayAtArrival_2
        {
            get { return mDelayAtArrival_2; }
            set { mDelayAtArrival_2 = value; }
        }

        private int mDelayAtDeparture_2;
        public int DelayAtDeparture_2
        {
            get { return mDelayAtDeparture_2; }
            set { mDelayAtDeparture_2 = value; }
        }
        private int mCancelledTrain_2;
        public int CancelledTrain_2
        {
            get { return mCancelledTrain_2; }
            set { mCancelledTrain_2 = value; }
        }
        private int mNextTrainWillnotStop_2;
        public int NextTrainWillnotStop_2
        {
            get { return mNextTrainWillnotStop_2; }
            set { mNextTrainWillnotStop_2 = value; }
        }

        private int mTrainEnd_Of_Service_2;
        public int TrainEnd_Of_Service_2
        {
            get { return mTrainEnd_Of_Service_2; }
            set { mTrainEnd_Of_Service_2 = value; }
        }
        private int mTrainWillNotOpenDoor_2;
        public int TrainWillNotOpenDoor_2
        {
            get { return mTrainWillNotOpenDoor_2; }
            set { mTrainWillNotOpenDoor_2 = value; }
        }
        private int mLastTrainOfTheOperatingDay_2;
        public int LastTrainOfTheOperatingDay_2
        {
            get { return mLastTrainOfTheOperatingDay_2; }
            set { mLastTrainOfTheOperatingDay_2 = value; }
        }

        private int mTrain_Not_in_service_2;
        public int Train_Not_in_service_2
        {
            get { return mTrain_Not_in_service_2; }
            set { mTrain_Not_in_service_2 = value; }
        }
        private int mLine_Operation_Mode_2;
        public int Line_Operation_Mode_2
        {
            get { return mLine_Operation_Mode_2; }
            set { mLine_Operation_Mode_2 = value; }
        }
        private int mTest_Train_2;
        public int Test_Train_2
        {
            get { return mTest_Train_2; }
            set { mTest_Train_2 = value; }
        }
        private int mTrain_Direction_2;
        public int Train_Direction_2
        {
            get { return mTrain_Direction_2; }
            set { mTrain_Direction_2 = value; }
        }
        private int mSpare4;
        public int Spare4
        {
            get { return mSpare4; }
            set { mSpare4 = value; }
        }
        #endregion
        #region constructor
        private ushort mStartAddress;
        public ushort StartAddress
        {
            get { return mStartAddress; }
            set { mStartAddress = value; }
        }


        private ushort[] mRegisterBuffer; //OCS 的原始資料    
        public ushort[] RegisterBuffer
        {
            get { return mRegisterBuffer; }
            set { mRegisterBuffer = value; }
        }

        /// <summary>
        /// 全部月台的資料  
        /// </summary> 
        public static Dictionary<ushort, OCSPlatform> PlatformData = new Dictionary<ushort, OCSPlatform>();
        private static List<byte[]> PlatformByteData;
        private byte[] ProcessByteArray;
        ///預設初始化的狀態  
        public OCSPlatform(ushort startAddress, ushort[] RegisterBuffer)
        {
            this.mStartAddress = startAddress;
            this.mRegisterBuffer = RegisterBuffer;
            PlatformByteData = new List<byte[]>();
            ProcessByteArray = new byte[] { };
        }
        #endregion
        #region method
        private byte[] Process(ushort[] registerBuffer) ///將號誌原始的資料 進行拆解成新的byteArrays 
        {
            byte[] newByteArray = new byte[registerBuffer.Length * 2]; 
            int byteListIndex = 0;
            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i))
                {
                    byte[] bytes = CombineByte(registerBuffer[i], registerBuffer[i + 1]);
                    newByteArray[byteListIndex] = bytes[0];
                    newByteArray[byteListIndex + 1] = bytes[1];
                    newByteArray[byteListIndex + 2] = bytes[2];
                    newByteArray[byteListIndex + 3] = bytes[3];
                    byteListIndex += 4;
                    i++; // 跳過下一個索引  
                }
                else
                {
                    byte[] ushortBytes = BitConverter.GetBytes(registerBuffer[i]);
                    newByteArray[byteListIndex] = ushortBytes[0];
                    newByteArray[byteListIndex + 1] = ushortBytes[1];
                    byteListIndex += 2; 
                }
            }
            AssignFromByteArray(newByteArray);
            return newByteArray;
        }
        private void AssignFromByteArray(byte[] byteArray) /// 將拆解後的byteArray 分別給對應的property   
        {
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                NumberofPlatform = reader.ReadInt16();
                Spare1 = reader.ReadInt16();
                PlatofrmID = reader.ReadInt32();
                Pre_Arrival = reader.ReadInt16();
                Arrival = reader.ReadInt16();
                Pre_Departure = reader.ReadInt16();
                Departure = reader.ReadInt16();
                Skip = reader.ReadInt16();
                Hold = reader.ReadInt16();
                NumberOfJourneydata = reader.ReadInt16();
                Spare2 = reader.ReadInt16();
                Validity_Field_1 = reader.ReadInt16();
                Number_Of_Cars_1 = reader.ReadInt16();
                Train_Unit_ID_1 = reader.ReadInt32();
                Service_Number_1 = reader.ReadInt32();
                Trip_Number_1 = reader.ReadInt32();
                Destination_Number_1 = reader.ReadInt32();
                ArrivalTime_1 = reader.ReadInt64();
                DepartureTime_1 = reader.ReadInt64();
                DelayAtArrival_1 = reader.ReadInt32();
                DelayAtDeparture_1 = reader.ReadInt32();
                CancelledTrain_1 = reader.ReadInt16();
                NextTrainWillnotStop_1 = reader.ReadInt16();
                TrainEnd_Of_Service_1 = reader.ReadInt16();
                TrainWillNotOpenDoor_1 = reader.ReadInt16();
                LastTrainOfTheOperatingDay_1 = reader.ReadInt16();
                Train_Not_in_service_1 = reader.ReadInt16();
                Line_Operation_Mode_1 = reader.ReadInt16();
                Test_Train_1 = reader.ReadInt16();
                Train_Direction_1 = reader.ReadInt16();
                Spare3 = reader.ReadInt16();
                Validity_Field_2 = reader.ReadInt16();
                Number_Of_Cars_2 = reader.ReadInt16();
                Train_Unit_ID_2 = reader.ReadInt32();
                Service_Number_2 = reader.ReadInt32();
                Trip_Number_2 = reader.ReadInt32();
                Destination_Number_2 = reader.ReadInt32();
                ArrivalTime_2 = reader.ReadInt64();
                DepartureTime_2 = reader.ReadInt64();
                DelayAtArrival_2 = reader.ReadInt32();
                DelayAtDeparture_2 = reader.ReadInt32();
                CancelledTrain_2 = reader.ReadInt16();
                NextTrainWillnotStop_2 = reader.ReadInt16();
                TrainEnd_Of_Service_2 = reader.ReadInt16();
                TrainWillNotOpenDoor_2 = reader.ReadInt16();
                LastTrainOfTheOperatingDay_2 = reader.ReadInt16();
                Train_Not_in_service_2 = reader.ReadInt16();
                Line_Operation_Mode_2 = reader.ReadInt16();
                Test_Train_2 = reader.ReadInt16();
                Train_Direction_2 = reader.ReadInt16();
                Spare4 = reader.ReadInt16();
            }
        }
        void WriteLog(ushort StartAddress, ushort[] OCSdata) ///將蒐集的資料寫進log檔中 
        {
            string logMessage = $"目前的Address{StartAddress}：\n";
            int i = 1;
            var AISData = new byte[] { };
            foreach (var a in OCSdata)
            {
                logMessage += $"接收第{i}個資料 (ushort):{a},拆解後(byte):{AISData[i - 1]}\n";
                i++;
            }
            ASI.Lib.Log.DebugLog.Log("To_OCS_Data", logMessage);

        }
        int CombineBytesToInt(ushort byte1, ushort byte2)  ///兩個byte 組成一個 int    
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(byte2 >> 8);
            bytes[2] = (byte)byte2;
            bytes[1] = (byte)(byte1 >> 8);
            bytes[0] = (byte)byte1;
            return BitConverter.ToInt32(bytes, 0);
        }


        byte[] CombineByte(ushort byte1, ushort byte2) ///將兩個byte的高低位元位置進行變動   
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(byte2 >> 8);
            bytes[2] = (byte)byte2;
            bytes[1] = (byte)(byte1 >> 8);
            bytes[0] = (byte)byte1;
            return bytes;
        }
        bool IsSpecialIndex(int index) ///特定的索引值進行處理  
        {
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }
        //xor算法 
        public static ushort[] XOR(ushort[] bHEX1, ushort[] bHEX2)
        {
            ushort[] bHEX_OUT = new ushort[bHEX1.Length];
            for (int i = 0; i < bHEX1.Length; i++)
            {
                bHEX_OUT[i] = (byte)(bHEX1[i] ^ bHEX2[i]);
            }
            return bHEX_OUT;
        }

        #endregion

    }
}
