using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.Message
{
    public class Helper
    {
        /// <summary>
        /// 共用的訊息識別碼
        /// </summary>
        private static int mMessageID = 0;

        /// <summary>
        /// 取得共用的訊息識別碼 
        /// </summary>
        /// <returns></returns>
        public static int GetMessageID()
        {
            if (mMessageID < 65535)
            {
                mMessageID++;
            }
            else
            {
                mMessageID = 1;
            }

            return mMessageID;
        }

        /// <summary>
        /// 將ASI.Wanda.CMFT.Message.Message物件轉成byte[]封包
        /// </summary>
        /// <param name="PAmessage"></param>
        /// <returns></returns>
        public static byte[] Pack(Message CMFTmessage)
        {
            byte[] arrReturn = new byte[] { };
            List<byte> oByteList = new List<byte>(100);
            List<byte> oSendByteList = new List<byte>(100);

            try
            {
                //訊息識別碼
                //byte[] byteID = CutApart(CMFTmessage.ID);
                oByteList.AddRange(CMFTmessage.ID);

                //TSAI　2023/01/10 Delete Start 
                //設定訊息長度前須先將0xAX 轉成 0xA0,0x0X
                //CMFTmessage.Content = ASI.Wanda.CMFT.Message.Helper.CutApart(CMFTmessage.Content);
                //TSAI　2023/01/10 Delete End

                //訊息長度
                if (CMFTmessage.MessageType != Message.eMessageType.Ack)
                {
                    if (CMFTmessage.LEN != null)
                    {
                        //byte[] byteLEN = CutApart(CMFTmessage.LEN);
                        oByteList.AddRange(CMFTmessage.LEN);
                    }

                    //訊息內容
                    if (CMFTmessage.Content != null)
                    {
                        //TSAI　2023/01/10 Add Start 
                        //設定訊息長度前須先將0xAX 轉成 0xA0,0x0X
                        //CMFTmessage.Content = CutApart(CMFTmessage.Content);
                        //TSAI　2023/01/10 Add End
                        oByteList.AddRange(CMFTmessage.Content);
                    }

                    //建立CRC16檢查碼
                    CMFTmessage.CRC16 = CMFTmessage.GenCRC16();
                    oByteList.AddRange(CMFTmessage.CRC16);
                }

                //訊息識別碼、訊息長度、訊息內容、CRC16檢查碼等欄位皆須CutApart特別處理，避免內容包含封包頭尾
                byte[] oByteContent = CutApart(oByteList.ToArray());

                //起始識別碼
                oSendByteList.Add(CMFTmessage.HEAD);

                //訊息類別碼
                oSendByteList.Add((byte)CMFTmessage.MessageType);

                //加入CutApart特別處理過後的訊息識別碼、訊息長度、訊息內容、CRC16檢查碼等欄位
                oSendByteList.AddRange(oByteContent);

                //封包結尾
                oSendByteList.Add(CMFTmessage.TAIL);

                arrReturn = oSendByteList.ToArray();

                return arrReturn;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                arrReturn = null;
                oByteList = null;
            }
        }

        /// <summary>
        /// 將byte[]封包轉換成ASI.Wanda.CMFT.Message.Message物件
        /// </summary>
        /// <param name="msgBytes">包含起始識別碼(0xAC)及封包結尾(0xA9)的完整byte[]封包</param>
        /// <returns></returns>
        public static Message UnPack(byte[] msgBytes)
        {
            Message oReturn = null;

            try
            {
                oReturn = new Message();

                //先將資料內容中所有 0xA0,0x0X 組合(2 byte)都還原成0xAX
                msgBytes = UnCutApart(msgBytes);

                oReturn.CompleteContent = msgBytes;

                //訊息類別碼
                oReturn.MessageType = (Message.eMessageType)msgBytes[1];

                //訊息識別碼
                oReturn.ID = ASI.Lib.Msg.Parsing.ByteArray.Capture(msgBytes, 2, 2);

                if (oReturn.MessageType != Message.eMessageType.Ack)
                {
                    //訊息長度
                    oReturn.LEN = ASI.Lib.Msg.Parsing.ByteArray.Capture(msgBytes, 4, 2);

                    //訊息內容
                    oReturn.Content = ASI.Lib.Msg.Parsing.ByteArray.Capture(msgBytes, 6, oReturn.MessageLength);

                    //CRC16檢查碼
                    oReturn.CRC16 = ASI.Lib.Msg.Parsing.ByteArray.Capture(msgBytes, oReturn.MessageLength + 6, 2);
                }

                return oReturn;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                oReturn = null;
            }
        }

        /// <summary>
        /// 取得JsonObject物件，需強制型別轉換
        /// </summary>
        /// <param name="jsonData">Json資料內容</param>
        /// <returns></returns>
        public static Object GetJsonObject(string jsonData)
        {
            if (jsonData != null && jsonData != "")
            {
                string sJsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(jsonData, "JsonObjectName");
                System.Type type = System.Type.GetType(sJsonObjectName);
                if (type != null)
                {
                    return ASI.Lib.Text.Parsing.Json.DeserializeObject(jsonData, type);
                }
            }

            return null;
        }

        /// <summary>
        /// 將資料內容中所有0xAX都轉成 0xA0,0x0X組合(2 byte)
        /// </summary>
        /// <param name="byteArray">資料內容</param>
        /// <returns></returns>
        public static byte[] CutApart(byte[] byteArray)
        {
            List<byte> oByteList = new List<byte>();
            for (int i = 0; i < byteArray.Length; ++i)
            {
                if (byteArray[i] >= 0xA0 && byteArray[i] <= 0xAF)
                {
                    oByteList.Add(0xA0);
                    oByteList.Add((byte)(byteArray[i] - 0xA0));
                }
                else
                {
                    oByteList.Add(byteArray[i]);
                }
            }
            return oByteList.ToArray();
        }

        /// <summary>
        /// 將資料內容中所有 0xA0,0x0X 組合(2 byte)都還原成0xAX
        /// </summary>
        /// <param name="byteArray">資料內容</param>
        /// <returns></returns>
        public static byte[] UnCutApart(byte[] byteArray)
        {
            if (byteArray != null && byteArray.Length >= 2)
            {
                byte[] bArray = new byte[byteArray.Length];

                int j = 0;
                for (int ii = 0; ii <= byteArray.Length - 1; ++ii)
                {
                    byte bTemp = byteArray[ii];
                    if (bTemp == 0xA0)
                    {
                        bTemp = (byte)(bTemp + byteArray[ii + 1]);
                        ii = ii + 1;
                    }

                    bArray[j] = bTemp;
                    j = j + 1;
                }

                byte[] bRtn = new byte[j];
                for (int ii = 0; ii < j; ++ii)
                {
                    bRtn[ii] = bArray[ii];
                }

                return bRtn;
            }
            else
            {
                return byteArray;
            }
        }
    }
}
