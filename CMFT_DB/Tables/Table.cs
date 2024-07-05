using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace ASI.Wanda.CMFT.DB.Tables
{
    abstract public class Table<T>
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

        static public List<T> SelectAll(eSortWay inserTimeSortWay = eSortWay.Asc)
        {
       
            List<T> result = null;
            string modelName = typeof(T).Name;
            string commandString = string.Empty;
            switch (inserTimeSortWay)
            {
                case eSortWay.Asc:
                    commandString = string.Format(@"select* from dbo.{0} order by ins_time asc;", modelName);
                    break;
                case eSortWay.Desc:
                    commandString = string.Format(@"select* from dbo.{0} order by ins_time desc;", modelName);
                    break;
                default: break;
            }
            result = Query(commandString, null);

            return result;
        }

        #endregion

        #region Protected

        static protected List<T> Query(string commandString, SqlParameter[] sqlParams = null)
        {
            List<T> modelList = new List<T>();
            if (ASI.Wanda.CMFT.DB.Manager.IsUseDatabase)
            {
                IDbConnection connection = new NpgsqlConnection();

                T model;

                try
                {
                    //先確認網路連線正常
                    if (!ASI.Lib.Comm.Network.NetworkLib.Ping(ASI.Wanda.CMFT.DB.Manager.ConnectIP, 1000))
                    {
                        Manager.ErrorHandle?.Invoke();
                        throw new Exception($"與{ASI.Wanda.CMFT.DB.Manager.ConnectIP}網路連接失敗!");
                    }

                    connection.ConnectionString = ASI.Wanda.CMFT.DB.Manager.ConnectionString;
                    connection.Open();

                    IDbCommand command = new NpgsqlCommand();
                    command.CommandText = commandString;
                    command.Connection = connection;
                    if (sqlParams != null)
                    {
                        command.Parameters.Add(sqlParams);
                    }

                    IDataReader reader;
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);

                    while (reader.Read())
                    {
                        model = Activator.CreateInstance<T>();

                        foreach (PropertyInfo property in model.GetType().GetProperties())
                        {
                            //遍歷取得model的各個屬性類型
                            Type _modelColType = property.PropertyType;
                            //遍歷取得model的各個屬性名稱
                            string _modelColName = property.Name;

                            //依據屬性名稱取得此表欄位的類型
                            Type _tableValueType = reader[_modelColName].GetType();
                            //依據屬性名稱取得此表欄位的內容值
                            object _colValue = reader[_modelColName];

                            //確認Mapping時屬性是否一致
                            if (_modelColType == _tableValueType)
                            {
                                //將資料表的值Mapping至Model
                                property.SetValue(model, _colValue, null);
                            }
                        }
                        modelList.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    string errorMsg = $"Sql命令:{Environment.NewLine}{commandString}";
                    throw new Exception(errorMsg, ex);
                }
                finally
                {
                    connection.Close();
                    connection = null;
                }
            }
          
            return modelList;
        }

        static protected int NonQuery(string commandString, SqlParameter[] sqlParams = null)
        {
            int impactRow = 0;

            if (ASI.Wanda.CMFT.DB.Manager.IsUseDatabase)
            {
                IDbConnection connection = new NpgsqlConnection();
                try
                {
                    //先確認網路連線正常
                    if (!ASI.Lib.Comm.Network.NetworkLib.Ping(ASI.Wanda.CMFT.DB.Manager.ConnectIP, 1000))
                    {
                        Manager.ErrorHandle?.Invoke();
                        throw new Exception($"與{ASI.Wanda.CMFT.DB.Manager.ConnectIP}網路連接失敗!");
                    }

                    connection.ConnectionString = ASI.Wanda.CMFT.DB.Manager.ConnectionString;
                    connection.Open();


                    IDbCommand command = new NpgsqlCommand();
                    command.CommandText = commandString;
                    command.Connection = connection;
                    if (sqlParams != null)
                    {
                        command.Parameters.Add(sqlParams);
                    }

                    impactRow = command.ExecuteNonQuery();

                    Manager.onNonQueryExcuted?.Invoke(commandString);
                }
                catch (Exception ex)
                {
                    string errorMsg = $"Sql命令:{Environment.NewLine}{commandString}";
                    throw new Exception(errorMsg, ex);
                }
                finally
                {
                    connection.Close();
                    connection = null;
                }

            }
           

            return impactRow;
        }

        static protected T Select(params object[] pks)
        {
            ICollection<T> result = null;
            string modelName = typeof(T).Name;
            string selectString = string.Empty;
            string whereString = string.Empty;

            selectString = string.Format(@"select* from dbo.{0}", modelName);

            whereString = string.Format(@"where");
            int i = 0;
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                KeyAttribute attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) as KeyAttribute;
                if (attribute != null && pks[i] != null)
                {
                    string pkName = property.Name;
                    whereString += string.Format(@" {0} = '{1}'", pkName, pks[i]) + "\n" + @"and";
                    i++;
                }
            }
            whereString = whereString.Substring(0, whereString.Length - 3);

            string commandString = selectString + "\n" + whereString + ";";

            result = Query(commandString, null);

            return result.FirstOrDefault();
        }

        static protected List<T> SelectWhere(string where, eSortWay inserTimeSortWay = eSortWay.Asc)
        {
            ICollection<T> result = null;
            string modelName = typeof(T).Name;
            string selectString = string.Empty;
            string whereString = where;
            string orederByString = string.Empty;
            selectString = string.Format(@"select* from dbo.{0}", modelName);
            switch (inserTimeSortWay)
            {
                case eSortWay.Asc:
                    orederByString = string.Format(@"order by ins_time asc;", modelName);
                    break;
                case eSortWay.Desc:
                    orederByString = string.Format(@"order by ins_time desc;", modelName);
                    break;
                default: break;
            }

            string commandString = selectString + "\n" + whereString + "\n" + orederByString + ";";

            result = Query(commandString, null);

            return result.ToList();
        }

        static protected int Insert(params object[] paramObjects)
        {
            string modelName = typeof(T).Name;
            string insertString = string.Empty;
            //string contentString = string.Empty;
            string endString = string.Empty;

            //startString            
            insertString = string.Format(@"insert into dbo.{0} ", modelName);

            //GetProperties()方法不以特定順序回傳屬性，特定順序指字母順序或宣告順序，其隨機性會造成欄位填入對應錯誤
            //PropertyInfo[] properties = typeof(T).GetProperties();            
            //contentString 
            //foreach (PropertyInfo property in properties)
            //{
            //    contentString += "\n" + string.Format(@", {0}", property.Name);
            //}
            //contentString += ")";
            //contentString = "  " + contentString.Substring(3, contentString.Length - 3);

            foreach (object paramObject in paramObjects)
            {
                if (paramObject == null)
                {
                    endString += "\n" + string.Format(@", null ");
                    continue;
                }

                Type propertyType = paramObject.GetType();
                if (propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(Guid)))
                {
                    endString += "\n" + string.Format(@", '{0}'", paramObject.ToString());
                }
                else if (propertyType.Equals(typeof(DateTime)))
                {
                    endString += "\n" + string.Format(@", '{0}'", Convert.ToDateTime(paramObject).ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {

                    endString += "\n" + string.Format(@", {0}", paramObject.ToString());
                }
            }
            endString = "  " + endString.Substring(3, endString.Length - 3) + "\n";
            endString += string.Format(@", '{0}'", ASI.Wanda.CMFT.DB.Manager.CurrentUserID) + "\n";
            endString += string.Format(@", {0}", ASI.Wanda.CMFT.DB.Manager.CurrentSqlTime) + "\n";
            endString += string.Format(@", '{0}'", ASI.Wanda.CMFT.DB.Manager.CurrentUserID) + "\n";
            endString += string.Format(@", {0}", ASI.Wanda.CMFT.DB.Manager.CurrentSqlTime);
            endString = "values(" + "\n" + endString + ")";

            //string commandString = insertString + "\n" + contentString + "\n" + endString + ";";
            string commandString = insertString + "\n" + endString + ";";

            return NonQuery(commandString, null);
        }

        static protected int Update(params object[] paramObjects)
        {
            string modelName = typeof(T).Name;
            string updateString = string.Empty;
            string setString = string.Empty;
            string whereString = string.Empty;

            //updateString
            updateString = string.Format(@"update dbo.{0} set", modelName);

            //setString
            int i = 0;
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (object paramObject in paramObjects)
            {                
                Type propertyType = paramObject.GetType();

                if (propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(Guid)))
                {
                    setString += "\n" + string.Format(@", {0} = '{1}'", properties[i].Name, paramObject);
                }
                else if (propertyType.Equals(typeof(DateTime)))
                {
                    setString += "\n" + string.Format(@", {0} = '{1}'", properties[i].Name, Convert.ToDateTime(paramObject).ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    setString += "\n" + string.Format(@", {0} = {1}", properties[i].Name, paramObject);
                }
                i++;
            }
            setString = "  " + setString.Substring(3, setString.Length - 3) + "\n";
            setString += string.Format(@", upd_user = '{0}'", ASI.Wanda.CMFT.DB.Manager.CurrentUserID) + "\n";
            setString += string.Format(@", upd_time = {0}", ASI.Wanda.CMFT.DB.Manager.CurrentSqlTime);

            //whereString
            whereString = string.Format(@"where");
            i = 0;
            foreach (PropertyInfo property in properties)
            {
                KeyAttribute attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) as KeyAttribute;
                if (attribute != null)
                {
                    string pkName = property.Name;
                    whereString += string.Format(@" {0} = '{1}'", pkName, paramObjects[i]) + "\n" + @"and";
                    i++;
                }
            }
            whereString = whereString.Substring(0, whereString.Length - 4);//移除\n跟and


            string commandString = updateString + "\n" + setString + "\n" + whereString + ";";

            return NonQuery(commandString, null);
        }

        static protected int UpdateWhere(Dictionary<string, object> columnVals, string where)
        {
            string modelName = typeof(T).Name;
            string updateString = string.Empty;
            string setString = string.Empty;
            string whereString = string.Empty;

            //updateString
            updateString = string.Format(@"update dbo.{0} set", modelName);

            //setString
            for (int ii = 0; ii < columnVals.Count; ii++)
            {
                string sColumnName = columnVals.ElementAt(ii).Key;
                object oValue = columnVals.ElementAt(ii).Value;

                PropertyInfo property = typeof(T).GetProperty(sColumnName);
                Type propertyType = property.PropertyType;

                if (propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(Guid)))
                {
                    setString += "\n" + string.Format(@", {0} = '{1}'", sColumnName, oValue);
                }
                else if (propertyType.Equals(typeof(DateTime)))
                {
                    setString += "\n" + string.Format(@", {0} = '{1}'", sColumnName, Convert.ToDateTime(oValue).ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    setString += "\n" + string.Format(@", {0} = '{1}'", sColumnName, oValue);
                }
            }
            setString = "  " + setString.Substring(3, setString.Length - 3) + "\n";
            setString += string.Format(@", upd_user = '{0}'", ASI.Wanda.CMFT.DB.Manager.CurrentUserID) + "\n";
            setString += string.Format(@", upd_time = {0}", ASI.Wanda.CMFT.DB.Manager.CurrentSqlTime);

            //whereString
            whereString = where;

            string commandString = updateString + "\n" + setString + "\n" + whereString + ";";

            return NonQuery(commandString, null);
        }
              
        static protected int Delete(params object[] pks)
        {
            string modelName = typeof(T).Name;
            string deleteString = string.Empty;
            string whereString = string.Empty;

            deleteString = string.Format(@"delete from dbo.{0}", modelName);
            whereString = string.Format(@"where");
            int i = 0;
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                KeyAttribute attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) as KeyAttribute;
                if (attribute != null && pks[i] != null)
                {
                    string pkName = property.Name;
                    whereString += string.Format(@" {0} = '{1}'", pkName, pks[i]) + "\n" + @"and";
                    i++;
                }
            }
            whereString = whereString.Substring(0, whereString.Length - 3);

            string commandString = deleteString + "\n" + whereString + ";";
            return NonQuery(commandString, null);
        }

        static protected int DeleteWhere(string where)
        {
            string modelName = typeof(T).Name;
            string deleteString = string.Empty;
            string whereString = where;
            deleteString = string.Format(@"delete from dbo.{0}", modelName);

            string commandString = deleteString + "\n" + whereString + ";";
            return NonQuery(commandString, null);
        }

        static public bool IsExist()
        {
            string modelName = typeof(T).Name;
            string commandString = string.Format( @"select exists(select from pg_tables where schemaname = 'dbo' AND tablename = '{0}' );", modelName);
            bool isExists = false;

            if (ASI.Wanda.CMFT.DB.Manager.IsUseDatabase)
            {
                IDbConnection connection = new NpgsqlConnection();

                T model;
               
                try
                {
                    //先確認網路連線正常
                    if (!ASI.Lib.Comm.Network.NetworkLib.Ping(ASI.Wanda.CMFT.DB.Manager.ConnectIP, 1000))
                    {
                        Manager.ErrorHandle?.Invoke();
                        throw new Exception($"與{ASI.Wanda.CMFT.DB.Manager.ConnectIP}網路連接失敗!");
                    }

                    connection.ConnectionString = ASI.Wanda.CMFT.DB.Manager.ConnectionString;
                    connection.Open();

                    IDbCommand command = new NpgsqlCommand();
                    command.CommandText = commandString;
                    command.Connection = connection;
                  
                    IDataReader reader;
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);

                    var result = reader.Read();
                    if(result == true)
                    {
                        string strResult = reader["exists"].ToString();
                        isExists = bool.Parse(strResult);
                    }
                    
                }
                catch (Exception ex)
                {
                    string errorMsg = $"Sql命令:{Environment.NewLine}{commandString}";
                    throw new Exception(errorMsg, ex);
                }
                finally
                {
                    connection.Close();
                    connection = null;
                }
            }

            return isExists;
        }

        #endregion
    }
}
