using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.DB
{
    public class Tool
    {
        /// <summary>
        /// 備份Postgres資料庫，將輸出附檔名為.backup的備份檔案
        /// </summary>
        /// <param name="pgPath">pg_dump.exe的絕對路徑</param>
        /// <param name="dbHost">資料庫IP</param>
        /// <param name="dbPort">資料庫Port</param>
        /// <param name="pgUser">postgres資料庫使用者</param>
        /// <param name="pgPwd">使用者密碼</param>
        /// <param name="dbName">欲備份的資料庫名稱</param>
        /// <param name="outputPath">輸出路徑</param>
        /// <param name="outputFileName">輸出檔名(不含副檔名)</param>
        public static void PostgresBackup(string pgPath, string dbHost, string dbPort, string pgUser, string pgPwd, string dbName, string outputPath, string outputFileName)
        {
            System.Diagnostics.Process oProcess = new System.Diagnostics.Process();
            try
            {
                oProcess.StartInfo.FileName = "cmd.exe";
                oProcess.StartInfo.UseShellExecute = false;
                oProcess.StartInfo.RedirectStandardInput = true;
                oProcess.StartInfo.RedirectStandardOutput = true;
                oProcess.StartInfo.CreateNoWindow = true;
                oProcess.Start();

                //備份位置    
                if (!System.IO.Directory.Exists(outputPath))
                {
                    System.IO.Directory.CreateDirectory(outputPath);
                }

                //執行備份
                string sCommand = "";
                sCommand = $"SET PGPATH=\"{pgPath}\"";
                oProcess.StandardInput.WriteLine(sCommand);
                sCommand = $"SET PGPASSWORD={pgPwd}";
                oProcess.StandardInput.WriteLine(sCommand);
                sCommand = $"%PGPATH% -h 127.0.0.1 -p 5432 -U postgres -F c -b -v -f {outputPath}{outputFileName}.backup {dbName}";
                oProcess.StandardInput.WriteLine(sCommand);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                oProcess.StandardInput.WriteLine("exit");
                oProcess.Close();
            }
        }

    }
}
