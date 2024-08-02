using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Web
{
    public class CallAPI
    {
        public static string Post(string url , string data)
        {
            //string url = "https://api.baidu.com/json/sms/v3/AccountService/getAccountInfo";
            //string data = "{\n\"header\": {\n\"token\": \"30xxx6aaxxx93ac8cxx8668xx39xxxx\",\n\"username\": \"jdads\",\n\"password\": \"liuqiangdong2010\",\n\"action\": \"\"\n},\n\"body\": {}\n}";
            string sReturn = "";

            HttpWebRequest oHttpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            oHttpWebRequest.Method = "POST";
            oHttpWebRequest.ContentType = "application/json";            

            byte[] bData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            oHttpWebRequest.ContentLength = bData.Length;

            using (Stream postStream = oHttpWebRequest.GetRequestStream())
            {
                postStream.Write(bData, 0, bData.Length);
            }

            using (HttpWebResponse response = oHttpWebRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                sReturn = reader.ReadToEnd();
            }

            return sReturn;
        }
    }
}
