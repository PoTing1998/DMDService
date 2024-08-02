using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Parsing
{
    public class String
    {
        /// <summary>
        /// Split input string by delimeter and return an array for split strings.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] Split(
            string input,
            string delimiter)
        {
            if (delimiter.Contains("."))
            {
                return System.Text.RegularExpressions.Regex.Split(input, "\\" + delimiter);
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Split(input, delimiter);
            }
        }

        /// <summary>
        /// Convert Hex string to byte array by every 1 or 2 chars from Hex string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(
            string input,
            int digits)
        {
            try
            {
                char[] charInputs = input.ToCharArray();
                byte[] byteMsg = null;

                for (int ii = 0; ii < charInputs.Length; ii += digits)
                {
                    if ((ii + digits) <= charInputs.Length)
                    {
                        string sSeg = "";

                        // Get digits for converting.
                        for (int jj = 0; jj < digits; jj++)
                        {
                            sSeg += charInputs[ii + jj];
                        }

                        // Resize byte array.
                        if (byteMsg != null)
                        {
                            System.Array.Resize(ref byteMsg, byteMsg.Length + 1);
                        }
                        else
                        {
                            byteMsg = new byte[1];
                        }

                        // Convert string to byte.
                        byteMsg[byteMsg.Length - 1] = System.Convert.ToByte(sSeg, 16);
                    }
                }

                return byteMsg;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convert byte array to Hex string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] input)
        {
            try
            {
                string sRet = null;

                foreach (byte bb in input)
                {
                    sRet += System.String.Format("{0,2:X}", bb).Replace(' ', '0');
                }

                return sRet;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convert byte array to Hex string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="seperate">byte與byte間的分隔字串</param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] input, string seperate)
        {
            try
            {
                string sRet = null;

                for (int ii = 0; ii < input.Length; ii++)
                {
                    byte bb = input[ii];
                    sRet += System.String.Format("{0,2:X}", bb).Replace(' ', '0');
                    if (ii != input.Length - 1)
                    {
                        sRet += seperate;
                    }
                }

                return sRet;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convert byte array to string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] input)
        {
            try
            {
                string sRet = "";

                foreach (byte bb in input)
                {
                    sRet += bb + " ";
                }

                return sRet;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 將字串以某個字元補滿至指定的長度
        /// </summary>
        /// <param name="input">輸入字串</param>
        /// <param name="digits">要補滿的位數</param>
        /// <param name="fillOf">做為補滿的字元</param>
        /// <returns></returns>
        public static string StringFilled(string input, int digits, char fillOf)
        {
            try
            {
                return ASI.Lib.Text.Parsing.String.StringFilled(input, digits, fillOf, true);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 將字串以某個字元補滿至指定的長度
        /// </summary>
        /// <param name="input">輸入字串</param>
        /// <param name="digits">要補滿的位數</param>
        /// <param name="fillOf">做為補滿的字元</param>
        /// <param name="direction">true為向左補滿，false為向右補滿</param>
        /// <returns></returns>
        public static string StringFilled(string input, int digits, char fillOf, bool direction)
        {
            try
            {
                if (input.Length > 0 && digits >= input.Length)
                {
                    int iLess = digits - input.Length;
                    for (int ii = 0; ii < iLess; ii++)
                    {
                        if (direction)
                        {
                            input = fillOf.ToString() + input;
                        }
                        else
                        {
                            input += fillOf.ToString();
                        }
                    }
                    return input;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 將byte[]轉換成ASCII字串
        /// </summary>
        /// <param name="input">輸入byte[]</param>
        /// <returns></returns>
        public static string ByteArrCvt2ASCIIStr(byte[] input)
        {
            string sRtn = "";
            for (int ii = 0; ii < input.Length; ii++)
            {
                byte[] bb2 = null;
                if (ii == input.Length - 1)
                {
                    bb2 = new byte[] { input[ii] };
                }
                else
                {
                    bb2 = new byte[] { input[ii], input[ii + 1] };
                }

                if (IsBig5(bb2))
                {
                    sRtn += Encoding.GetEncoding(950).GetString(bb2);

                    //如果是Big5編碼，則直接跳過下一個byte
                    ii = ii + 1;
                    continue;
                }
                else
                {
                    sRtn += Encoding.ASCII.GetString(new byte[] { input[ii] });
                }
            }

            return sRtn;
        }

        /// <summary>
        /// 是否為Big5碼
        /// </summary>
        /// <param name="input">輸入byte[]</param>
        /// <returns></returns>
        public static bool IsBig5(byte[] input)
        {
            if (input == null ||
                input.Length != 2)
            {
                return false;
            }

            bool bIsBig5 = false;
            int iCode = 0;

            //A440~C67E 常用字
            //C940~F9D5 次常用字 
            //A140~A3BF 特殊符號標準字
            //A3C0~A3E0 特殊符號控制碼
            //C6A1~C8FE 罕用符號區
            iCode = (int)(input[0] << 8);
            iCode |= input[1];

            if ((0xA440 <= iCode && iCode <= 0xC67E) ||
                (0xC940 <= iCode && iCode <= 0xF9D5) ||
                (0xA140 <= iCode && iCode <= 0xA3BF) ||
                (0xA3C0 <= iCode && iCode <= 0xA3E0) ||
                (0xC6A1 <= iCode && iCode <= 0xC8FE))
            {
                bIsBig5 = true;
            }
            else
            {
                bIsBig5 = false;
            }

            return bIsBig5;
        }

        /// <summary>
        /// 是否為大寫英文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCapitalEnglish(string str)
        {
            try
            {
                System.Text.RegularExpressions.Regex eng = new System.Text.RegularExpressions.Regex("^[A-Z]+$");
                return eng.IsMatch(str);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 是否為數字
        /// </summary>
        /// <param name="pInput"></param>
        /// <returns></returns>
        public static bool IsNumber(string inputString)
        {
            try
            {
                System.Text.RegularExpressions.Regex objNotNumberPattern = new System.Text.RegularExpressions.Regex("[^0-9.-]");
                System.Text.RegularExpressions.Regex objTwoDotPattern = new System.Text.RegularExpressions.Regex("[0-9]*[.][0-9]*[.][0-9]*");
                System.Text.RegularExpressions.Regex objTwoMinusPattern = new System.Text.RegularExpressions.Regex("[0-9]*[-][0-9]*[-][0-9]*");
                string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
                string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
                System.Text.RegularExpressions.Regex objNumberPattern = new System.Text.RegularExpressions.Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

                return !objNotNumberPattern.IsMatch(inputString) &&
                    !objTwoDotPattern.IsMatch(inputString) &&
                    !objTwoMinusPattern.IsMatch(inputString) &&
                    objNumberPattern.IsMatch(inputString);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 字串轉成Integer
        /// </summary>
        /// <param name="inputString">輸入字串</param>
        /// <returns></returns>
        public static int String2Int(string inputString)
        {
            try
            {
                int avalue = -99999;
                int.TryParse(inputString, out avalue);
                return avalue;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 字串轉成System.DateTime
        /// </summary>
        /// <param name="inputString">輸入字串</param>
        /// <param name="format">指定要解析的字串格式，例如：yyyyMMddHHmmss</param>
        /// <returns></returns>
        public static System.DateTime String2DateTime(string inputString,string format)
        {
            System.DateTime dateTime = System.DateTime.MinValue;

            try
            {                
                System.DateTime.TryParseExact(inputString, format, null, System.Globalization.DateTimeStyles.None, out dateTime);
                return dateTime;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從字串中取得Key-Value組合的集合內容
        /// </summary>
        /// <param name="source">來源字串</param>
        /// <param name="splitSetting">分隔各項組合的字元，例如:key1=value1;key2=value2;key3=value3，則splitSetting參數為";"</param>
        /// <param name="splitKeyValue">分隔Key和Value的字元，例如:key1=value1;key2=value2;key3=value3，splitKeyValue參數為"="</param>
        /// <param name="error">解析時發生的錯誤描述</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetKeyValueDictionary(string source, string splitSetting, string splitKeyValue, out string error)
        {
            Dictionary<string, string> oDictionary = new Dictionary<string, string>();
            error = "";

            try
            {
                string[] sSplitSettingArray = Split(source, splitSetting);
                if (sSplitSettingArray != null && sSplitSettingArray.Length > 0)
                {
                    foreach(string sSetting in sSplitSettingArray)
                    {
                        string[] splitKeyValueArray = Split(sSetting, splitKeyValue);
                        if (splitKeyValueArray != null && splitKeyValueArray.Length == 2)
                        {
                            oDictionary.Add(splitKeyValueArray[0], splitKeyValueArray[1]);
                        }
                        else
                        {
                            error = $"設定{sSetting}中沒有[{splitKeyValue}]字元";
                        }
                    }
                }
                else
                {
                    error = $"字串{source}中沒有[{splitSetting}]字元";
                }

                return oDictionary;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從指定字串中擷取第一次出現的頭、尾之間的內容
        /// </summary>
        /// <param name="input">指定字串</param>
        /// <param name="startTag">頭</param>
        /// <param name="endTag">尾</param>
        /// <returns></returns>
        public static string ExtractContent(string input, string startTag, string endTag)
        {
            int startIndex = input.IndexOf(startTag);
            int endIndex = input.IndexOf(endTag, startIndex + startTag.Length);

            if (startIndex != -1 && endIndex != -1)
            {
                int contentStartIndex = startIndex + startTag.Length;
                int contentLength = endIndex - contentStartIndex;

                if (contentLength > 0)
                {
                    string extractedContent = input.Substring(contentStartIndex, contentLength);
                    return extractedContent;
                }
            }

            return null;
        }
    }
}
