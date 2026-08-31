using System;
using System.Data;

using ASI.Wanda.DB;

using Npgsql;

namespace ASI.Wanda.CMFT.DB.Tables
{
    /// <summary>
    /// CMFT 資料庫的連線與稽核設定，供 <see cref="ASI.Wanda.DB.TableBase{TModel, TOptions}"/> 使用。
    /// </summary>
    public sealed class CmftTableOptions : ITableOptions
    {
        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(ASI.Wanda.CMFT.DB.Manager.ConnectionString);
        }

        public bool IsUseDatabase
        {
            get { return ASI.Wanda.CMFT.DB.Manager.IsUseDatabase; }
        }

        public string CurrentUserID
        {
            get { return ASI.Wanda.CMFT.DB.Manager.CurrentUserID; }
        }

        public string CurrentSqlTime
        {
            get { return ASI.Wanda.CMFT.DB.Manager.CurrentSqlTime; }
        }

        /// <summary>
        /// 沿用舊版：連線前先 Ping 一次資料庫主機，失敗時觸發 Manager.ErrorHandle。
        /// </summary>
        public void BeforeConnect()
        {
            if (!ASI.Lib.Comm.Network.NetworkLib.Ping(ASI.Wanda.CMFT.DB.Manager.ConnectIP, 1000))
            {
                ASI.Wanda.CMFT.DB.Manager.ErrorHandle?.Invoke();
                throw new Exception(string.Format("與{0}網路連接失敗!", ASI.Wanda.CMFT.DB.Manager.ConnectIP));
            }
        }
    }

    /// <summary>
    /// CMFT 資料表存取基底。實作在 DB_Core 的 <see cref="ASI.Wanda.DB.TableBase{TModel, TOptions}"/>，
    /// 三個資料庫專案共用同一份。
    /// </summary>
    abstract public class Table<T> : ASI.Wanda.DB.TableBase<T, CmftTableOptions>
    {
    }
}
