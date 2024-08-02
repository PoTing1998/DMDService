using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Web
{
    /// <summary>
    /// Web Service
    /// </summary>
    public class WebService
    {
        /// <summary>
        /// 動態呼叫Web Service
        /// </summary>
        /// <param name="url">WebService的http形式的位址，EX:http://www.yahoo.com/Service/Service.asmx </param>
        /// <param name="namespace">欲呼叫的WebService的namespace</param>
        /// <param name="classname">欲呼叫的WebService的class name</param>
        /// <param name="methodName">欲呼叫的WebService的method name</param>
        /// <param name="args">參數列表，請將每個參數分別放入object[]中</param>
        /// <returns>WebService的執行結果</returns>
        public static object InvokeWebservice(string url, string @namespace, string classname, string methodName, object[] args)
        {
            object oReItem = null;
            System.Net.WebClient oWebClient = null;
            System.IO.Stream oStream = null;
            System.Web.Services.Description.ServiceDescription oServiceDesp = null;
            System.Web.Services.Description.ServiceDescriptionImporter oServiceDespImport = null;
            System.CodeDom.CodeNamespace oCodeNamespace = null;
            System.CodeDom.CodeCompileUnit oCodeComUnit = null;
            Microsoft.CSharp.CSharpCodeProvider oCSProvider = null;
            System.CodeDom.Compiler.ICodeCompiler oCodeCom = null;
            System.Text.StringBuilder oSB = null;
            System.Reflection.MethodInfo oInvokeMethod = null;
            System.CodeDom.Compiler.CompilerResults oComResult = null;
            Type[] oArgsType = null;
            System.Reflection.Assembly oAssembly = null;
            Type oType = null;
            object oTypeInstance = null;

            try
            {
                oWebClient = new System.Net.WebClient();
                //讀取WSDL檔，確認Web Service描述內容 
                oStream = oWebClient.OpenRead(url + "?WSDL");
                oServiceDesp = System.Web.Services.Description.ServiceDescription.Read(oStream);
                //將讀取到的WSDL檔描述import進來 
                oServiceDespImport = new System.Web.Services.Description.ServiceDescriptionImporter();
                oServiceDespImport.AddServiceDescription(oServiceDesp, "", "");
                oCodeNamespace = new System.CodeDom.CodeNamespace(@namespace);

                //指定要編譯程式 
                oCodeComUnit = new System.CodeDom.CodeCompileUnit();
                oCodeComUnit.Namespaces.Add(oCodeNamespace);
                oServiceDespImport.Import(oCodeNamespace, oCodeComUnit);

                //以C#的Compiler來進行編譯 
                oCSProvider = new Microsoft.CSharp.CSharpCodeProvider();
#pragma warning disable CS0618 // 'CodeDomProvider.CreateCompiler()' 已經過時: 'Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class. Those inheriting from CodeDomProvider must still implement this interface, and should exclude this warning or also obsolete this method.'
                oCodeCom = oCSProvider.CreateCompiler();
#pragma warning restore CS0618 // 'CodeDomProvider.CreateCompiler()' 已經過時: 'Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class. Those inheriting from CodeDomProvider must still implement this interface, and should exclude this warning or also obsolete this method.'
                System.CodeDom.Compiler.CodeDomProvider provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");

                //設定編譯參數 
                System.CodeDom.Compiler.CompilerParameters tComPara = new System.CodeDom.Compiler.CompilerParameters();
                tComPara.GenerateExecutable = false;
                tComPara.GenerateInMemory = true;

                //取得編譯結果 
                oComResult = oCodeCom.CompileAssemblyFromDom(tComPara, oCodeComUnit);

                //如果編譯有錯誤的話，將錯誤訊息丟出 
                if (true == oComResult.Errors.HasErrors)
                {
                    oSB = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError tComError in oComResult.Errors)
                    {
                        oSB.Append(tComError.ToString());
                        oSB.Append(System.Environment.NewLine);
                    }
                    throw new Exception(oSB.ToString());
                }

                //取得編譯後產出的Assembly 
                oAssembly = oComResult.CompiledAssembly;
                oType = oAssembly.GetType(@namespace + "." + classname, true, true);
                oTypeInstance = Activator.CreateInstance(oType);

                //若WS有overload的話，需明確指定參數內容
                oArgsType = null;
                if (args == null)
                {
                    oArgsType = new Type[0];
                }
                else
                {
                    int tArgsLength = args.Length;
                    oArgsType = new Type[tArgsLength];
                    for (int ii = 0; ii < tArgsLength; ii++)
                    {
                        oArgsType[ii] = args[ii].GetType();
                    }
                }

                //若沒有overload的話，第二個參數便不需要，這邊要注意的是WsiProfiles.BasicProfile1_1本身不支援
                //Web Service overload，因此需要改成不遵守WsiProfiles.BasicProfile1_1協議 
                oInvokeMethod = oType.GetMethod(methodName, oArgsType);
                //實際invoke該method 
                oReItem = oInvokeMethod.Invoke(oTypeInstance, args);
                return oReItem;
            }
            catch (System.Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Web.WebService", ex);
                return null;
            }
            finally
            {
                oReItem = null;
                oServiceDesp = null;
                oServiceDespImport = null;
                oCodeNamespace = null;
                oCodeComUnit = null;
                oCodeCom = null;
                oSB = null;
                oInvokeMethod = null;
                oComResult = null;
                oArgsType = null;
                oAssembly = null;
                oType = null;
                oTypeInstance = null;

                if (oStream != null)
                {
                    oCSProvider.Dispose();
                    oCSProvider = null;
                }
                if (oStream != null)
                {
                    oStream.Dispose();
                    oStream = null;
                }
                if (oWebClient != null)
                {
                    oWebClient.Dispose();
                    oWebClient = null;
                }


            }

        }
    }
}
