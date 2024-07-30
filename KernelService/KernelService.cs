using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD
{
    public partial class KernelService : ServiceBase
    {

        private string mServer = "";
        private AppDomain taskkernel_domain = null;
        public KernelService()
        {
            InitializeComponent();
        }
        public void Start(string[] args)
        {
            this.OnStart(args);
        }
        public void Stop()
        {
            this.OnStop();
        }
        protected override void OnStart(string[] args)
        {
            // TODO: 在此加入啟動服務的程式碼。
            try
            {
                // set current directory to execution file folder 
                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                mServer = ASI.Lib.Config.ConfigApp.Instance.HostName; 

                //ASI.Lib.Log.DebugLog.Log("KernelService", $"OnStart CurrentDomain.BaseDirectory = [{System.AppDomain.CurrentDomain.BaseDirectory}]");
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ExecTaskKernel));  // startup TaskKernel
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("KernelService", ex.Message);
            }
        }
        private void ExecTaskKernel(object pNull)
        {
            try
            {
                ASI.Lib.Log.LogFile.Log(mServer, "KernelService", $"ExecTaskKernel CurrentDirectory = [{System.IO.Directory.GetCurrentDirectory()}]");
                taskkernel_domain = AppDomain.CreateDomain("TaskKernel");
                string exe_name = System.AppDomain.CurrentDomain.BaseDirectory + "TaskKernel.exe";
                taskkernel_domain.ExecuteAssembly(exe_name, new string[] { "TaskKernel" });
                //taskkernel_domain.ExecuteAssemblyByName("ASI.Wanda.DMD.TaskKernel");
            }
            catch (Exception ex)
            {
                
                ASI.Lib.Log.ErrorLog.Log("KernelService", ex.Message);
                taskkernel_domain = null;
            }
        }
        protected override void OnStop()
        {

            // TODO: 在此加入停止服務所需執行的終止程式碼。
            try
            {
                // send stop message to TaskKernel
                ASI.Lib.Log.LogFile.Log(mServer, "KernelService", "開始停止服務");
                ASI.Lib.Process.MSGStopProc msg = new ASI.Lib.Process.MSGStopProc(new ASI.Lib.Process.MSGFrameBase("KernelService", "TaskKernel"));
                msg.StopFlag = true;
                ASI.Lib.Process.MSQueue.SendMessage(msg);
                ASI.Lib.Log.LogFile.Log(mServer, "KernelService", "送出MSGStopProc給TaskKernel");

                System.Threading.Thread.Sleep(10000);

                if (taskkernel_domain != null)
                    AppDomain.Unload(taskkernel_domain);
                taskkernel_domain = null;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("KernelService", ex.Message);
            }
        }
    }
}
