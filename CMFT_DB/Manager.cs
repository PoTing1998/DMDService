using System;
using System.Data;
using Npgsql;

namespace ASI.Wanda.CMFT.DB
{
    static public class Manager
    {
        #region Properties
        static public string ConnectIP
        {
            get;
            private set;
        } = "localhost";
        static public string ConnPort
        {
            get;
            private set;
        } = "5432";
        static public string DataBaseName
        {
            get;
            private set;
        } = "CMFTDB";
        static public string UserID
        {
            get;
            private set;
        } = "postgres";
        static public string Passward
        {
            get;
            private set;
        } = "postgres";
        static public string ConnectionString
        {
            get;
            private set;
        } = "Server='localhost'; Port='5432'; Database='CMFTDB'; User Id='postgres'; Password='postgres'";
        static public string CurrentUserID
        {
            get;
            private set;
        } = "admin";
        static public string CurrentSqlTime
        {
            get;
            private set;
        } = "clock_timestamp()";
        static public bool IsUseDatabase
        {
            get;
            set;
        } = true;
        static public Action ErrorHandle;
        #endregion

        /// <summary>
        /// 資料庫設定
        /// </summary>
        /// <param name="connIP">連接資料庫的ip地址</param>
        /// <param name="connPort">連接資料庫的port</param>
        /// <param name="Database">連接資料庫的資料庫名稱</param>
        /// <param name="userID">連接資料庫的使用者ID</param>
        /// <param name="Passward">連接資料庫的使用者密碼</param>
        /// <param name="currentUserID">ins_user/upd_user欄位的名稱</param>
        /// <returns></returns>
        static public bool Initializer(string connIP, string connPort, string databaseName, string userID, string passward, string currentUserID)
        { 
            //設定連接資料庫字串所需參數
            ConnectIP = connIP;
            ConnPort = connPort;
            DataBaseName = databaseName;
            UserID = userID;
            Passward = passward;

            //組成資料庫連接字串
            ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; User Id={3}; Password={4};"
                , ConnectIP
                , ConnPort
                , DataBaseName
                , UserID
                , Passward);

            //設定ins_user/ins_time/upd_user/upd_time
            CurrentUserID = currentUserID;
            CurrentSqlTime = "clock_timestamp()";

            if (IsUseDatabase)
            {
                //連接一次資料庫,檢查是否成功
                IDbConnection connection = new NpgsqlConnection();

                try
                {
                    //先確認網路連線正常
                    if (!ASI.Lib.Comm.Network.NetworkLib.Ping(ConnectIP, 1000))
                    {
                        ErrorHandle?.Invoke();
                        throw new Exception($"與{ConnectIP}網路連接失敗!");
                    }

                    try
                    {
                        connection.ConnectionString = ASI.Wanda.CMFT.DB.Manager.ConnectionString;
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandle?.Invoke();
                        throw new Exception($"與CMFT資料庫連接失敗!{Environment.NewLine}", ex);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection = null;
                }
            }
            return true;
        }
    }
}
