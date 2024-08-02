using ASI.Lib.Config;
using ASI.Lib.Queue;
using System;
using System.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Process
{
	/// <summary>
	/// 程序狀態類別
	/// </summary>
	public class ProcInfo : ReceiveStateBase
	{
		public System.Diagnostics.Process Domain;
		public string ExeName;
		private string mComputer = "";

		/// <summary>
		/// 建構式
		/// </summary>
		/// <param name="pComuterName"></param>
		/// <param name="pProcName"></param>
		/// <param name="pAppName"></param>
		public ProcInfo(string pComuterName, string pProcName, string pAppName)
			: base(pProcName, pAppName)
		{
			CommInitialTimeOut = 180;
			CommHealthTimeOut = 120;
			mComputer = pComuterName;
		}

		private DateTime mLastHeathTime = DateTime.Now.AddDays(-1);

		/// <summary>
		/// 記錄程序狀態
		/// </summary>
		/// <param name="pNow"></param>
		/// <returns></returns>
		protected override int LogProcStatus(StateType pNow)
		{
			if (pNow == StateType.Healthy
				&& DateTime.Now.Subtract(mLastHeathTime).TotalMinutes > 2)
			{
				mLastHeathTime = DateTime.Now;
				return 1;
			}
			else if (pNow == StateType.NonHealthy)
			{
				return -1;
			}

			return 0;
		}
	}

	/// <summary>
	/// 程序程式館類別
	/// </summary>
	public class ProcessLib
	{
		/// <summary>
		/// 啟動另一程序
		/// </summary>
		/// <param name="pInfo"></param>
		/// <returns></returns>
		public static int StartProcess(ProcInfo pInfo)  // use a new thread to start a process
		{
			try
			{
				KillProcess(pInfo.Name);

				pInfo.Domain = System.Diagnostics.Process.Start(pInfo.ExeName, pInfo.Name);
				if (pInfo.Domain != null)
				{
					pInfo.State = ProcInfo.StateType.Initial;
					pInfo.LastTime = DateTime.Now;
					return 1;
				}
			}
			catch { }

			return -1;
		}

		/// <summary>
		/// 停止另一程序
		/// </summary>
		/// <param name="pInfo"></param>
		/// <returns></returns>
		public static int StopProcess(ProcInfo pInfo)  // stop a process by unload his application domain
		{
			try
			{
				if (pInfo.Domain != null)
				{
					KillProcess(pInfo.Name);
					pInfo.Domain = null;
				}

				return 1;
			}
			catch
			{
				return -1;
			}
		}

		/// <summary>
		/// 刪除工作管理員中所有指定名稱的process
		/// </summary>
		/// <param name="pProcName"></param>
		private static void KillProcess(string pProcName)
		{
			System.Diagnostics.Process[] oProcessArray = System.Diagnostics.Process.GetProcessesByName(pProcName);
			if (oProcessArray != null)
			{
				foreach (System.Diagnostics.Process oProcess in oProcessArray)
				{
					try
					{
						oProcess.Kill();
						oProcess.WaitForExit(1000);
					}
					catch 
					{
						
					}
				}
			}
		}
	}

	/// <summary>
	/// 微軟Message Queue操作類別
	/// </summary>
	public class MSQueue
	{
		private static string QueuePrfix = ConfigApp.Instance.HostName;

		/// <summary>
		/// 傳送訊息給指定的Message Queue
		/// </summary>
		/// <param name="pIP"></param>
		/// <param name="pQueue"></param>
		/// <param name="pBody"></param>
		/// <param name="pPriority"></param>
		/// <returns></returns>
		public static int SendMessage(string pIP, string pQueue, string pBody, MessagePriority pPriority)
		{
			utyMsgQueue aqueue = new utyMsgQueue();
			if (aqueue.Open(pIP, pQueue, false) > 0)
				return aqueue.Write(pBody, pPriority);

			return -1;
		}

		/// <summary>
		/// 傳送訊息給指定的Message Queue
		/// </summary>
		/// <param name="pQueue"></param>
		/// <param name="pBody"></param>
		/// <param name="pPriority"></param>
		/// <returns></returns>
		public static int SendMessage(string pQueue, string pBody, MessagePriority pPriority)
		{
			string[] strs = MSGFrameBase.DeIPQueue(pQueue);
			if (strs == null)
				return SendMessage(utyMsgQueue.HostName, QueuePrfix + pQueue, pBody, pPriority);
			else
				return SendMessage(strs[1], strs[0], pBody, pPriority);
		}

		/// <summary>
		/// 傳送訊息給指定的Message Queue
		/// </summary>
		/// <param name="pQueue"></param>
		/// <param name="pBody"></param>
		/// <returns></returns>
		public static int SendMessage(string pQueue, string pBody)
		{
			return SendMessage(pQueue, pBody, MessagePriority.Normal);
		}

		/// <summary>
		/// 傳送訊息給指定的Message Queue
		/// </summary>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public static int SendMessage(MSGPacketBase pMessage)
		{
			return SendMessage(pMessage.Frame.Destination, pMessage.Pack(), pMessage.priority);
		}
	}
}
