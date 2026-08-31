using System;
using System.Collections.Generic;
using System.Data;

namespace ASI.Lib.DB
{
    /// <summary>
    /// 資料庫交易範圍。
    ///
    /// 開啟之後，同一個執行緒上針對同一個資料庫的所有 <see cref="TableBase{TModel, TOptions}"/> 操作
    /// 都會自動沿用這個範圍的連線與交易，因此既有的 Table wrapper 呼叫不需要修改：
    ///
    /// <code>
    /// using (var scope = ASI.Wanda.DMD.DB.Manager.BeginTransaction())
    /// {
    ///     dmdGroup.InsertGroup(groupID, name, description);
    ///     dmdGroupTarget.InsertGroupTarget(groupID, stationID, areaID, deviceID);
    ///     scope.Complete();   // 沒有呼叫 Complete 就離開 using 會自動 Rollback
    /// }
    /// </code>
    ///
    /// 注意事項：
    ///  - 範圍是以「執行緒」為界（ThreadStatic）。跨執行緒或 await 之後不會延續，請在同一個執行緒內完成。
    ///  - 同一個資料庫的交易範圍不可巢狀，巢狀開啟會直接拋出例外。
    ///  - 不同資料庫（DMD / CMFT / DCU）各自獨立，可以同時開啟，但彼此不是同一筆交易。
    /// </summary>
    public sealed class DbTransactionScope : IDisposable
    {
        [ThreadStatic]
        static private Dictionary<Type, DbTransactionScope> _Current;

        static private Dictionary<Type, DbTransactionScope> Store
        {
            get
            {
                if (_Current == null)
                    _Current = new Dictionary<Type, DbTransactionScope>();
                return _Current;
            }
        }

        private readonly Type _OptionsType;
        private readonly IDbConnection _Connection;
        private readonly IDbTransaction _Transaction;
        private bool _Completed;
        private bool _Disposed;

        private DbTransactionScope(Type optionsType, IDbConnection connection, IDbTransaction transaction)
        {
            _OptionsType = optionsType;
            _Connection = connection;
            _Transaction = transaction;
        }

        /// <summary>
        /// 針對指定資料庫開啟一個交易範圍。
        /// </summary>
        static public DbTransactionScope Begin(ITableOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            Type optionsType = options.GetType();

            if (Store.ContainsKey(optionsType))
                throw new InvalidOperationException(
                    string.Format("{0} 的交易範圍已經開啟，不支援巢狀交易。", optionsType.Name));

            options.BeforeConnect();

            IDbConnection connection = options.CreateConnection();
            IDbTransaction transaction;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
            }
            catch
            {
                connection.Dispose();
                throw;
            }

            DbTransactionScope scope = new DbTransactionScope(optionsType, connection, transaction);
            Store[optionsType] = scope;
            return scope;
        }

        /// <summary>
        /// 取得目前執行緒上，指定資料庫的交易範圍；沒有開啟時回傳 null。
        /// </summary>
        static internal DbTransactionScope Current(Type optionsType)
        {
            DbTransactionScope scope;
            return Store.TryGetValue(optionsType, out scope) ? scope : null;
        }

        internal IDbConnection Connection
        {
            get { return _Connection; }
        }

        internal IDbTransaction Transaction
        {
            get { return _Transaction; }
        }

        /// <summary>
        /// 認可這個範圍內的所有異動。沒有呼叫就離開 using 會自動 Rollback。
        /// </summary>
        public void Complete()
        {
            if (_Disposed)
                throw new ObjectDisposedException("DbTransactionScope");

            if (_Completed)
                throw new InvalidOperationException("這個交易範圍已經 Complete 過了。");

            _Transaction.Commit();
            _Completed = true;
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            Store.Remove(_OptionsType);

            try
            {
                if (!_Completed)
                    _Transaction.Rollback();
            }
            catch
            {
                // Rollback 失敗（例如連線已中斷）不應蓋掉原本的例外
            }
            finally
            {
                _Transaction.Dispose();
                _Connection.Dispose();
            }
        }
    }
}
