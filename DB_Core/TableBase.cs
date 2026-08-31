using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

using Dapper;

namespace ASI.Wanda.DB
{
    /// <summary>
    /// 資料表存取的共用基底類別，DMD_DB / CMFT_DB / DCU_DB 三個專案共用同一份實作。
    /// 各專案只需提供一個 <see cref="ITableOptions"/> 實作（連線字串、稽核使用者、是否啟用資料庫），
    /// 再宣告 <c>abstract class Table&lt;T&gt; : TableBase&lt;T, XxxTableOptions&gt;</c> 即可。
    ///
    /// SQL 一律以參數化方式送出（Dapper），並依 Model 的屬性產生欄位清單。
    ///
    /// 識別字（資料表名、欄位名）刻意不加雙引號，沿用 PostgreSQL 自動折成小寫的行為，
    /// 因為 Model 之中存在 OCS_Data、countdown_display_Interval 這類與實際欄位大小寫不一致的名稱。
    /// </summary>
    /// <typeparam name="TModel">對應資料表的 Model 型別</typeparam>
    /// <typeparam name="TOptions">提供連線與稽核設定的型別</typeparam>
    abstract public class TableBase<TModel, TOptions> where TOptions : ITableOptions, new()
    {
        #region Public

        public enum eSortWay
        {
            /// <summary>
            /// 由小到大排序
            /// </summary>
            Asc,

            /// <summary>
            /// 由大到小排序
            /// </summary>
            Desc
        }

        /// <summary>
        /// 取回整張表。Model 若沒有 ins_time 欄位則不加排序。
        /// </summary>
        static public List<TModel> SelectAll(eSortWay inserTimeSortWay = eSortWay.Asc)
        {
            string commandString = string.Format("select * from {0}{1};", TableName, OrderByClause(inserTimeSortWay));

            return Query(commandString);
        }

        /// <summary>
        /// 針對這個資料庫開啟一個交易範圍，範圍內的所有 Table 操作都會沿用同一筆交易。
        /// </summary>
        static public DbTransactionScope BeginTransaction()
        {
            return DbTransactionScope.Begin(Options);
        }

        #endregion

        #region Protected - 執行

        /// <summary>
        /// 執行查詢並將結果對映為 Model 清單。
        /// </summary>
        /// <param name="commandString">SQL 命令，值請一律用 @參數名 表示。</param>
        /// <param name="param">參數物件，可傳匿名型別或 DynamicParameters；沒有參數時傳 null。</param>
        static protected List<TModel> Query(string commandString, object param = null)
        {
            List<TModel> modelList = new List<TModel>();

            if (!Options.IsUseDatabase)
                return modelList;

            try
            {
                DbTransactionScope scope = DbTransactionScope.Current(typeof(TOptions));
                if (scope != null)
                {
                    modelList = scope.Connection.Query<TModel>(commandString, param, scope.Transaction).AsList();
                }
                else
                {
                    Options.BeforeConnect();

                    using (IDbConnection connection = Options.CreateConnection())
                    {
                        connection.Open();
                        modelList = connection.Query<TModel>(commandString, param).AsList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(BuildErrorMessage(commandString, param), ex);
            }

            return modelList;
        }

        /// <summary>
        /// 執行 insert / update / delete，回傳受影響的資料列數。
        /// </summary>
        /// <param name="commandString">SQL 命令，值請一律用 @參數名 表示。</param>
        /// <param name="param">參數物件，可傳匿名型別或 DynamicParameters；沒有參數時傳 null。</param>
        static protected int NonQuery(string commandString, object param = null)
        {
            int impactRow = 0;

            if (!Options.IsUseDatabase)
                return impactRow;

            try
            {
                DbTransactionScope scope = DbTransactionScope.Current(typeof(TOptions));
                if (scope != null)
                {
                    impactRow = scope.Connection.Execute(commandString, param, scope.Transaction);
                }
                else
                {
                    Options.BeforeConnect();

                    using (IDbConnection connection = Options.CreateConnection())
                    {
                        connection.Open();
                        impactRow = connection.Execute(commandString, param);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(BuildErrorMessage(commandString, param), ex);
            }

            return impactRow;
        }

        #endregion

        #region Protected - 以主鍵操作

        static protected TModel Select(params object[] pks)
        {
            DynamicParameters parameters = new DynamicParameters();
            string whereString = BuildKeyWhere(pks, parameters);

            string commandString = string.Format("select * from {0}{1};", TableName, whereString);

            return Query(commandString, parameters).FirstOrDefault();
        }

        static protected int Delete(params object[] pks)
        {
            DynamicParameters parameters = new DynamicParameters();
            string whereString = BuildKeyWhere(pks, parameters);

            string commandString = string.Format("delete from {0}{1};", TableName, whereString);

            return NonQuery(commandString, parameters);
        }

        /// <summary>
        /// 新增一筆資料。
        /// paramObjects 依序對應 Model 的「非稽核」屬性，欄位清單由 Model 產生，
        /// ins_user / ins_time / upd_user / upd_time 則依 Model 有宣告哪幾個自動補上。
        /// </summary>
        static protected int Insert(params object[] paramObjects)
        {
            if (paramObjects == null || paramObjects.Length == 0)
                throw new ArgumentException(
                    string.Format("{0}.Insert 至少需要一個欄位值。", typeof(TModel).Name), "paramObjects");

            if (paramObjects.Length > ValueProperties.Length)
                throw new ArgumentException(
                    string.Format("{0}.Insert 傳入 {1} 個值，超過 Model 可寫入的欄位數 {2}。",
                        typeof(TModel).Name, paramObjects.Length, ValueProperties.Length), "paramObjects");

            DynamicParameters parameters = new DynamicParameters();
            List<string> columns = new List<string>();
            List<string> values = new List<string>();

            for (int index = 0; index < paramObjects.Length; index++)
            {
                columns.Add(ValueProperties[index].Name);

                if (paramObjects[index] == null)
                {
                    // null 以 SQL 常值送出，讓 PostgreSQL 自行推斷欄位型別
                    values.Add("null");
                    continue;
                }

                string parameterName = "p" + index;
                values.Add("@" + parameterName);
                parameters.Add(parameterName, paramObjects[index]);
            }

            bool useAuditUser = false;
            foreach (PropertyInfo audit in AuditProperties)
            {
                columns.Add(audit.Name);

                if (IsUserColumn(audit.Name))
                {
                    values.Add("@" + AuditUserParameter);
                    useAuditUser = true;
                }
                else
                {
                    values.Add(SqlTimeExpression(audit));
                }
            }

            if (useAuditUser)
                parameters.Add(AuditUserParameter, Options.CurrentUserID);

            string commandString = string.Format(
                "insert into {0} ({1}){2}values ({3});",
                TableName,
                string.Join(", ", columns.ToArray()),
                Environment.NewLine,
                string.Join(", ", values.ToArray()));

            return NonQuery(commandString, parameters);
        }

        /// <summary>
        /// 新增一筆資料，欄位與值都直接取自 Model（稽核欄位一樣自動補上）。
        /// 呼叫端不必逐一列出欄位，Model 日後增減欄位也不會對錯位置。
        /// </summary>
        static protected int Insert(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            return Insert(ReadValueProperties(model));
        }

        /// <summary>
        /// 依主鍵更新資料。
        /// paramObjects 依序對應 Model 的「非稽核」屬性，前段必須是主鍵（[Key]）的值。
        /// Model 若有宣告 upd_user / upd_time 會自動更新。
        /// </summary>
        static protected int Update(params object[] paramObjects)
        {
            if (paramObjects == null || paramObjects.Length == 0)
                throw new ArgumentException(
                    string.Format("{0}.Update 至少需要一個欄位值。", typeof(TModel).Name), "paramObjects");

            if (paramObjects.Length > ValueProperties.Length)
                throw new ArgumentException(
                    string.Format("{0}.Update 傳入 {1} 個值，超過 Model 可寫入的欄位數 {2}。",
                        typeof(TModel).Name, paramObjects.Length, ValueProperties.Length), "paramObjects");

            DynamicParameters parameters = new DynamicParameters();
            List<string> setClauses = new List<string>();

            for (int index = 0; index < paramObjects.Length; index++)
            {
                string columnName = ValueProperties[index].Name;

                if (paramObjects[index] == null)
                {
                    setClauses.Add(string.Format("{0} = null", columnName));
                    continue;
                }

                string parameterName = "p" + index;
                setClauses.Add(string.Format("{0} = @{1}", columnName, parameterName));
                parameters.Add(parameterName, paramObjects[index]);
            }

            AppendUpdateAudit(setClauses, parameters);

            // 主鍵條件同樣取自 paramObjects 的前段
            string whereString = BuildKeyWhere(paramObjects, parameters);

            string commandString = string.Format(
                "update {0} set{1}  {2}{3};",
                TableName,
                Environment.NewLine,
                string.Join(Environment.NewLine + ", ", setClauses.ToArray()),
                whereString);

            return NonQuery(commandString, parameters);
        }

        #endregion

        #region Protected - 以自訂條件操作

        static protected List<TModel> SelectWhere(string where, eSortWay inserTimeSortWay = eSortWay.Asc)
        {
            return SelectWhere(where, null, inserTimeSortWay);
        }

        /// <summary>
        /// 以自訂 where 子句查詢。
        /// where 內的值請寫成 @參數名 並由 param 帶入，例如：
        /// SelectWhere("where station_id = @station_id", new { station_id = stationID })
        /// </summary>
        static protected List<TModel> SelectWhere(string where, object param, eSortWay inserTimeSortWay = eSortWay.Asc)
        {
            string commandString = string.Format(
                "select * from {0}{1}{2}{3};",
                TableName,
                Environment.NewLine,
                where,
                OrderByClause(inserTimeSortWay));

            return Query(commandString, param);
        }

        /// <summary>
        /// 以自訂 where 子句更新指定欄位。
        /// where 內的值請寫成 @參數名 並由 whereParam 帶入。
        /// </summary>
        static protected int UpdateWhere(Dictionary<string, object> columnVals, string where)
        {
            return UpdateWhere(columnVals, where, null);
        }

        /// <summary>
        /// 以自訂 where 子句更新整筆 Model（稽核欄位除外，那幾個會自動處理）。
        /// 欄位清單由 Model 產生，呼叫端不必手動維護，例如：
        /// UpdateWhere(data, "where platform_id = @platform_id", new { platform_id })
        /// </summary>
        static protected int UpdateWhere(TModel model, string where, object whereParam = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Dictionary<string, object> columnVals = new Dictionary<string, object>();
            object[] values = ReadValueProperties(model);

            for (int index = 0; index < ValueProperties.Length; index++)
                columnVals[ValueProperties[index].Name] = values[index];

            return UpdateWhere(columnVals, where, whereParam);
        }

        /// <summary>
        /// 以自訂 where 子句更新指定欄位，例如：
        /// UpdateWhere(columnVals, "where alarm_id = @alarm_id", new { alarm_id = alarmID })
        /// </summary>
        static protected int UpdateWhere(Dictionary<string, object> columnVals, string where, object whereParam)
        {
            if (columnVals == null || columnVals.Count == 0)
                throw new ArgumentException(
                    string.Format("{0}.UpdateWhere 至少需要一個欄位值。", typeof(TModel).Name), "columnVals");

            DynamicParameters parameters = new DynamicParameters();
            List<string> setClauses = new List<string>();
            int index = 0;

            foreach (KeyValuePair<string, object> columnVal in columnVals)
            {
                if (!PropertyNames.Contains(columnVal.Key))
                    throw new ArgumentException(
                        string.Format("{0} 沒有名為 {1} 的屬性。", typeof(TModel).Name, columnVal.Key), "columnVals");

                if (columnVal.Value == null)
                {
                    setClauses.Add(string.Format("{0} = null", columnVal.Key));
                }
                else
                {
                    string parameterName = "c" + index;
                    setClauses.Add(string.Format("{0} = @{1}", columnVal.Key, parameterName));
                    parameters.Add(parameterName, columnVal.Value);
                }

                index++;
            }

            AppendUpdateAudit(setClauses, parameters);

            if (whereParam != null)
                parameters.AddDynamicParams(whereParam);

            string commandString = string.Format(
                "update {0} set{1}  {2}{3}{4};",
                TableName,
                Environment.NewLine,
                string.Join(Environment.NewLine + ", ", setClauses.ToArray()),
                Environment.NewLine,
                where);

            return NonQuery(commandString, parameters);
        }

        /// <summary>
        /// 以自訂 where 子句刪除。
        /// where 內的值請寫成 @參數名 並由 param 帶入。
        /// </summary>
        static protected int DeleteWhere(string where, object param = null)
        {
            string commandString = string.Format(
                "delete from {0}{1}{2};",
                TableName,
                Environment.NewLine,
                where);

            return NonQuery(commandString, param);
        }

        #endregion

        #region Private - Model 中繼資料

        /// <summary>
        /// 稽核欄位使用的參數名稱，刻意與 p0 / k0 / c0 系列區隔避免撞名。
        /// </summary>
        private const string AuditUserParameter = "audit_user";

        static private readonly string[] AuditColumnNames =
            new string[] { "ins_user", "ins_time", "upd_user", "upd_time" };

        static private readonly TOptions Options = new TOptions();

        /// <summary>Model 的全部屬性，順序即宣告順序。</summary>
        static private readonly PropertyInfo[] AllProperties = typeof(TModel).GetProperties();

        /// <summary>可由呼叫端指定值的欄位（扣掉稽核欄位）。</summary>
        static private readonly PropertyInfo[] ValueProperties =
            AllProperties.Where(p => !IsAuditColumn(p.Name)).ToArray();

        /// <summary>Model 實際有宣告的稽核欄位，順序即宣告順序。</summary>
        static private readonly PropertyInfo[] AuditProperties =
            AllProperties.Where(p => IsAuditColumn(p.Name)).ToArray();

        static private readonly HashSet<string> PropertyNames =
            new HashSet<string>(AllProperties.Select(p => p.Name), StringComparer.Ordinal);

        /// <summary>Model 是否有 ins_time 欄位，決定 select 能不能加 order by。</summary>
        static private readonly bool HasInsertTimeColumn =
            AllProperties.Any(p => string.Equals(p.Name, "ins_time", StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// 資料表名稱。不加雙引號，交由 PostgreSQL 自動折成小寫。
        /// </summary>
        static protected string TableName
        {
            get { return "dbo." + typeof(TModel).Name; }
        }

        static private bool IsAuditColumn(string name)
        {
            return AuditColumnNames.Any(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase));
        }

        static private bool IsUserColumn(string name)
        {
            return name.EndsWith("_user", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 依 ValueProperties 的順序，把 Model 的非稽核欄位值讀成陣列。
        /// </summary>
        static private object[] ReadValueProperties(TModel model)
        {
            object[] values = new object[ValueProperties.Length];

            for (int index = 0; index < ValueProperties.Length; index++)
                values[index] = ValueProperties[index].GetValue(model, null);

            return values;
        }

        static private bool IsKey(PropertyInfo property)
        {
            return Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) != null;
        }

        #endregion

        #region Private - SQL 組裝

        static private string OrderByClause(eSortWay sortWay)
        {
            if (!HasInsertTimeColumn)
                return string.Empty;

            return string.Format("{0}order by ins_time {1}",
                Environment.NewLine,
                sortWay == eSortWay.Desc ? "desc" : "asc");
        }

        /// <summary>
        /// 取得目前時間的 SQL 運算式。時間欄位若是字串型別就轉成文字，避免型別不符。
        /// </summary>
        static private string SqlTimeExpression(PropertyInfo property)
        {
            string expression = Options.CurrentSqlTime;

            return property.PropertyType == typeof(string)
                 ? string.Format("cast({0} as text)", expression)
                 : expression;
        }

        /// <summary>
        /// 依 Model 有沒有宣告 upd_user / upd_time，補上更新時的稽核欄位。
        /// </summary>
        static private void AppendUpdateAudit(List<string> setClauses, DynamicParameters parameters)
        {
            bool useAuditUser = false;

            foreach (PropertyInfo audit in AuditProperties)
            {
                if (!audit.Name.StartsWith("upd_", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (IsUserColumn(audit.Name))
                {
                    setClauses.Add(string.Format("{0} = @{1}", audit.Name, AuditUserParameter));
                    useAuditUser = true;
                }
                else
                {
                    setClauses.Add(string.Format("{0} = {1}", audit.Name, SqlTimeExpression(audit)));
                }
            }

            if (useAuditUser)
                parameters.Add(AuditUserParameter, Options.CurrentUserID);
        }

        /// <summary>
        /// 依 Model 上標記 [Key] 的屬性組出參數化的 where 子句。
        /// pks 以 [Key] 屬性的宣告順序對應，遇到 null 即停止往下取主鍵。
        /// </summary>
        static private string BuildKeyWhere(object[] pks, DynamicParameters parameters)
        {
            if (pks == null)
                throw new ArgumentNullException("pks");

            List<string> conditions = new List<string>();
            int index = 0;

            foreach (PropertyInfo property in AllProperties)
            {
                if (!IsKey(property))
                    continue;

                if (index >= pks.Length || pks[index] == null)
                    break;

                string parameterName = "k" + index;
                conditions.Add(string.Format("{0} = @{1}", property.Name, parameterName));
                parameters.Add(parameterName, pks[index]);
                index++;
            }

            if (conditions.Count == 0)
                throw new InvalidOperationException(
                    string.Format("{0} 未取得任何主鍵條件，請確認 Model 已標記 [Key] 且主鍵值不為 null。",
                        typeof(TModel).Name));

            return Environment.NewLine
                 + "where " + string.Join(Environment.NewLine + "  and ", conditions.ToArray());
        }

        /// <summary>
        /// 組出包含 SQL 與參數內容的錯誤訊息，維持「Sql命令:」開頭以相容既有的 log 檢索。
        /// </summary>
        static private string BuildErrorMessage(string commandString, object param)
        {
            StringBuilder message = new StringBuilder();
            message.Append("Sql命令:").Append(Environment.NewLine).Append(commandString);

            try
            {
                DynamicParameters dynamicParameters = param as DynamicParameters;
                if (dynamicParameters != null)
                {
                    message.Append(Environment.NewLine).Append("參數:");
                    foreach (string parameterName in dynamicParameters.ParameterNames)
                    {
                        object value = dynamicParameters.Get<object>(parameterName);
                        message.Append(Environment.NewLine)
                               .Append(string.Format("  @{0} = {1}",
                                   parameterName, value == null ? "null" : value.ToString()));
                    }
                }
            }
            catch
            {
                // 組錯誤訊息本身失敗時不可蓋掉原始例外
            }

            return message.ToString();
        }

        #endregion
    }
}
