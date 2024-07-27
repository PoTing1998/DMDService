using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using ASI.Lib.DB;

using Npgsql;


namespace ASI.Wanda.DMD.DB
{
    static public class Manager 
    {
        #region Properties
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
        } = "DMDDB";
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
        } = "Server='10.107.26.55'; Port='5432'; Database='DMDDB'; User Id='postgres'; Password='postgres'";
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

        static  public  bool Initializer(string Host, string connPort,string Database, string userID, string Password , string currentUserID)
        {

            //設定連接資料庫字串所需參數   
            ConnectIP = Host;
            ConnPort = connPort;
            DataBaseName = Database;
            UserID = userID;
            Passward = Password;

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
                        connection.ConnectionString = ASI.Wanda.DMD.DB.Manager.ConnectionString;
                        connection.Open();
                    } 
                    catch (Exception ex)
                    {
                        ErrorHandle?.Invoke();
                        throw new Exception($"與DMD資料庫連接失敗!{Environment.NewLine}", ex);
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
#endregion