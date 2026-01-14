using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Security.Policy;
using ASI.Lib.Log;
using ASI.Lib.Process;
using ASI.Lib.Queue;

namespace ASI.Wanda.DMD.TaskKernel
{
    public class ProcKernel : ProcBase
    {
        public class TaskMainInfo : ProcInfo
        {
            /// <summary>
            /// 建構式
            /// </summary>
            /// <param name="pComuterName"></param>
            /// <param name="pProcName"></param>
            /// <param name="pAppName"></param>
            public TaskMainInfo(string pComuterName, string pProcName, string pAppName)
                : base(pComuterName, pProcName, pAppName)
            {
            }

            protected override int LogProcStatus(StateType pNow)
            {
                return 0;
            }
        }

        private ProcInfo taskmain_info;

        public override int ProcEvent(string pLabel, string pBody)
        {
            MSGSimple aevent = new MSGSimple(new MSGFrameBase(""));
            if (aevent.UnPack(pBody) > 0)
            {
                string astr = "Rcving from " + aevent.Frame.Source + " : " + pLabel;
                LogFile.Log(mComputerName, _mProcName, astr);
                LogFile.Display(astr);

                return base.ProcEvent(pLabel, pBody);
            }
            else
            {
                string astr = "Rcving error : " + pBody;
                LogFile.Log(mComputerName, _mProcName, "Rcving error : " + pBody);
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
                if (taskmain_info.Name == aevent.Frame.Source)
                {
                    if (aevent.HealthFlag == true &&
                        (taskmain_info.State == ProcInfo.StateType.Healthy ||
                        taskmain_info.State == ProcInfo.StateType.Initial))
                    {
                        taskmain_info.LastTime = DateTime.Now;  // log the latest time receving the heart-beat
                        taskmain_info.State = ProcInfo.StateType.Healthy;
                    }
                    return 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// handle timer message
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
		public override int ProcTimerEvent(string pMessage)
        {
            DateTime atime = DateTime.Now;

            MSGTimer aevent = new MSGTimer(new MSGFrameBase(""));
            if (aevent.UnPack(pMessage) > 0)
            {
                if (new TimeSpan(atime.Ticks - aevent.TimeNow.Ticks).TotalSeconds > mTimerTick)
                    return 0;

                mLastHealth = DateTime.Now;  // don't cancel, otherwise timer will stop

                if (taskmain_info.Domain != null)
                {
                    MSGHealth ahealth = new MSGHealth(new MSGFrameBase(_mProcName, "TaskMain"));
                    ahealth.HealthFlag = true;
                    MSQueue.SendMessage(ahealth);
                }

                PeriodCheckHealth();

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// periodically check whether process is healthy
        /// </summary>
		private void PeriodCheckHealth()
        {
            if (mStopFlag) return;

            if (taskmain_info.State == ProcInfo.StateType.Stop)
            {
                StartProcess(taskmain_info);
                return;
            }

            if (taskmain_info.CheckHealthy() < 0)
            {
                taskmain_info.State = ProcInfo.StateType.Stop;
                StopProcess(taskmain_info);
                return;
            }
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
                LogFile.Log(mComputerName, _mProcName, "Start Process " + pInfo.Name + " Success");
                return 1;
            }

            LogFile.Log(mComputerName, _mProcName, "Start Process " + pInfo.Name + " Fail");
            return -1;
        }

        /// <summary>
        /// stop a process by unload his application domain
        /// </summary>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        private int StopProcess(ProcInfo pInfo)
        {
            MSGStopProc astop = new MSGStopProc(new MSGFrameBase("TaskKernel", "TaskMain"));
            astop.StopFlag = true;
            SendMessage(astop);

            Thread.Sleep(10000);

            if (ProcessLib.StopProcess(pInfo) > 0)
            {
                LogFile.Log(mComputerName, _mProcName, "Stop Process Success " + pInfo.Name);
                return 1;
            }

            LogFile.Log(mComputerName, _mProcName, "Stop Process Fail " + pInfo.Name);
            return -1;
        }

        public override int StartTask(string pComputer, string pProcName)
        {

            // 在 Debug 模式下等待除錯器附加
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
            if (base.StartTask(pComputer, pProcName) < 0)
                return -1;

            // start all process in-use
            taskmain_info = new ProcInfo(mComputerName, "TaskMain", "TaskMain");
            taskmain_info.ExeName = "TaskMain.exe";
            taskmain_info.InUse = 1;
            taskmain_info.AutoFlag = 1;
            //Thread.Sleep(15000);
            return StartProcess(taskmain_info);
        }

        public override void StopTask()
        {
            base.StopTask();
            StopProcess(taskmain_info);
        }
    }
}
