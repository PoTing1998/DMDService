using System;
using System.Threading;
using System.Collections.Generic;
using System.Security.Policy;
using System.Configuration;
using System.Text;
using System.Net.NetworkInformation;
using ASI.Lib.DB;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Lib.Queue;
using ASI.Lib.Utility;
using ASI.Lib.Config;

namespace ASI.Wanda.DCU.TaskMain
{
    public class ProcMain : ProcBase
    {
        public override int ProcEvent(string pLabel, string pBody)
        {
            MSGSimple aevent = new MSGSimple(new MSGFrameBase(""));
            if (aevent.UnPack(pBody) > 0)
            {
                string astr = "Rcving from " + aevent.Frame.Source + " : " + pLabel;
                LogFile.Log(mComputerName, mProcName, astr);
                LogFile.Display(astr);

                return base.ProcEvent(pLabel, pBody);
            }
            else
            {
                string astr = "Rcving error : " + pBody;
                LogFile.Log(mComputerName, mProcName, "Rcving error : " + pBody);
                LogFile.Display(astr);

                return -1;
            }
        }

        /// <summary>
        /// handle healthy message from other process
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
		protected override int ProcHealthEvent(string pMessage)
        {
            MSGHealth aevent = new MSGHealth(new MSGFrameBase(""));
            if (aevent.UnPack(pMessage) > 0)
            {
                if (aevent.Frame.Source == "TaskKernel")
                {
                    mLastHealth = DateTime.Now;
                    return 1;
                }

                foreach (ProcInfo ainfo in mProcInfo)
                {
                    if (ainfo.Name == aevent.Frame.Source)
                    {
                        if (aevent.HealthFlag == true &&
                            (ainfo.State == ProcInfo.StateType.Healthy ||
                            ainfo.State == ProcInfo.StateType.Initial))
                        {
                            ainfo.LastTime = DateTime.Now;  // log the latest time receving the heart-beat
                            ainfo.State = ProcInfo.StateType.Healthy;
                        }
                        return 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// periodically check whether process is healthy
        /// </summary>
		private void PeriodCheckHealth()
        {
            DateTime atime = DateTime.Now;
            foreach (ProcInfo ainfo in mProcInfo)
            {
                if (ainfo.CheckHealthy() > 0)
                {
                    MSGHealth ahealth = new MSGHealth(new MSGFrameBase("TaskMain", ainfo.Name));
                    ahealth.HealthFlag = true;
                    MSQueue.SendMessage(ahealth);
                }
            }

            // processing kernel healthy
            //if (new TimeSpan(atime.Ticks - mLastHealth.Ticks).TotalSeconds > ProcHealthTimeOut)
            //    base.mStopFlag = true;

            MSGHealth khealth = new MSGHealth(new MSGFrameBase("TaskMain", "TaskKernel"));
            khealth.HealthFlag = true;
            MSQueue.SendMessage(khealth);
        }

        /// <summary>
        /// handle timer message
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
		public override int ProcTimerEvent(string pMessage)
        {
            MSGTimer aevent = new MSGTimer(new MSGFrameBase(""));
            if (aevent.UnPack(pMessage) > 0)
            {
                CheckProcInfo();
                PeriodCheckHealth();
                if (aevent.TimeNow.Minute == 22)
                    LogFile.DelLogFile(aevent.TimeNow, 92);  // clear log file

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// collection of process's state
        /// </summary>
		private List<ProcInfo> mProcInfo = new List<ProcInfo>();
        //private List<ProcInfo> ReadProcInfo()
        //{
        //    List<ProcInfo> proc_infos = new List<ProcInfo>();
        //    DataBase adb = new DataBase(null);
        //    try
        //    {
        //        adb = ProjectDB.Connect(ConfigApp.Instance.ServerDB);
        //        tbProcConf atable = new tbProcConf(adb.Conn);
        //        atable.SelectAll();
        //        foreach (tbProcConf.Row arow in atable.records)
        //        {
        //            ProcInfo ainfo = new ProcInfo(mComputerName, mProcName, arow.ProcID);
        //            ainfo.InUse = arow.InUse;
        //            ainfo.AutoFlag = arow.AutoAlive;
        //            ainfo.ExeName = arow.ExeName;
        //            proc_infos.Add(ainfo);
        //        }
        //    }
        //    finally { adb.Close(); }

        //    return proc_infos;
        //}

        /// <summary>
        /// 讀取config.xml裡各個process資訊並存放在ProcInfo物件
        /// </summary>
        /// <returns></returns>
        private List<ProcInfo> ReadProcInfo()
        {
            List<ProcInfo> proc_infos = new List<ProcInfo>();
            List<string> procs = ConfigApp.Instance.GetConfigSettings("Process");
            foreach (string strproc in procs)
            {
                LogFile.Log(mComputerName, mProcName, "Get Process in config = " + strproc);
                //string[] args = strproc.Split(new char[] { ';' });
                //if (args.Length != 2) continue;

                ProcInfo ainfo = new ProcInfo(mComputerName, mProcName, strproc);
                ainfo.InUse = 1;
                ainfo.AutoFlag = 1;
                ainfo.ExeName = strproc;
                proc_infos.Add(ainfo);
            }

            return proc_infos;
        }

        /// <summary>
        /// use a new thread to start a process
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
		private int StartProcess(ProcInfo pInfo)
        {
            if (ProcessLib.StartProcess(pInfo) > 0)
            {
                LogFile.Log(mComputerName, mProcName, "Start Process " + pInfo.Name + " Success");
                System.Threading.Thread.Sleep(1000);
                return 1;
            }

            LogFile.Log(mComputerName, mProcName, "Start Process " + pInfo.Name + " Fail");

            return -1;
        }

        /// <summary>
        /// stop a process by unload his application domain
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
		private int StopProcess(ProcInfo pInfo)
        {
            if (ProcessLib.StopProcess(pInfo) > 0)
            {
                LogFile.Log(mComputerName, mProcName, "Stop Process Success " + pInfo.Name);
                return 1;
            }

            LogFile.Log(mComputerName, mProcName, "Stop Process Fail " + pInfo.Name);
            return -1;
        }

        /// <summary>
        /// 檢查process的狀態
        /// </summary>
        /// <returns></returns>
		private int CheckProcInfo()
        {
            if (mStopFlag) return 0;

            foreach (ProcInfo ainfo in mProcInfo)
            {
                if (ainfo.State == ProcInfo.StateType.Healthy ||
                    ainfo.State == ProcInfo.StateType.Initial)  // healthy process
                {
                    if (ainfo.InUse < 1)
                        ainfo.State = ProcInfo.StateType.NonHealthy;
                }

                if (ainfo.State == ProcInfo.StateType.NonHealthy)  // stop process
                {
                    //ainfo.State = ProcInfo.StateType.Unload;
                    StopProcess(ainfo);
                    ainfo.State = ProcInfo.StateType.Stop;
                }
                else if (ainfo.State == ProcInfo.StateType.Unload)
                {
                }
                else if (ainfo.State == ProcInfo.StateType.Stop)
                {
                    if (ainfo.InUse > 0 && ainfo.AutoFlag > 0)   // restart process
                    {
                        StartProcess(ainfo);
                    }
                }
            }

            return mProcInfo.Count;
        }

        public override int StartTask(string pComputer, string pProcName)
        {
            int iReturn = base.StartTask(pComputer, pProcName);
            if (iReturn > 0)
            {
                mProcInfo = ReadProcInfo();
                if (mProcInfo.Count > 0)
                    return 1;
            }

            return -1;
        }

        public override void StopTask()
        {
            base.StopTask();

            foreach (ProcInfo ainfo in mProcInfo)   // stop all process
            {
                MSGStopProc astop = new MSGStopProc(new MSGFrameBase(mProcName, ainfo.Name));
                astop.StopFlag = true;
                MSQueue.SendMessage(astop);
            }

            Thread.Sleep(5000);
            foreach (ProcInfo ainfo in mProcInfo)   // stop all process
                StopProcess(ainfo);
        }
    }
}
