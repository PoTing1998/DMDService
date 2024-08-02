using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.FileProc
{
    public class Reader
    {     

        public static string ReadAll(string filePath)
        {            
            string sReturn = "";
            
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    sReturn = System.IO.File.ReadAllText(filePath);
                }

                System.IO.FileInfo fileInfo = new FileInfo(filePath);

                return sReturn;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

    public class FileSystemWatcher
    {
        private System.IO.FileSystemWatcher mFileSystemWatcher = null;

        private long mPreviousPosition = 0;

        public delegate void FileNewContentEventHandler(string newContent);
        /// <summary>
        /// 檔案新增內容的事件
        /// </summary>
        public event FileNewContentEventHandler FileNewContentEvent;


        /// <summary>
        /// 要監控的資料夾路徑
        /// </summary>
        /// <param name="folderPath"></param>
        public FileSystemWatcher(string folderPath)
        {
            mFileSystemWatcher = new System.IO.FileSystemWatcher();

            //設定所要監控的資料夾
            mFileSystemWatcher.Path = folderPath;

            //設定所要監控的變更類型
            mFileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            //設定所要監控的檔案
            mFileSystemWatcher.Filter = "*.TXT";

            //設定是否監控子資料夾
            mFileSystemWatcher.IncludeSubdirectories = true;

            //設定是否啟動元件，此部分必須要設定為 true，不然事件是不會被觸發的
            mFileSystemWatcher.EnableRaisingEvents = true;

            //設定觸發事件
            mFileSystemWatcher.Created += new FileSystemEventHandler(mFileSystemWatcher_Created);
            mFileSystemWatcher.Changed += new FileSystemEventHandler(mFileSystemWatcher_Changed);
            mFileSystemWatcher.Renamed += new RenamedEventHandler(mFileSystemWatcher_Renamed);
            mFileSystemWatcher.Deleted += new FileSystemEventHandler(mFileSystemWatcher_Deleted);
        }

        /// <summary>
        /// 當所監控的資料夾有建立文字檔時觸發
        /// </summary>
        private void mFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(e.FullPath.ToString());

            sb.AppendLine("新建檔案於：" + dirInfo.FullName.Replace(dirInfo.Name, ""));
            sb.AppendLine("新建檔案名稱：" + dirInfo.Name);
            sb.AppendLine("建立時間：" + dirInfo.CreationTime.ToString());
            sb.AppendLine("目錄下共有：" + dirInfo.Parent.GetFiles().Count() + " 檔案");
            sb.AppendLine("目錄下共有：" + dirInfo.Parent.GetDirectories().Count() + " 資料夾");            
        }

        /// <summary>
        /// 當所監控的資料夾有文字檔檔案內容有異動時觸發
        /// </summary>
        private void mFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(e.FullPath.ToString());

            sb.AppendLine("被異動的檔名為：" + e.Name);
            sb.AppendLine("檔案所在位址為：" + e.FullPath.Replace(e.Name, ""));
            sb.AppendLine("異動內容時間為：" + dirInfo.LastWriteTime.ToString());

            string newContent = "";

            //取得本次新增的內容
            using (StreamReader reader = new StreamReader(e.FullPath))
            {
                reader.BaseStream.Seek(mPreviousPosition, SeekOrigin.Begin);

                // 讀取本次新增的內容
                newContent = reader.ReadToEnd();
            }

            FileNewContentEvent?.Invoke(newContent);
        }

        /// <summary>
        /// 當所監控的資料夾有文字檔檔案重新命名時觸發
        /// </summary>
        private void mFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            System.IO.FileInfo fi = new FileInfo(e.FullPath.ToString());

            sb.AppendLine("檔名更新前：" + e.OldName.ToString());
            sb.AppendLine("檔名更新後：" + e.Name.ToString());
            sb.AppendLine("檔名更新前路徑：" + e.OldFullPath.ToString());
            sb.AppendLine("檔名更新後路徑：" + e.FullPath.ToString());
            sb.AppendLine("建立時間：" + fi.LastAccessTime.ToString());
        }

        /// <summary>
        /// 當所監控的資料夾有文字檔檔案有被刪除時觸發
        /// </summary>
        private void mFileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("被刪除的檔名為：" + e.Name);
            sb.AppendLine("檔案所在位址為：" + e.FullPath.Replace(e.Name, ""));
            sb.AppendLine("刪除時間：" + DateTime.Now.ToString());
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"檔案 {e.FullPath} 有新內容新增。");

            // 從上次讀取位置繼續讀取檔案內容
            string previousContent = File.ReadAllText(e.FullPath);
            long previousPosition = previousContent.Length;

            using (StreamReader reader = new StreamReader(e.FullPath))
            {
                reader.BaseStream.Seek(previousPosition, SeekOrigin.Begin);

                // 讀取新增的內容
                string newContent = reader.ReadToEnd();

                Console.WriteLine($"新增內容：{newContent}");
            }
        }
    }

}
