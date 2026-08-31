using System.Data;

namespace ASI.Wanda.DB
{
    /// <summary>
    /// 由各個資料庫專案（DMD_DB / CMFT_DB / DCU_DB）提供給 <see cref="TableBase{TModel, TOptions}"/> 的連線與稽核設定。
    /// 實作類別必須有公開的無參數建構式。
    /// </summary>
    public interface ITableOptions
    {
        /// <summary>
        /// 建立一條尚未開啟的連線。實作端負責帶入該資料庫的連線字串。
        /// </summary>
        IDbConnection CreateConnection();

        /// <summary>
        /// 是否實際存取資料庫。為 false 時查詢回傳空集合、異動回傳 0。
        /// </summary>
        bool IsUseDatabase { get; }

        /// <summary>
        /// 寫入 ins_user / upd_user 的使用者代碼。
        /// </summary>
        string CurrentUserID { get; }

        /// <summary>
        /// 取得目前時間的 SQL 運算式（例如 clock_timestamp()）。
        /// 這段字串會直接併入 SQL，不可放入任何來自外部輸入的內容。
        /// </summary>
        string CurrentSqlTime { get; }

        /// <summary>
        /// 開啟連線前的前置檢查，例如先 Ping 一次資料庫主機。不需要時留空即可。
        /// </summary>
        void BeforeConnect();
    }
}
