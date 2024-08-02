using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Utility
{
    /// <summary>
    /// 動態載入元件、呼叫Method與Property、以及字串型別轉換。
    /// </summary>
    public class Reflection
    {
        /// <summary>
        /// 從組件名稱找到組件然後找出指定型別，並使用系統啟動項，利用區分大小寫的搜尋，建立它的執行個體
        /// </summary>
        /// <param name="assemblyString">組件名稱</param>
        /// <param name="fullName">執行個體名稱</param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyString, string fullName)
        {
            System.Reflection.Assembly oAssembly = System.Reflection.Assembly.Load(assemblyString);
            object oObj = ASI.Lib.Utility.Reflection.CreateInstance(oAssembly, fullName, false);
            oAssembly = null;
            return oObj;
        }

        /// <summary>
        /// 從組件名稱利用選擇性區分大小寫的搜尋找到組件，然後找出指定型別，並使用系統啟動項，利用區分大小寫的搜尋，建立它的執行個體
        /// </summary>
        /// <param name="assemblyString">組件名稱</param>
        /// <param name="fullName">執行個體名稱</param>
        /// <param name="ignoreCase">是否忽略大小寫</param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyString, string fullName, bool ignoreCase)
        {
            System.Reflection.Assembly oAssembly = System.Reflection.Assembly.Load(assemblyString);
            object oObj = ASI.Lib.Utility.Reflection.CreateInstance(oAssembly, fullName, ignoreCase);
            oAssembly = null;
            return oObj;
        }

        /// <summary>
        /// 從組件找出指定型別，並使用系統啟動項，利用區分大小寫的搜尋，建立它的執行個體
        /// </summary>
        /// <param name="assembly">組件</param>
        /// <param name="fullName">執行個體名稱</param>
        /// <returns></returns>
        public static object CreateInstance(System.Reflection.Assembly assembly, string fullName)
        {
            return ASI.Lib.Utility.Reflection.CreateInstance(assembly, fullName, false);
        }

        /// <summary>
        /// 從組件找出指定型別，並使用系統啟動項，利用選擇性區分大小寫的搜尋，建立它的執行個體
        /// </summary>
        /// <param name="assembly">組件</param>
        /// <param name="fullName">執行個體名稱</param>
        /// <param name="ignoreCase">是否忽略大小寫</param>
        /// <returns></returns>
        public static object CreateInstance(System.Reflection.Assembly assembly, string fullName, bool ignoreCase)
        {
            return assembly.CreateInstance(fullName, ignoreCase);
        }

        #region 設定Call Method

        /// <summary>
        /// 需取得的MethodInfo列表
        /// </summary>
        private System.Collections.Generic.Dictionary<string, System.Reflection.MethodInfo> mMethodInfoList = new Dictionary<string, System.Reflection.MethodInfo>();

        /// <summary>
        /// 取得Method資訊
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public System.Reflection.MethodInfo GetMethodInfo(object obj, string methodName, object[] parameters)
        {
            string sFullName = obj.GetType().FullName;
            string sPropertyName = methodName;
            System.Reflection.MethodInfo oMethodInfo = null;
            if (sFullName == null)
            {
                sFullName = "";
            }
            string sKey = sFullName.Trim() + "." + methodName.Trim();

            if (!mMethodInfoList.TryGetValue(sKey, out oMethodInfo))
            {
                List<Type> oTypeList = new List<Type>();
                //Type[] oTypeArray = new Type[parameters.Length];
                if (parameters != null)
                {
                    foreach (object para in parameters)
                    {
                        oTypeList.Add(para.GetType());
                    }

                    //parameters.CopyTo(oTypeArray, 0);
                }

                oMethodInfo = obj.GetType().GetMethod(methodName, oTypeList.ToArray());
                mMethodInfoList.Add(sKey, oMethodInfo);
            }

            return oMethodInfo;
        }


        /// <summary>
        /// CallMethod委派處理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private delegate object DelegateCallMethod(object obj, System.Reflection.MethodInfo methodInfo, object[] parameters);

        /// <summary>
        /// 引用設定CallMethod
        /// </summary>
        /// <param name="obj">要呼叫方法的物件</param>
        /// <param name="methodInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object InvokeCallMethod(object obj, System.Reflection.MethodInfo methodInfo, object[] parameters)
        {
            try
            {
                return methodInfo.Invoke(obj, parameters);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                
                return null;
            }

        }

        /// <summary>
        /// Reflection呼叫方法
        /// </summary>
        /// <param name="obj">要呼叫方法的物件</param>
        /// <param name="methodInfo">探索方法的屬性 (Attribute) 並提供方法中繼資料 (Metadata) 的存取</param>
        /// <param name="parameters">呼叫方法的參數</param>
        /// <returns></returns>
        public object CallMethod(object obj, System.Reflection.MethodInfo methodInfo, ASI.Lib.Utility.Reflection.Object[] parameters)
        {
            object[] oObjects = null;
            object oObj = null;
            try
            {
                if (parameters != null && parameters.Length > 0)
                {
                    oObjects = new object[parameters.Length];
                    for (int ii = 0; ii < parameters.Length; ii++)
                    {
                        oObj = null;
                        if (ASI.Lib.Utility.Reflection.GetObject(out oObj, parameters[ii]) != 0)
                        {
                            oObjects = null;
                            ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),"CallMethod時，parameters物件轉換失敗!!");
                            return null;
                        }
                        oObjects[ii] = oObj;
                    }
                }

                return CallMethod(obj, methodInfo, oObjects);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                return null;
            }

        }

        /// <summary>
        /// Reflection呼叫方法
        /// </summary>
        /// <param name="obj">要呼叫方法的物件</param>
        /// <param name="methodName">呼叫方法的名稱</param>
        /// <param name="parameters">呼叫方法的參數</param>
        /// <returns></returns>
        public object CallMethod(object obj, string methodName, object[] parameters)
        {
            string sFullName = obj.GetType().FullName;
            string sMethodName = methodName;
            string sKey = sFullName + "." + sMethodName;
            System.Reflection.MethodInfo oMethodInfo = null;
            oMethodInfo = this.GetMethodInfo(obj, methodName, parameters);
            return CallMethod(obj, oMethodInfo, parameters);
        }

        /// <summary>
        /// Reflection呼叫方法
        /// </summary>
        /// <param name="obj">要呼叫方法的物件</param>
        /// <param name="methodInfo">探索方法的屬性 (Attribute) 並提供方法中繼資料 (Metadata) 的存取</param>
        /// <param name="parameters">呼叫方法的參數</param>
        /// <returns></returns>
        public object CallMethod(object obj, System.Reflection.MethodInfo methodInfo, object[] parameters)
        {
            string sFullName = obj.GetType().FullName;
            string sMethodName = methodInfo.Name;
            string sKey = sFullName + "." + sMethodName;
            try
            {
                object oObj = null;

                System.ComponentModel.ISynchronizeInvoke oISynchronizeInvoke = (System.ComponentModel.ISynchronizeInvoke)obj;
                if (oISynchronizeInvoke.InvokeRequired)
                {
                    DelegateCallMethod oDelegateCallMethod = new DelegateCallMethod(this.InvokeCallMethod);
                    oObj = oISynchronizeInvoke.Invoke(oDelegateCallMethod, new object[3] { obj, methodInfo, parameters });
                }
                else
                {
                    oObj = methodInfo.Invoke(obj, parameters);
                }

                return oObj;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                return null;
            }
        }

        #endregion

        #region 設定Set Property

        /// <summary>
        /// 需取得的PropertyInfo列表
        /// </summary>
        private System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo> mPropertyInfoList = new Dictionary<string, System.Reflection.PropertyInfo>();

        /// <summary>
        /// 取得Property資訊
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public System.Reflection.PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            string sFullName = obj.GetType().FullName;
            string sPropertyName = propertyName;
            try
            {
                System.Reflection.PropertyInfo oPropertyInfo = null;
                if (sFullName == null)
                {
                    sFullName = "";
                }
                string sKey = sFullName.Trim() + "." + sPropertyName.Trim();

                if (!mPropertyInfoList.TryGetValue(sKey, out oPropertyInfo))
                {
                    oPropertyInfo = obj.GetType().GetProperty(propertyName);
                    mPropertyInfoList.Add(sKey, oPropertyInfo);
                }
                return oPropertyInfo;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this), ex);
                return null;
            }


        }

        /// <summary>
        /// SetProperty委派處理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="input"></param>
        private delegate void DelegateSetProperty(object obj, System.Reflection.PropertyInfo propertyInfo, object input);

        /// <summary>
        /// 引用設定Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="input"></param>
        private void InvokeSetProperty(object obj, System.Reflection.PropertyInfo propertyInfo, object input)
        {
            try
            {
                propertyInfo.SetValue(obj, input, null);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
            }

        }

        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="input"></param>
        public void SetProperty(object obj, System.Reflection.PropertyInfo propertyInfo, ASI.Lib.Utility.Reflection.Object input)
        {
            object oObj = null;
            try
            {
                if (obj == null)
                {
                    throw new System.Exception("目標物件不可為Null!!");
                }
                if (propertyInfo == null)
                {
                    throw new System.Exception("PropertyInfo不可為Null!!");
                }
                if (ASI.Lib.Utility.Reflection.GetObject(out oObj, input) != 0)
                {
                    ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),"SetProperty失敗 !! Property=" + propertyInfo.Name + " Type=" + input.Type + " Value=" + input.Value);
                }
                else
                {
                    this.SetProperty(obj, propertyInfo, oObj);
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
            }
        }

        /// <summary>
        /// 設定Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="input"></param>
        public void SetProperty(object obj, string propertyName, object input)
        {
            try
            {
                if (obj == null)
                {
                    throw new System.Exception("目標物件不可為Null");
                }
                System.Reflection.PropertyInfo oPropertyInfo = this.GetPropertyInfo(obj, propertyName);
                if (oPropertyInfo == null)
                {
                    throw new System.Exception("目標物件找不到Property[" + propertyName + "]");
                }
                SetProperty(obj, oPropertyInfo, input);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                throw ex;
            }
        }

        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="input"></param>
        public void SetProperty(object obj, System.Reflection.PropertyInfo propertyInfo, object input)
        {
            string sFullName = obj.GetType().FullName;
            string sMethodName = propertyInfo.Name;
            string sKey = sFullName + "." + sMethodName;
            try
            {
                if (obj == null)
                {
                    throw new System.Exception("目標物件不可為Null!!");
                }

                if (propertyInfo == null)
                {
                    throw new System.Exception("PropertyInfo不可為Null");
                }

                System.ComponentModel.ISynchronizeInvoke oISynchronizeInvoke = (System.ComponentModel.ISynchronizeInvoke)obj;
                if (oISynchronizeInvoke.InvokeRequired)
                {
                    DelegateSetProperty oDelegateSetProperty = new DelegateSetProperty(this.InvokeSetProperty);
                    oISynchronizeInvoke.Invoke(oDelegateSetProperty, new object[3] { obj, propertyInfo, input });
                    oDelegateSetProperty = null;
                }
                else
                {
                    propertyInfo.SetValue(obj, input, null);
                }
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(TypeDescriptor.GetClassName(this),ex);
                throw ex;
            }
        }

        #endregion
           
        /// <summary>
        /// 取得Type
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static Type GetType(string typeFullName)
        {
            try
            {
                string sTypeName = typeFullName;
                Type oType = null;
                if (sTypeName != null && sTypeName.Trim() != "")
                {
                    sTypeName = sTypeName.Trim();

                    if (sTypeName.IndexOf(",") >= 0)
                    {
                        oType = Type.GetType(sTypeName);
                    }
                    else
                    {
                        int iLastDot = sTypeName.LastIndexOf(".");
                        string sClassName = null;
                        string sNamespace = null;
                        if (iLastDot < 0)
                        {
                            sClassName = sTypeName.ToLower();
                            sNamespace = "System";
                            //縮寫
                            if (sClassName == "int")
                            {
                                oType = typeof(System.Int32);
                            }
                            else if (sClassName == "float")
                            {
                                oType = typeof(System.Single);
                            }
                            else if (sClassName == "string")
                            {
                                oType = typeof(System.String);
                            }
                            else if (sClassName == "decimal")
                            {
                                oType = typeof(System.Decimal);
                            }
                            else if (sClassName == "double")
                            {
                                oType = typeof(System.Double);
                            }
                            else
                            {
                                if (sClassName.Length > 1)
                                {
                                    sClassName = sTypeName.Substring(0, 1).ToUpper() + sTypeName.Substring(1).ToLower();
                                }
                                else
                                {
                                    sClassName = sTypeName.Substring(0, 1).ToUpper();
                                }

                                oType = Type.GetType(sNamespace + "." + sClassName);
                            }
                        }
                        else if (iLastDot >= 0 && iLastDot <= (sTypeName.Length - 1))
                        {
                            sNamespace = sTypeName.Substring(0, iLastDot);
                            sClassName = sTypeName.Substring(iLastDot + 1);
                            if (sNamespace == "System")
                            {
                                oType = Type.GetType(sTypeName);
                            }
                            else if (sNamespace == "System.Drawing")
                            {
                                oType = Type.GetType(sNamespace + "." + sClassName + "," + sNamespace);
                                if (oType != null)
                                {
                                    return oType;
                                }
                                if (sClassName == "Color")
                                {
                                    oType = typeof(System.Drawing.Color);
                                }                                
                                else if (sClassName == "SolidBrush")
                                {
                                    oType = typeof(System.Drawing.SolidBrush);
                                }
                                else if (sClassName == "Pen")
                                {
                                    oType = typeof(System.Drawing.Pen);
                                }
                                else if (sClassName == "Brush")
                                {
                                    oType = typeof(System.Drawing.Brush);
                                }
                                else if (sClassName == "Point")
                                {
                                    oType = typeof(System.Drawing.Point);
                                }
                                else if (sClassName == "PointF")
                                {
                                    oType = typeof(System.Drawing.PointF);
                                }
                                else if (sClassName == "FontStyle")
                                {
                                    oType = typeof(System.Drawing.FontStyle);
                                }
                            }
                        }
                    }
                }


                if (oType == null)
                {
                    ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "GetTypeFromName(string name)[name=" + typeFullName + "]的轉換失敗!!");
                }
                return oType;


            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "發生例外錯誤 GetTypeFromName(string name)[name=" + typeFullName + "]的轉換失敗!!");
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", ex);
                return null;
            }

        }

        /// <summary>
        /// 將文字轉成物件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dtatType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetObject(out object obj, string dtatType, string value)
        {
            obj = null;
            try
            {
                Type oType = null;
                oType = ASI.Lib.Utility.Reflection.GetType(dtatType);
                return ASI.Lib.Utility.Reflection.GetObject(out obj, oType, value);
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "GetObject發生例外錯誤!! GetObject(out object , string dtatType , string value)[dtatType=" + dtatType + " value=" + value + "]的轉換失敗");
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", ex);
                return -1;
            }
        }

        /// <summary>
        /// 將文字轉成物件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="refObj"></param>
        /// <returns></returns>
        public static int GetObject(out object obj, ASI.Lib.Utility.Reflection.Object refObj)
        {
            obj = null;
            try
            {
                obj = null;
                object oReObj = null;
                int iReInt = 0;
                iReInt = GetObject(out oReObj, refObj.Type, refObj.Value);
                obj = oReObj;
                return iReInt;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", ex);
                return -1;
            }

        }

        /// <summary>
        /// 將文字轉成物件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetObject(out object obj, Type dataType, string value)
        {

            string sValue = value;
            Type oType = dataType;
            string sX, sY;
            string[] sPoint = null;
            obj = null;

            try
            {
                if (oType != null && sValue != null)
                {
                    if (oType.Name == "PointF" || oType.Name == "Point")
                    {
                        //特殊格式x=?;y=?
                        try
                        {
                            sValue = sValue.ToLower().Trim();
                            sValue = sValue.Replace(" ", "");
                            sPoint = ASI.Lib.Text.Parsing.String.Split(sValue, ";");
                            sX = sPoint[0].Replace("x=", "");
                            sY = sPoint[1].Replace("y=", "");
                            if (oType.Name == "PointF")
                            {
                                obj = (object)(new System.Drawing.PointF(Convert.ToInt32(sX), Convert.ToInt32(sY)));
                            }
                            else
                            {
                                obj = (object)(new System.Drawing.Point(Convert.ToInt32(sX), Convert.ToInt32(sY)));
                            }
                            return 0;
                        }
                        catch (System.Exception ex)
                        {
                            ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", ex);
                            ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "GetObject(Type dtatType , string value)[dtatType=" + oType.FullName + " value=" + sValue + "]的格式不合法");
                            return -2;
                        }
                    }
                    else
                    {

                        obj = System.ComponentModel.TypeDescriptor.GetConverter(oType).ConvertFromString(sValue);
                    }

                }
                else
                {
                    ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "GetObject發生例外錯誤!! GetObject(Type dtatType , string value)[dtatType=" + oType + " value=" + sValue + "]的轉換失敗");
                    return -2;
                }

                return 0;

            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", "GetObject發生例外錯誤!! GetObject(Type dtatType , string value)[dtatType=" + oType + " value=" + sValue + "]的轉換失敗");
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Utility.Reflection", ex);
                return -1;
            }

        }

        /// <summary>
        /// 表示物件的內容
        /// </summary>
        public struct Object
        {
            /// <summary>
            /// Type參數
            /// </summary>
            public string Type;
            /// <summary>
            /// Value參數
            /// </summary>
            public string Value;
            /// <summary>
            /// Object建構子
            /// </summary>
            /// <param name="type"></param>
            /// <param name="val"></param>
            public Object(string type, string val)
            {
                Type = type;
                Value = val;
            }
        }
    }
}
