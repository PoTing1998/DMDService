using System.Data;

using ASI.Wanda.DB;

using Npgsql;

namespace ASI.Wanda.DMD.DB.Tables
{
    /// <summary>
    /// DMD 資料庫的連線與稽核設定，供 <see cref="ASI.Wanda.DB.TableBase{TModel, TOptions}"/> 使用。
    /// </summary>
    public sealed class DmdTableOptions : ITableOptions
    {
        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(ASI.Wanda.DMD.DB.Manager.ConnectionString);
        }

        public bool IsUseDatabase
        {
            get { return ASI.Wanda.DMD.DB.Manager.IsUseDatabase; }
        }

        public string CurrentUserID
        {
            get { return ASI.Wanda.DMD.DB.Manager.CurrentUserID; }
        }

        public string CurrentSqlTime
        {
            get { return ASI.Wanda.DMD.DB.Manager.CurrentSqlTime; }
        }

        public void BeforeConnect()
        {
        }
    }

    /// <summary>
    /// DMD 資料表存取基底。實作在 DB_Core 的 <see cref="ASI.Wanda.DB.TableBase{TModel, TOptions}"/>，
    /// 三個資料庫專案共用同一份。
    /// </summary>
    abstract public class Table<T> : ASI.Wanda.DB.TableBase<T, DmdTableOptions>
    {
    }
}
