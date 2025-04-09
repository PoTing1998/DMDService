using Npgsql;
using Npgsql.Replication;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DCU.DB.Tables 
{
    abstract public class Table<T>
    {

        public enum eSortWay
        {
            Asc,
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
                    commandString = string.Format(@"select* from dbo.{0} order by ins_time dasc;", modelName);
                    break;
            }
            result = Query(commandString, null);
            return result;
        }


        static protected List<T> Query(string commandString, SqlParameter[] sqlParameters = null)
        {
            List<T> modelList = new List<T>();
            IDbConnection connection = new NpgsqlConnection();

            T model;
            try
            {
                connection.ConnectionString = DB.Manager.ConnectionString;
                connection.Open();

                IDbCommand command = new NpgsqlCommand();
                command.CommandText = commandString;
                command.Connection = connection;
                if(sqlParameters != null)
                {
                    command.Parameters.Add(sqlParameters);
                }


                IDataReader reader;
                reader = command.ExecuteReader(CommandBehavior.CloseConnection| CommandBehavior.SingleResult);


                while (reader.Read())
                {
                    // 創建一个 T 類型的新實例，反映當前型的數據  
                    model = Activator.CreateInstance<T>();


                    foreach(PropertyInfo property in model.GetType().GetRuntimeProperties())
                    {
                        Type _modelColType = property.PropertyType;
                        string _modelColName = property.Name;

                        Type _tableValueType = reader[_modelColName].GetType();
                        object _colValue = reader[_modelColName];


                        if (_modelColType == _tableValueType)
                        {
                            property.SetValue(model, _colValue, null);
                        }
                    }
                   
                    modelList.Add(model);

                }
            }
            catch(Exception ex)
            {
                // 如果發生異常，在錯誤訊息中包含 SQL 命令，然後重新拋出異常  
                string errorMsg = string.Format(
                                         "Sql命令:"
                 + Environment.NewLine + "{0}"
                 , commandString
                );
                throw new Exception(errorMsg, ex);
            }
            finally
            {
                connection.Close();
                connection = null;
            }


            return modelList;
        }

        static protected int NonQuery(string commandString, SqlParameter[] sqlParams = null)
        {
            List<T> modelList = new List<T>();
            IDbConnection connection = new NpgsqlConnection();  
            int impactRow = 0;
            try
            {
                connection.ConnectionString = ASI.Wanda.DCU.DB.Manager.ConnectionString;
                connection.Open();

                IDbCommand command = new NpgsqlCommand();
                command.CommandText = commandString;
                command.Connection = connection;
                if (sqlParams != null)
                {
                    command.Parameters.Add(sqlParams);
                }

                impactRow = command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                string errorMsg = $"Sql命令:{Environment.NewLine}{commandString}";
                throw new Exception(errorMsg, ex);
            }
            finally
            {
                connection.Close();
                connection = null;

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
                    // 建構 WHERE 子句中的條件，例如 " ID = '123' AND"    
                    whereString += string.Format(@" {0} = '{1}'", pkName, pks[i]) + "\n" + @"and";
                    i++;
                }
            }
            // 移除 WHERE 子句末尾多餘的 "AND"
            whereString = whereString.Substring(0, whereString.Length - 3);

            string commandString = selectString + "\n" + whereString + ";";

            result = Query(commandString, null);

            return result.FirstOrDefault();
        }
        static protected int Update(params object[] paramObjects)
        {
            string modelName = typeof(T).Name;
            string updateString = string.Empty;
            string setString = string.Empty;
            string whereString = string.Empty;

            //startString
            updateString = string.Format(@"update dbo.{0} set", modelName);

            //contentString
            int i = 0;
            PropertyInfo[] properties = typeof(T).GetProperties();

            //根據屬性類型決定如何處理
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
            setString += string.Format(@", upd_user = '{0}'", ASI.Wanda.DCU.DB.Manager.CurrentUserID) + "\n";
            setString += string.Format(@", upd_time = {0}", ASI.Wanda.DCU.DB.Manager.CurrentSqlTime);

            //endString
            whereString = string.Format(@"where");
            i = 0;
            //遍歷模型的屬性，檢查是否有被標記為主鍵的屬性
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
            whereString = whereString.Substring(0, whereString.Length - 3);

            string commandString = updateString + "\n" + setString + "\n" + whereString + ";";
            //使用 NonQuery 方法執行 SQL 更新，返回影響的行數
            return NonQuery(commandString, null);
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
            endString += string.Format(@", '{0}'", ASI.Wanda.DCU.DB.Manager.CurrentUserID) + "\n";
            endString += string.Format(@", {0}", ASI.Wanda.DCU.DB.Manager.CurrentSqlTime) + "\n";
            endString += string.Format(@", '{0}'", ASI.Wanda.DCU.DB.Manager.CurrentUserID) + "\n";
            endString += string.Format(@", {0}", ASI.Wanda.DCU.DB.Manager.CurrentSqlTime);
            endString = "values(" + "\n" + endString + ")";

            //string commandString = insertString + "\n" + contentString + "\n" + endString + ";";
            string commandString = insertString + "\n" + endString + ";";

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


  

        static protected int DeleteWhere(string where)
        {
            string modelName = typeof(T).Name;
            string deleteString = string.Empty;
            string whereString = where;
            deleteString = string.Format(@"delete from dbo.{0}", modelName);

            string commandString = deleteString + "\n" + whereString + ";";
            return NonQuery(commandString, null);
        }



    }

}
