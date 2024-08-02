using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Utility
{
    /// <summary>
    /// 遠端資料夾
    /// </summary>
    public class RemoteFolder
    {
        /// <summary>
        /// 連線並登入至遠端電腦
        /// </summary>
        /// <param name="pRemoteHost">遠端資料夾路徑(foramt:IP\資料夾路徑)</param>
        /// <param name="pUserName">使用者帳號</param>
        /// <param name="pPassword">登入密碼</param>
        /// <returns></returns>
        public static bool Connect(string pRemoteHost, string pUserName, string pPassword)
        {

            bool Flag = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            try
            {

                proc.Start();
                string command = @"net  use  \\" + pRemoteHost + "  " + pPassword + "  " + "  /user:" + pUserName + ">NUL";
                proc.StandardInput.WriteLine(command);
                command = "exit";
                proc.StandardInput.WriteLine(command);

                while (proc.HasExited == false)
                {
                    proc.WaitForExit(1000);
                }

                string errormsg = proc.StandardError.ReadToEnd();
                if (errormsg != "")
                    Flag = false;

                proc.StandardError.Close();
            }
            catch (Exception ex)
            {
                Flag = false;
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        /// <summary>
        /// 自遠端電腦離線
        /// </summary>
        /// <param name="pRemoteHost">遠端資料夾路徑(foramt:IP\資料夾路徑)</param>
        /// <returns></returns>
        public static bool DisConnect(string pRemoteHost)
        {
            bool Flag = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            try
            {

                proc.Start();
                string command = @"net  use  \\" + pRemoteHost + " /delete";
                proc.StandardInput.WriteLine(command);
                command = "exit";
                proc.StandardInput.WriteLine(command);

                while (proc.HasExited == false)
                {
                    proc.WaitForExit(1000);
                }

                string errormsg = proc.StandardError.ReadToEnd();
                if (errormsg != "")
                    Flag = false;

                proc.StandardError.Close();
            }
            catch (Exception ex)
            {
                Flag = false;
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        /// <summary>
        /// 複製檔案
        /// </summary>
        /// <param name="pFilePath">來源檔案路徑(foramt:IP\路徑)</param>
        /// <param name="pFileName">來源檔案名稱(為空值則會複製指定資料夾下所有檔案)</param>
        /// <param name="pUserName">登入使用者名稱</param>
        /// <param name="pPassword">登入密碼</param>
        /// <param name="pDestFilePath">目標檔案路徑</param>
        /// <param name="pDestFileName">目標檔案名稱(若複製整個資料夾無需指定)</param>
        /// <returns></returns>
        public static bool FileCopy(string pFilePath, string pFileName, string pUserName, string pPassword, string pDestFilePath, string pDestFileName)
        {
            try
            {
                bool bRtn = false;

                bRtn = Connect(pFilePath, pUserName, pPassword);

                if (pFileName.Length > 0)
                {
                    System.IO.File.Copy("\\\\" + pFilePath + "\\" + pFileName, pDestFilePath + "\\" + pDestFileName, true);
                }
                else
                {
                    System.IO.DirectoryInfo oSrcDirInfo = new System.IO.DirectoryInfo("\\\\" + pFilePath);
                    System.IO.FileInfo[] oFileInfoArray = oSrcDirInfo.GetFiles();

                    foreach (System.IO.FileInfo oFileInfo in oFileInfoArray)
                    {
                        if (oFileInfo.Name.ToLower() != "system.xml")
                        {
                            System.IO.File.Copy(oFileInfo.FullName, pDestFilePath + "\\" + oFileInfo.Name, true);
                        }
                    }
                }

                bRtn = DisConnect(pFilePath);
                return bRtn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
