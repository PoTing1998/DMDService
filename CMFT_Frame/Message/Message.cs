using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ASI.Wanda.CMFT.Message
{
    public class Message
    {
        #region Constructor
        public Message()
        {

        }
        public Message(eMessageType messageType, int messageID, string jsonContent)
        {
            MessageType = messageType;
            MessageID = messageID;
            JsonContent = jsonContent;
        }
        #endregion

        #region 起始識別碼
        public readonly byte HEAD = 0xAC;
        #endregion

        #region 訊息類別碼
        public enum eMessageType
        {
            Heartbeat = 0x00,
            Ack = 0x01,
            Command = 0x02,
            Response = 0x03,
            ResponseOver = 0x04,
            Report = 0x05
        }
        public eMessageType MessageType { get; set; }
        #endregion

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
                mID = ASI.Lib.Msg.Parsing.ByteArray.Int16ToBytes((Int16)mMessageID, ASI.Lib.Enum.Endianness.BigEndian);
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
                    mMessageLength = Content.Length;

                    //固定為2個byte，所以需先轉成Int16再轉成byte[]
                    mLength = ASI.Lib.Msg.Parsing.ByteArray.Int16ToBytes((Int16)mMessageLength, ASI.Lib.Enum.Endianness.BigEndian);

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
        /// 取得或設定Json訊息以UTF-8編譯之byte[]內容
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

        #region CRC16檢查碼
        /// <summary>
        /// 設定或取得CRC16檢查碼，固定為2個byte。
        /// </summary>
        public byte[] CRC16 { get; set; }

        /// <summary>
        /// 依據目前封包內容取得檢查碼(CRC16檢查碼校驗範圍：起始識別碼 + 訊息類別碼 + 訊息識別碼 + 訊息長度 + 訊息內容)，固定為2個byte。
        /// </summary>
        /// <returns></returns>
        public byte[] GenCRC16()
        {
            byte[] bCRC16 = null;

            try
            {
                List<byte> oTempByteList = new List<byte>(100);

                oTempByteList.Add(HEAD);
                oTempByteList.Add((byte)MessageType);
                oTempByteList.AddRange(ID);
                oTempByteList.AddRange(LEN);
                oTempByteList.AddRange(Content);

                ASI.Lib.Msg.Parsing.CRC16 oCRC16 = new ASI.Lib.Msg.Parsing.CRC16();
                oCRC16.GetChecksum(out bCRC16, oTempByteList.ToArray());
                return bCRC16;
            }
            catch (System.Exception)
            {
                return new byte[2];
            }
        }
        #endregion

        #region 封包結尾
        /// <summary>
        /// 封包結尾，固定為0xA9
        /// </summary>
        public readonly byte TAIL = 0xA9;
        #endregion

    }
}
