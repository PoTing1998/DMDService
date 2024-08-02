using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Text.Parsing
{
    public class Json
    {
        /// <summary>
        /// 將物件序列化
        /// </summary>
        /// <param name="obj">物件</param>
        /// <returns>序列化字串</returns>
        public static string SerializeObject(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 將序列化字串依指定的物件型別反序列化成物件
        /// </summary>
        /// <param name="value">序列化字串</param>
        /// <param name="type">物件的型別</param>
        /// <returns>物件</returns>
        public static object DeserializeObject(string value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// 取得指定屬性的值
        /// </summary>
        /// <param name="jsonData">Json序列化字串</param>
        /// <param name="propertyName">屬性名稱(key)</param>
        /// <returns></returns>
        public static string GetValue(string jsonData,string propertyName)
        {
            string sReturn = "";
            Newtonsoft.Json.Linq.JToken oJToken = null;

            try
            {
                if (jsonData != null)
                {                    
                    Newtonsoft.Json.Linq.JObject.Parse(jsonData).TryGetValue(propertyName, out oJToken);
                }

                if (oJToken != null)
                {
                    sReturn = oJToken.ToString();
                }
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
             
            return sReturn;
        }
    }
}
