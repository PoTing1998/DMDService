using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace ASI.Lib.Log
{
    public class TextFile
    {
        public static int WriteRecords(string pFile, string pRecs)
        {
            try
            {
                using (StreamWriter afile = new StreamWriter(File.Open(pFile, FileMode.Append), Encoding.GetEncoding(65001)))
                {
                    afile.WriteLine(pRecs);
                    afile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            return 1;
        }

        public static int OverwriteRecords(string pFile, string pRecs)
        {
            try
            {
                FileStream stream = File.Open(pFile, FileMode.OpenOrCreate, FileAccess.Write);
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                stream.Close();

                using (StreamWriter afile = new StreamWriter(pFile, true, Encoding.GetEncoding(65001)))
                {
                    afile.WriteLine(pRecs);
                    afile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            return 1;
        }
    }

    public class LogFile
    {
        private string mFilename = "";
        public LogFile(string pLogType, string pProcess)
        {
            string sDate = DateTime.Now.ToString("yyyyMMdd");
            mFilename = $".\\Log\\{sDate}\\{sDate}.{pLogType}-{pProcess}.log";
        }

        public void Log(string pMessage)
        {
            string ames = DateTime.Now.ToString("HH:mm:ss.fff") + "-" + pMessage;
            TextFile.WriteRecords(mFilename, ames);
        }

        public static void Log(string pLogType, string pProcess, string pMessage)
        {
            string sDate = DateTime.Now.ToString("yyyyMMdd");
            if (!System.IO.Directory.Exists($".\\Log\\{sDate}\\"))
            {
                System.IO.Directory.CreateDirectory($".\\Log\\{sDate}\\");
            }

            string sLog = DateTime.Now.ToString("HH:mm:ss.fff") + "-" + pMessage;
            string sFileName = $".\\Log\\{sDate}\\{sDate}.{pLogType}-{pProcess}.log";
            TextFile.WriteRecords(sFileName, sLog);
        }

        public static int DelLogDir(string pLogType, DateTime pTime, int pDays)
        {
            try
            {
                DirectoryInfo oLogDir = new DirectoryInfo(".\\Log");
                DirectoryInfo[] oAllDirs = oLogDir.GetDirectories();

                if (oAllDirs != null)
                {
                    DateTime oDirDate = new DateTime();
                    for (int ii = 0; ii < oAllDirs.Length; ii++)
                    {
                        if (oAllDirs[ii].Name.Length == 8)
                        {
                            string sDirDate = $"{oAllDirs[ii].Name.Substring(0, 4)}-{oAllDirs[ii].Name.Substring(4, 2)}-{oAllDirs[ii].Name.Substring(6, 2)}";

                            if (System.DateTime.TryParse(sDirDate, out oDirDate))
                            {
                                if (pTime.Subtract(oDirDate).TotalDays > pDays)
                                {
                                    oAllDirs[ii].Delete(true);
                                    LogFile.Log(pLogType, "ASI.Lib.Comm.MSMQ.LogLib", $"刪除{oAllDirs[ii].Name}資料夾");
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorLog.Log("ASI.Lib.Log.LogFile", ex);
            }

            return 1;
        }

        public static int DelLogFile(DateTime pTime, int pDays)
        {
            string date15 = new DateTime(pTime.Ticks).AddDays(-pDays).ToString("yyyyMMdd");
            DirectoryInfo adi = new DirectoryInfo(".\\Log");
            FileInfo[] fis = adi.GetFiles("*.log");

            for (int i = 0; i < fis.Length; ++i)
            {
                string[] str = fis[i].Name.Split((new char[] { '.' }));
                if (str.Length > 0)
                {
                    if (str[0].CompareTo(date15) <= 0)
                        File.Delete(fis[i].FullName);
                }
            }

            return 1;
        }

        /// <summary>
        /// 由console顯示指定內容
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Display(string pMessage)
        {
            string ames = DateTime.Now.ToString("HH:mm:ss.fff") + "-" + pMessage;
            Console.WriteLine(ames);
        }
    }

    public class ErrorLog
    {
        public static void Log(string pProcess, string pMessage)
        {
            LogFile.Log("Error", pProcess, pMessage);
        }

        public static void Log(string pProcess, Exception pException)
        {
            string errlog = "";
            while (pException != null)
            {
                errlog = errlog + pException.Message + " " + pException.StackTrace + "\r\n";
                pException = pException.InnerException;
            }
            Log(pProcess, errlog);
        }
    }

    public class DebugLog
    {
        public static void Log(string pProcess, string pMessage)
        {
            LogFile.Log("Debug", pProcess, $"◎◎◎{pMessage}\r\n");
        }
    }

    public class CSVLog
    {
        public static void Log(string pProcess, string pMessage)
        {
            LogFile.Log("CSV", pProcess, $",{pProcess.Replace(",", "；")},{pMessage.Replace(",", "；")}\r\n");
        }
    }

    public class SchdFile
    {
        public SchdFile(string pProcess)
        {
            ;
        }
        public static void Log(string pMessage)
        {
            string sSchdPath = ".\\SchdLog";
            string sDate = DateTime.Now.ToString("yyyyMMdd");
            string sTime = DateTime.Now.ToString("HHmmss");
            string sSchd = "Schedule";

            if (!System.IO.Directory.Exists($"{sSchdPath}\\{sDate}\\"))
            {
                System.IO.Directory.CreateDirectory($"{sSchdPath}\\{sDate}\\");
            }
            string sSchdFileName = $"{sSchdPath}\\{sDate}\\{sDate}.{sTime}-{sSchd}.log";
            string ames = pMessage;
            TextFile.WriteRecords(sSchdFileName, ames);
        }
    }
}
