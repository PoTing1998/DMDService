using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data.Common;

namespace ASI.Lib.DB
{
    public abstract class DBFactory
    {
        protected const string AcccessString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = ";
        protected const string SQLServerString = "Provider=SQLOLEDB.1;Data Source = ";
        protected const string OracleString = "Provider=MSDAORA.1;Data Source = ";
        protected const string SybaseString = "Provider=ASEOLEDB.1;Data Source= ";

        public abstract DataBase Create(string pDB);
    }

    public abstract class OleDB : DBFactory
    {
        public override DataBase Create(string pDB)
        {
            DataBase mDB = new DataBase(new OleDbConnection());
            mDB.Connect(pDB);
            return mDB;
        }
    }

    public class AccessOleDB : OleDB
    {
        public override DataBase Create(string pDB)
        {
            return base.Create(AcccessString + pDB);
        }
    }

    public class SQLServerOleDB : OleDB
    {
        public override DataBase Create(string pDB)
        {
            return base.Create(SQLServerString + pDB);
        }
    }

    public class OracleOleDB : OleDB
    {
        public override DataBase Create(string pDB)
        {
            return base.Create(OracleString + pDB);
        }
    }

    public class SybaseOleDB : OleDB
    {
        public override DataBase Create(string pDB)
        {
            return base.Create(SybaseString + pDB);
        }
    }

    public class SingletonOleAccessDB : DBFactory
    {
        private static DataBase mDB = new DataBase(null);
        public override DataBase Create(string pDB)
        {
            if (mDB.isClosed())
            {
                mDB.Close();
                mDB = new AccessOleDB().Create(pDB);
            }

            return mDB;
        }
    }

    public class DataBase : IDisposable
    {
        private IDbConnection mConn = null;
        public IDbConnection Conn
        {
            get { return mConn; }
        }

        private DataBase() { }

        public DataBase(IDbConnection pConn)
        {
            mConn = pConn;
        }

        public void Dispose()
        {
            mConn.Close();
        }

        public int Connect(string pDB)
        {
            mConn.ConnectionString = pDB;
            string[] sSettings = null;
            string[] sParas = null;
            string sDBServerIP = "";

            try
            {
                //先ping看看
                sSettings = ASI.Lib.Text.Parsing.String.Split(pDB, ";");
                if (sSettings != null &&
                    sSettings.Length > 0)
                {
                    foreach (string sPara in sSettings)
                    {
                        sParas = ASI.Lib.Text.Parsing.String.Split(sPara, "=");
                        if (sParas != null &&
                            sParas.Length == 2)
                        {
                            if (sParas[0].ToLower().Contains("server") ||
                                sParas[0].ToLower().Contains("data source"))
                            {
                                sDBServerIP = sParas[1];
                                break;
                            }
                        }
                    }
                }

                int iPingFail = 0;
                while (!ASI.Lib.Comm.Network.NetworkLib.Ping(sDBServerIP,500))
                {
                    iPingFail++;
                    if (iPingFail >= 3)
                    {
                        //連ping三次失敗即判定為網路連線不通
                        ASI.Lib.Log.ErrorLog.Log("DBLib", $"與DB Server IP:{sDBServerIP}網路連線不通! ConnectionString = {pDB}");
                        return -2;
                    }
                    System.Threading.Thread.Sleep(1);
                }

                mConn.Open();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(pDB + " Connect : " + ex.Message);
                ASI.Lib.Log.ErrorLog.Log("DBLib", pDB + " Connect : " + ex.Message);
            }

            return -1;
        }

        public bool isClosed()
        {
            try
            {
                if (mConn.State == System.Data.ConnectionState.Closed ||
                    mConn.State == System.Data.ConnectionState.Broken)
                    return true;
            }
            catch (Exception de)
            {
                Console.WriteLine(de.Message);
                return true;
            }

            return false;
        }

        public void Close()
        {
            try
            {
                if (mConn != null)
                {
                    mConn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataBase Close : " + ex.Message);
            }
        }
    }

    public abstract class CommonRecord : IDisposable
    {
        /// <summary>
        /// SelectByCondition()後存放資料的物件
        /// </summary>
		public List<object> Records = new List<object>();

        private string mDBOName = "";
        public string DBOName
        {
            get { return mDBOName; }
            set { mDBOName = value; }
        }

        private string mTableName = "";
        public string TableName
        {
            get { return mTableName; }
            set { mTableName = mDBOName + value; }
        }

        protected string[] mFieldName = null;

        public IDbCommand mSqlCmd;
        protected IDataReader mResultSet = null;

        protected abstract Object FetchRecord(IDataReader pRs);
        protected virtual int SetRecord(string strSql, Object pObj)
        {
            return -1;
        }

        private IDbConnection mConn = null;

        protected CommonRecord() { }
        protected CommonRecord(IDbConnection pConn)
        {
            mConn = pConn;
            if (mConn != null)
            {
                mSqlCmd = mConn.CreateCommand();
            }
        }

        public string[] GetFieldName()
        {
            return mFieldName;
        }

        public void Dispose()
        {
            CloseQuery();
        }

        protected List<string> SelectDistinctCondition(string pFieldName, string pCondition)
        {
            String sSql = "";

            sSql = "select distinct " + pFieldName + " from " + TableName;
            sSql += " " + pCondition;

            List<string> oRowList = new List<string>();
            if (ExecuteQuery(sSql))
            {
                while (NextRecord())
                {
                    try
                    {
                        string sRow = mResultSet.GetString(0);
                        oRowList.Add(sRow);
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine("SelectDistinctCondition : " + TableName + " " + ex.Message);
                        break;
                    }
                }
                CloseQuery();
            }
            return oRowList;
        }

        protected object SelectMaximumCondition(string pFieldName, string pCondition)
        {
            String sSql = "";

            sSql = "select Max(" + pFieldName + ") from " + TableName;
            sSql += " " + pCondition;

            object oRtn = null;
            if (ExecuteQuery(sSql))
            {
                while (NextRecord())
                {
                    try
                    {
                        oRtn = mResultSet.GetValue(0);
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine("SelectMaximumCondition : " + TableName + " " + ex.Message);
                        break;
                    }
                }
                CloseQuery();
            }
            return oRtn;
        }

        protected object SelectMinimumCondition(string pFieldName, string pCondition)
        {
            String sSql = "";

            sSql = "select Min(" + pFieldName + ") from " + TableName;
            sSql += " " + pCondition;

            object oRtn = null;
            if (ExecuteQuery(sSql))
            {
                while (NextRecord())
                {
                    try
                    {
                        oRtn = mResultSet.GetValue(0);
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine("SelectMinimumCondition : " + TableName + " " + ex.Message);
                        break;
                    }
                }
                CloseQuery();
            }
            return oRtn;
        }

        protected Hashtable SelectCountCondition(string pFieldName, string pCondition)
        {
            String sSql = "";

            sSql = "select count(*), " + pFieldName + " from " + TableName;
            sSql += " " + pCondition;

            Hashtable oHashTable = new Hashtable();
            if (ExecuteQuery(sSql))
            {
                while (NextRecord())
                {
                    try
                    {
                        int intCount = mResultSet.GetInt32(0);
                        string str = mResultSet.GetString(1);
                        oHashTable.Add(str, intCount);
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine("SelectCountCondition : " + TableName + " " + ex.Message);
                        break;
                    }
                }
                CloseQuery();
            }
            return oHashTable;
        }

        protected virtual int SelectByCondition(String pCondition)
        {
            String sSql = "";

            sSql = "select " + mFieldName[0];

            for (int i = 1; i < mFieldName.Length; ++i)
            {
                sSql += ", " + mFieldName[i];
            }

            sSql += " from " + TableName;
            sSql += " " + pCondition;

            Records.Clear();
            if (ExecuteQuery(sSql))
            {
                while (NextRecord())
                {
                    try
                    {
                        Object arow = FetchRecord(mResultSet);
                        if (arow != null)
                        {
                            Records.Add(arow);
                        }
                    }
                    catch (InvalidCastException ex)
                    {
                        throw ex;
                    }
                }
                CloseQuery();
            }
            return Records.Count;
        }

        protected int DeleteByCondition(String pCond)
        {
            String sSql = $" delete from {TableName}";
            sSql += " " + pCond;

            return Execute(sSql);
        }

        protected int DelAll()
        {
            String sSql = $" delete from {TableName}";

            return Execute(sSql);
        }

        protected bool AddParameter(string pParameter, Object pValue, DbType pType)
        {
            try
            {
                DbParameter oDBPara = (DbParameter)mSqlCmd.CreateParameter();
                oDBPara.DbType = pType;
                oDBPara.ParameterName = pParameter;
                oDBPara.Value = pValue;
                mSqlCmd.Parameters.Add(oDBPara);
            }
            catch (Exception ex)
            {
                throw ex;
                //Console.WriteLine("AddParameter : " + mTableName + " " + ex.Message);
            }
            return true;
        }

        protected bool AddParameter(string pParameter, Object pValue)
        {
            try
            {
                DbParameter oDBPara = (DbParameter)mSqlCmd.CreateParameter();
                oDBPara.ParameterName = pParameter;
                if (pValue == null)
                    oDBPara.Value = DBNull.Value;
                else
                    oDBPara.Value = pValue;
                mSqlCmd.Parameters.Add(oDBPara);
            }
            catch (Exception ex)
            {
                throw ex;
                //Console.WriteLine("AddParameter : " + mTableName + " " + ex.Message);
            }
            return true;
        }

        protected int Execute(string pSql)
        {
            string sException = "";

            try
            {
                if (mSqlCmd == null)
                {
                    if (mConn != null)
                    {
                        mSqlCmd = mConn.CreateCommand();
                    }
                    else
                    {
                        sException = "Execute時mConn物件為null!";
                    }
                }

                if (sException != "")
                {
                    ASI.Lib.Log.ErrorLog.Log("DBLib", $"{sException}\r\n SQL = {pSql} \r\n");
                    return -1;
                }

                mSqlCmd.CommandText = pSql;
                return mSqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("DBLib", $"{ex}\r\n SQL = {pSql} \r\n");
                return -1;
            }
        }

        protected bool ExecuteQuery(string pSql)
        {
            string sException = "";

            try
            {
                CloseQuery();

                if (mSqlCmd == null)
                {
                    if (mConn != null)
                    {
                        mSqlCmd = mConn.CreateCommand();
                    }
                    else
                    {
                        sException = "ExecuteQuery時mConn物件為null!";
                    }
                }

                if (sException != "")
                {
                    ASI.Lib.Log.ErrorLog.Log("DBLib", $"{sException}\r\n SQL = {pSql} \r\n");
                    return false;
                }

                mSqlCmd.CommandText = pSql;
                mResultSet = mSqlCmd.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("DBLib", $"{ex}\r\n SQL = {pSql} \r\n");
                return false;
            }
        }

        protected bool NextRecord()
        {
            return mResultSet.Read();
        }

        protected void CloseQuery()
        {
            if (mResultSet != null)
            {
                mResultSet.Close();
                mResultSet = null;
            }
        }
    }

    public class CommonTransaction
    {
        private class TransactionUnit
        {
            public IDbTransaction Transaction;
            public int Result;
        }

        private Dictionary<IDbConnection, TransactionUnit> mRegistry = new Dictionary<IDbConnection, TransactionUnit>();

        public void BeginTransaction(CommonRecord pTable)
        {
            TransactionUnit oTransactionUnit = null;

            if (!mRegistry.ContainsKey(pTable.mSqlCmd.Connection))
            {
                oTransactionUnit = new TransactionUnit();
                oTransactionUnit.Transaction = (IDbTransaction)pTable.mSqlCmd.Connection.BeginTransaction();
                pTable.mSqlCmd.Transaction = oTransactionUnit.Transaction;
                oTransactionUnit.Result = 0;

                mRegistry.Add((IDbConnection)pTable.mSqlCmd.Connection, oTransactionUnit);
            }
            else
            {
                oTransactionUnit = (TransactionUnit)mRegistry[pTable.mSqlCmd.Connection];
                pTable.mSqlCmd.Transaction = oTransactionUnit.Transaction;
            }
        }

        public void SetTransactionResult(CommonRecord pTable, int pResult)
        {
            if (pResult < 0)
            {
                TransactionUnit oTransactionUnit = (TransactionUnit)mRegistry[pTable.mSqlCmd.Connection];
                oTransactionUnit.Result = pResult;
            }
        }

        public bool EndTransaction()
        {
            bool bIsEndResult = true;
            foreach (IDbConnection oDbConnection in mRegistry.Keys)
            {
                TransactionUnit oTransactionUnit = (TransactionUnit)mRegistry[oDbConnection];
                if (oTransactionUnit.Result < 0)
                {
                    bIsEndResult = false;
                    break;
                }
            }

            foreach (IDbConnection oDbConnection in mRegistry.Keys)
            {
                TransactionUnit oTransactionUnit = (TransactionUnit)mRegistry[oDbConnection];

                if (bIsEndResult)
                {
                    oTransactionUnit.Transaction.Commit();
                }
                else
                {
                    oTransactionUnit.Transaction.Rollback();
                }
            }

            mRegistry.Clear();
            return bIsEndResult;
        }
    }

    /// <summary>
    /// 資料庫表格抽象類別
    /// </summary>
    public abstract class DBRecord : IDisposable
    {
        /// <summary>
        /// 資料庫表格記錄集合物件
        /// </summary>
        public List<object> RecordList = new List<object>();

        private string mDBOName = "";
        /// <summary>
        /// 資料庫擁有者名稱
        /// </summary>
		public string DBOName
        {
            get { return mDBOName; }
            set { mDBOName = value; }
        }

        private string mTableName = "";
        /// <summary>
        /// 資料庫表格名稱
        /// </summary>
		public string TableName
        {
            get { return mTableName; }
            set { mTableName = value; }
        }

        /// <summary>
        /// 完整資料庫表格名稱
        /// </summary>
		public string FullTableName
        {
            get
            {
                if (mDBOName.Trim().Length > 0)
                    return mDBOName.Trim() + "." + mTableName;
                else
                    return mTableName;
            }
        }

        private string orgTableName = "";
        public void SwithTableName(string pNewName)
        {
            orgTableName = mTableName;
            mTableName = pNewName;
        }

        public void RestoreTableName()
        {
            if (orgTableName != "")
                mTableName = orgTableName;
        }

        protected string[] mFieldName = null;
        /// <summary>
        /// 資料庫表格欄位陣列
        /// </summary>
		public string[] FieldName
        {
            get { return mFieldName; }
        }

        public IDbCommand mSqlCmd;
        protected IDataReader mResultSet = null;

        protected abstract Object FetchRecord(IDataReader pRs);
        protected virtual int SetRecord(string strSql, Object pObj)
        {
            return -1;
        }

        /// <summary>
        /// 建構式
        /// </summary>
        protected DBRecord() { }

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="pConn"></param>
        protected DBRecord(IDbConnection pConn)
        {
            if (pConn != null)
                mSqlCmd = pConn.CreateCommand();
        }

        public void Dispose()
        {
            CloseQuery();
        }

        /// <summary>
        /// 依條件式擷取出單一表格記錄(指定欄位不重複)
        /// </summary>
        /// <param name="pFieldName"></param>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        protected List<string> SelectDistinctCondition(string pFieldName, string pCondition)
        {
            String sql = "select distinct " + pFieldName + " from " + FullTableName;
            sql = sql + " " + pCondition;

            List<string> rows = new List<string>();
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                    {
                        string arow = mResultSet.GetString(0);
                        rows.Add(arow);
                    }
                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return rows;
        }

        /// <summary>
        /// 依條件擷取最大值表格記錄
        /// </summary>
        /// <param name="pFieldName"></param>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        protected object SelectMaximumCondition(string pFieldName, string pCondition)
        {
            String sql = "select Max(" + pFieldName + ") from " + FullTableName;
            sql = sql + " " + pCondition;

            object ret = null;
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                        ret = mResultSet.GetValue(0);

                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        /// <summary>
        /// 依條件擷取最小值表格記錄
        /// </summary>
        /// <param name="pFieldName"></param>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        protected object SelectMinimumCondition(string pFieldName, string pCondition)
        {
            String sql = "select Min(" + pFieldName + ") from " + FullTableName;
            sql = sql + " " + pCondition;

            object ret = null;
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                        ret = mResultSet.GetValue(0);

                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        /// <summary>
        /// 依條件擷取最上層表格記錄
        /// </summary>
        /// <param name="pRecNo"></param>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        protected virtual int SelectTopCondition(int pRecNo, String pCondition)
        {
            String sql = "select top " + pRecNo.ToString() + " " + mFieldName[0];

            for (int i = 1; i < mFieldName.Length; ++i)
                sql = sql + ", " + mFieldName[i];

            sql = sql + " from " + mTableName;
            sql = sql + " " + pCondition;

            RecordList.Clear();
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                    {
                        Object arow = FetchRecord(mResultSet);
                        if (arow != null)
                            RecordList.Add(arow);
                        else
                            break;
                    }
                    CloseQuery();
                }
            }
            catch (InvalidCastException oe)
            {
                Console.WriteLine("SelectTopCondition : " + oe.Message);
            }
            return RecordList.Count;
        }

        /// <summary>
        /// 依條件擷取表格記錄數目
        /// </summary>
        /// <param name="pFieldName"></param>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        protected Hashtable SelectCountCondition(string pFieldName, string pCondition)
        {
            String sql = "select count(*), " + pFieldName + " from " + FullTableName;
            sql = sql + " " + pCondition;

            Hashtable htRtn = new Hashtable();
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                    {
                        int intCount = mResultSet.GetInt32(0);
                        string str = mResultSet.GetString(1);
                        htRtn.Add(str, intCount);
                    }
                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return htRtn;
        }

        /// <summary>
        /// 依條件擷取表格記錄
        /// </summary>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        public virtual int SelectByCondition(String pCondition)
        {
            String sql = "select " + mFieldName[0];

            for (int i = 1; i < mFieldName.Length; ++i)
                sql = sql + ", " + mFieldName[i];

            sql = sql + " from " + FullTableName;
            sql = sql + " " + pCondition;

            RecordList.Clear();
            try
            {
                if (ExecuteQuery(sql))
                {
                    while (NextRecord())
                    {
                        Object arow = FetchRecord(mResultSet);
                        if (arow != null)
                            RecordList.Add(arow);
                    }
                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return RecordList.Count;
        }

        /// <summary>
        /// 依SQL指令擷取表格記錄
        /// </summary>
        /// <param name="pCondition"></param>
        /// <returns></returns>
        public virtual int SelectBySQL(String pSQL)
        {
            RecordList.Clear();

            try
            {
                if (ExecuteQuery(pSQL))
                {
                    while (NextRecord())
                    {
                        Object arow = FetchRecord(mResultSet);
                        if (arow != null)
                            RecordList.Add(arow);
                    }
                    CloseQuery();
                }
            }
            catch (Exception oe) { throw oe; }
            return RecordList.Count;
        }

        /// <summary>
        /// 新增表格記錄
        /// </summary>
        /// <param name="pItem"></param>
        /// <returns></returns>
        public virtual int Insert(Object pItem)
        {
            String sql = "insert into " + FullTableName;

            String field_str = " (" + mFieldName[0];
            for (int i = 1; i < mFieldName.Length; ++i)
            {
                field_str = field_str + ", " + mFieldName[i];
            }
            sql = sql + field_str + ")";

            int ret = -1;
            try
            {
                ret = SetRecord(sql, pItem);
            }
            catch (Exception ae)
            {
                Console.WriteLine(ae.Message);
                throw ae;
            }
            return ret;
        }

        /// <summary>
        /// 依條件刪除表格記錄
        /// </summary>
        /// <param name="pCond"></param>
        /// <returns></returns>
        protected int DeleteByCondition(String pCond)
        {
            String sql = "delete from " + FullTableName;
            sql = sql + " " + pCond;

            return Execute(sql);
        }

        protected bool AddParameter(string pParameter, Object pValue, DbType pType)
        {
            bool ret = false;
            try
            {
                DbParameter apara = (DbParameter)mSqlCmd.CreateParameter();
                apara.DbType = pType;
                apara.ParameterName = pParameter;
                apara.Value = pValue;
                mSqlCmd.Parameters.Add(apara);
                ret = true;
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        protected bool AddParameter(string pParameter, Object pValue)
        {
            bool ret = false;
            try
            {
                DbParameter apara = (DbParameter)mSqlCmd.CreateParameter();
                apara.ParameterName = pParameter;
                if (pValue == null)
                    apara.Value = DBNull.Value;
                else
                    apara.Value = pValue;
                mSqlCmd.Parameters.Add(apara);
                ret = true;
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        /// <summary>
        /// 執行非擷取命令
        /// </summary>
        /// <param name="pSql"></param>
        /// <returns></returns>
        protected int Execute(string pSql)
        {
            int ret = -1;
            try
            {
                mSqlCmd.CommandText = pSql;
                ret = mSqlCmd.ExecuteNonQuery();
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        /// <summary>
        /// 執行擷取命令
        /// </summary>
        /// <param name="pSql"></param>
        /// <returns></returns>
        protected bool ExecuteQuery(string pSql)
        {
            bool ret = false;
            CloseQuery();
            try
            {
                mSqlCmd.CommandText = pSql;
                mResultSet = mSqlCmd.ExecuteReader();
                ret = true;
            }
            catch (Exception oe) { throw oe; }
            return ret;
        }

        /// <summary>
        /// 讀取目前記錄並移到下一記錄
        /// </summary>
        /// <returns></returns>
        protected bool NextRecord()
        {
            return mResultSet.Read();
        }

        protected void CloseQuery()
        {
            if (mResultSet != null)
            {
                mResultSet.Close();
                mResultSet = null;
            }
        }
    }
}
