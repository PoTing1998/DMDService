using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;

namespace ASI.Lib.Config
{
    public class ConfigApp
    {
        #region =====[Private]Variable=====

        private ConfigApp() { }

        private List<KeyValuePair<string, string>> mAppConfig = new List<KeyValuePair<string, string>>();

        private List<System.Xml.XmlNode> mConfigNodeList = new List<XmlNode>();

        private static ConfigApp mInstance = new ConfigApp();

        #endregion


        #region =====[Public]Const=====

        public const string HOST_NAME = "Host_Name";
        public const string SERVER_DB = "Server_DB";
        public const string LOCAL_DB = "Local_DB";
        public const string PROCESS = "Process";

        #endregion


        #region =====[Public]Property=====

        public List<KeyValuePair<string, string>> APPConfig
        {
            get { return mAppConfig; }
        }

        /// <summary>
        /// ConfigApp的唯一物件
        /// </summary>
        public static ConfigApp Instance
        {
            get
            {
                lock (mInstance.mAppConfig)
                {
                    if (mInstance.mAppConfig.Count <= 0)
                    {
                        mInstance = new ConfigApp();
                        mInstance.Initial();
                    }
                }

                return mInstance;
            }
        }

        public string HostName
        {
            get { return GetConfig(HOST_NAME); }
        }

        public string ServerDB
        {
            get { return GetConfig(SERVER_DB); }
        }

        public string LocalDB
        {
            get { return GetConfig(LOCAL_DB); }
        }

        #endregion


        #region =====[Private]Function=====

        private void Initial()
        {
            try
            {
                mAppConfig = GetConfigs("Config\\Config.xml");
                string sHostName = GetConfig(HOST_NAME);
                if (sHostName.Length <= 0)
                {
                    return;
                }

                List<KeyValuePair<string, string>> oKeyValuePairList = GetConfigs("Config\\" + sHostName + ".xml");
                foreach (KeyValuePair<string, string> oPair in oKeyValuePairList)
                {
                    mAppConfig.Add(oPair);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProjectConfig.Initial : " + ex.Message);
                return;
            }
        }

        private List<KeyValuePair<string, string>> GetConfigs(string pFile)
        {
            List<KeyValuePair<string, string>> rets = new List<KeyValuePair<string, string>>();
            try
            {
                StreamReader afile = new StreamReader(pFile);

                XmlDocument adoc = new XmlDocument();
                adoc.Load(afile);
                foreach (XmlNode anode in adoc.DocumentElement.ChildNodes)
                {
                    if (anode != null &&
                        anode.Attributes != null)
                    {
                        if (anode.Attributes["key"] != null &&
                            anode.Attributes["value"] != null)
                        {
                            rets.Add(new KeyValuePair<string, string>(
                                anode.Attributes.GetNamedItem("key").InnerText,
                                anode.Attributes.GetNamedItem("value").InnerText));
                        }

                        mConfigNodeList.Add(anode);
                    }
                }
                afile.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return rets;
        }

        private string GetConfig(string pKey)
        {
            foreach (KeyValuePair<string, string> oKeyValPair in mAppConfig)
            {
                if (oKeyValPair.Key == pKey)
                {
                    return oKeyValPair.Value;
                }
            }

            return "";
        }

        #endregion


        #region =====[Public]Function=====

        public string GetConfigSetting(string pKey)
        {
            return GetConfig(pKey);
        }

        public List<string> GetConfigSettings(string pKey)
        {
            List<string> rets = new List<string>();
            foreach (KeyValuePair<string, string> apair in mAppConfig)
            {
                if (apair.Key == pKey)
                    rets.Add(apair.Value);
            }
            return rets;
        }

        /// <summary>
        /// 取得config內指定key名稱的節點
        /// </summary>
        /// <param name="pKey">指定key名稱，若為null則為不指定</param>
        /// <returns></returns>
        public List<System.Xml.XmlNode> GetConfigNodes(string pKey)
        {
            if (pKey == null)
            {
                return this.mConfigNodeList;
            }

            List<System.Xml.XmlNode> oRtnList = new List<XmlNode>();
            foreach (System.Xml.XmlNode oNode in mConfigNodeList)
            {
                if (oNode.Attributes["key"] != null &&
                    oNode.Attributes["key"].Value == pKey)
                {
                    oRtnList.Add(oNode);
                }
            }

            return oRtnList;
        }

        public int GetCommSetting(string pKey, out string pChannel, out string pParameter)
        {
            pChannel = "";
            pParameter = "";

            string setting = GetConfigSetting(pKey);

            string[] sSplits = setting.Split(new char[] { ';' }, 2);

            if (sSplits.Length < 2)
                return -1;

            pChannel = sSplits[0];
            pParameter = sSplits[1];

            return 1;
        }

        /// <summary>
		/// 從connection string中擷取IP
		/// </summary>
		/// <param name="connStr">connection string，格式如：IP=10.107.26.99;Port=8000;Type=Client</param>
		/// <returns></returns>
		public string GetIPFromConnStr(string connStr)
        {
            string sReturn = "";

            if (connStr != null && connStr != "" && connStr.Contains(";") && connStr.Contains("IP"))
            {
                connStr = connStr.ToUpper().Trim();
                string[] strArray = ASI.Lib.Text.Parsing.String.Split(connStr, ";");
                if (strArray != null)
                {
                    foreach (string ss in strArray)
                    {
                        if (ss.Contains("IP") && ss.Contains("="))
                        {
                            string[] strIP = ASI.Lib.Text.Parsing.String.Split(ss, "=");
                            if (strIP != null && strIP.Length == 2)
                            {
                                sReturn = strIP[1];
                                break;
                            }
                        }
                    }
                }
            }

            return sReturn;
        }

        #endregion
    }

}
