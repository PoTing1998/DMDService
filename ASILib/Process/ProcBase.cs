using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASI.Lib.Process
{
	public abstract class ProcBase : IProcess
	{
		protected string mComputerName;
		protected string mProcName;
		protected ASI.Lib.Comm.MSMQ.MsgQueLib mRcvQueue;
		protected bool mStopFlag = false;
		protected Thread mTimerThread;

		/// <summary>
		/// 預設為60秒，各Task可另外自行指定
		/// </summary>
		protected int mTimerTick = 60;   // seconds

		protected int ProcHealthTimeOut = 120;
		protected int ProcInitialTimeOut = 240;
		protected DateTime mLastHealth;

		protected ProcBase() { }

		private void ExecTimer(object pNull)
		{
			while (!mStopFlag)
			{
				try
				{
					MSGTimer msg = new MSGTimer(new MSGFrameBase(mProcName, mProcName));
					msg.TimeNow = DateTime.Now;
					SendMessage(msg);
				}
				catch (System.Exception ex)
				{
					ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
				}

				Thread.Sleep((mTimerTick - (DateTime.Now.Second % mTimerTick) + 1) * 1000);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pLabel"></param>
		/// <param name="pBody"></param>
		/// <returns></returns>
		public virtual int ProcEvent(string pLabel, string pBody)
		{
			if (pLabel == MSGTimer.Label)
				return ProcTimerEvent(pBody);
			else if (pLabel == MSGStopProc.Label)
				return ProcStopEvent(pBody);
			else if (pLabel == MSGHealth.Label)
				return ProcHealthEvent(pBody);
			return 0;
		}

		protected virtual int ProcHealthEvent(string pMessage)
		{
			MSGHealth aevent = new MSGHealth(new MSGFrameBase(""));
			if (aevent.UnPack(pMessage) > 0)
			{
				mLastHealth = DateTime.Now;
				return 1;
			}

			return -1;
		}

		/// <summary>
		/// 定時回報TaskMain
		/// </summary>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public virtual int ProcTimerEvent(string pMessage)
		{
			DateTime oNow = DateTime.Now;

			MSGTimer oMSGTimer = new MSGTimer(new MSGFrameBase(""));
			if (oMSGTimer.UnPack(pMessage) > 0)
			{
				MSGHealth oMSGHealth = new MSGHealth(new MSGFrameBase(mProcName, "TaskMain"));
				oMSGHealth.HealthFlag = true;
				SendMessage(oMSGHealth);

				if (oNow.Subtract(oMSGTimer.TimeNow).TotalSeconds > mTimerTick)
				{
					return 0;
				}

				// if taskmain expired, stop this process
				//if (new TimeSpan(atime.Ticks - mLastHealth.Ticks).TotalSeconds > ProcHealthTimeOut)
				//    mStopFlag = true;

				return 1;
			}

			return -1;
		}

		protected virtual int ProcStopEvent(string pMessage)
		{
			MSGStopProc aevent = new MSGStopProc(new MSGFrameBase(""));
			if (aevent.UnPack(pMessage) > 0)
			{
				mStopFlag = aevent.StopFlag;
				return 1;
			}

			return -1;
		}

		/// 
		/// <param name="pProcName"></param>
		public virtual int StartTask(string pComputer, string pProcName)
		{
			if (pComputer != "")
            {
				mComputerName = pComputer;
			}
			else
            {
				mComputerName = Config.ConfigApp.Instance.GetConfigSetting(Config.ConfigApp.HOST_NAME);
            }
			
			mProcName = pProcName;

			//if (utyMsgQueue.IsExist(pComputer + pProcName))

			try
			{
				ASI.Lib.Comm.MSMQ.MsgQueLib.Delete(mComputerName + mProcName);
				ASI.Lib.Comm.MSMQ.MsgQueLib.Create(mComputerName, mComputerName + mProcName);

				mRcvQueue = new ASI.Lib.Comm.MSMQ.MsgQueLib();
				if (mRcvQueue.Open(mComputerName + mProcName, false) <= 0)
				{					
					ASI.Lib.Log.ErrorLog.Log(mProcName, $"開啟MSMQ失敗!名稱:[{mComputerName + mProcName}]");
					return -1;
				}

				mRcvQueue.TimeOutInMilliSeconds = 1;
				string abody;
				while (mRcvQueue.Read(out abody) > 0) ;
				mRcvQueue.TimeOutInMilliSeconds = 10000;

				mTimerThread = new Thread(new ParameterizedThreadStart(this.ExecTimer));
				mTimerThread.IsBackground = true;

				mLastHealth = DateTime.Now;

				mTimerThread.Start("");
				ASI.Lib.Log.LogFile.Log(mComputerName, mProcName, "Start to run");
				return 1;
			}
			catch (System.Exception ex)
			{
				ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
				return -1;
			}
		}

		public virtual void StopTask()
		{
			mStopFlag = true;
			mRcvQueue.Close();
			ASI.Lib.Comm.MSMQ.MsgQueLib.Delete(mComputerName + mProcName);

			try
			{
				mTimerThread.Abort();
			}
			catch { }

			ASI.Lib.Log.LogFile.Log(mComputerName, mProcName, "Stop");
		}

		public void Run()
		{
			string sBody;
			while (!mStopFlag)
			{
				if (mRcvQueue.Read(out sBody) > 0)
				{
					try
					{
						MSGSimple oMSGSimple = new MSGSimple(new MSGFrameBase(""));
						if (oMSGSimple.UnPack(sBody) > 0)
						{
							Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss - ") + mProcName + " : " + oMSGSimple.mLabel);

							if (oMSGSimple.mLabel != MSGTimer.Label &&
								oMSGSimple.mLabel != MSGHealth.Label &&
								!oMSGSimple.Content.Contains("LINKTEST"))
							{
								ASI.Lib.Log.LogFile.Log(mComputerName, mProcName, sBody);
							}

							ProcEvent(oMSGSimple.mLabel, sBody);
						}
					}
					catch (Exception ex)
					{
						ASI.Lib.Log.ErrorLog.Log(mProcName, ex);
					}
				}
			}
		}

		public int SendMessage(MSGPacketBase pMessage)
		{
			return ASI.Lib.Process.ProcMsg.SendMessage(mComputerName + pMessage.Frame.Destination,
										pMessage.Pack(), pMessage.priority);
		}
	}
}
