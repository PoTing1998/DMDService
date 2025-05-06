using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.Message
{
    public class Message
    {
        #region Constructor
        public Message() { }

        public Message(eMessageType messageType, int messageID, string jsonContent)
        {
            MessageType = messageType;
            MessageID = messageID;
            JsonContent = jsonContent;
        }

        
        #endregion

        #region property
        /// <summary>
        /// 起始識別碼，固定為0xAC 
        /// </summary>
        public readonly byte HEAD = 0xAC;



        public enum eMessageType
        {
            Heartbeat = 0x00,
            Ack = 0x01,
            Command = 0x02,
            Response = 0x03,
            ResponseOver = 0x04,
            trainMessage = 0x05
        }

        /// <summary>
        /// 訊息類別碼，分別為Ack = 0x01；Change/Command = 0x02；Response = 0x03 
        /// </summary>
        public eMessageType MessageType;

        #region 訊息識別碼

        private int mMessageID = 1;

        /// <summary>
        /// 取得或設定訊息識別碼(1~65535) 
        /// </summary>
        public int MessageID
        {
            get
            {
                return mMessageID;
            }
            set
            {
                mMessageID = value;
                mID = BitConverter.GetBytes((short)mMessageID);
            }
        }

        private byte[] mID = null;

        /// <summary>
        /// 取得或設定訊息識別碼(2 bytes)
        /// </summary>
        public byte[] ID
        {
            get
            {
                return mID;
            }
            set
            {
                if (value != null && value.Length == 2)
                {
                    mID = value;
                    mMessageID = (mID[0] * 256) + mID[1];
                }
            }
        }

        #endregion

        #region 訊息長度

        private int mMessageLength = 0;

        /// <summary>
        /// 訊息長度 = 訊息內容byte數量
        /// </summary>
        public int MessageLength
        {
            get 
            {
                return mMessageLength;
            }
        }

        private byte[] mLength = new byte[2];

        /// <summary>
        /// 取得或設定長度(訊息內容byte數量)，固定為2個byte。
        /// 設定後同時影響MessageLength的值
        /// </summary>
        public byte[] LEN
        {
            get
            {
                try
                {
                    byte[] arrByte = null;
                    mMessageLength = Content.Length;

                    //固定為2個byte，所以需先轉成short再轉成byte[]
                    arrByte = BitConverter.GetBytes((short)MessageLength);

                    //byte[]需反轉
                    mLength = arrByte.Reverse().ToArray();

                    return mLength;
                }
                catch (System.Exception ex)
                {
                    return new byte[2];
                }
            }
            set
            {
                if (value != null && value.Length == 2)
                {
                    mLength = value;
                    mMessageLength = (mLength[0] * 256) + mLength[1];
                }
            }
        }

        #endregion

        #region 訊息內容

        private string mJsonContent = null;

        /// <summary>
        /// 取得或設定Json訊息內容(Json格式)
        /// </summary>
        public string JsonContent
        {
            get
            {
                return mJsonContent;
            }
            set
            {
                mJsonContent = value;
                if (value != null)
                {
                    mContent = Encoding.UTF8.GetBytes(value);
                }
                else
                {
                    mContent = null;
                }
            }
        }

        private byte[] mContent = null;

        /// <summary>
        /// 取得或設定Json訊息內容 (byte[] UTF-8)
        /// </summary>
        public byte[] Content
        {
            get
            {                
                return mContent;
            }
            set
            {
                mContent = value;
                if (value != null)
                {
                    mJsonContent = Encoding.UTF8.GetString(mContent);
                }
                else
                {
                    mJsonContent = null;
                }
            }
        }

        /// <summary>
        /// 取得或設定完整封包的內容
        /// </summary>
        public byte[] CompleteContent { set; get; }
        #endregion

        /// <summary>
        /// 設定或取得CRC16檢查碼，固定為2個byte。 
        /// </summary>
        public byte[] CRC16 { get; set; }
       
        /// <summary>
        /// 依據目前所在封包內容取得檢查碼(CRC16檢查碼校驗範圍：起始識別碼 + 訊息類別碼 + 訊息識別碼 + 訊息長度 + 訊息內容)，固定為2個byte。  
        /// </summary>
        /// <returns></returns>
        public byte[] GenCRC16()
        {
            byte[] bCRC16 = null;

            try
            {
                List<byte> oTempByteList = new List<byte>();

                oTempByteList.Add(HEAD);
                oTempByteList.Add((byte)MessageType);
                oTempByteList.AddRange(ID);
                oTempByteList.AddRange(LEN);
                oTempByteList.AddRange(Content);

                ASI.Lib.Msg.Parsing.CRC16 oCRC16 = new Lib.Msg.Parsing.CRC16();
                oCRC16.GetChecksum(out bCRC16, oTempByteList.ToArray());
                return bCRC16;
            }
            catch (System.Exception ex)
            {
                return new byte[2];
            }
        }
        /// <summary>
        /// 封包結尾，固定為0xA9
        /// </summary>
        public readonly byte TAIL = 0xA9;

        #endregion
    }
}
