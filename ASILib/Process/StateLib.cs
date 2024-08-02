using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Process
{
	public abstract class ReceiveStateBase
	{
		public string Server;
		public string Name;
		public int AutoFlag = 1;
		public int InUse = 1;

		private DateTime mLastTime = DateTime.Now;
		public DateTime LastTime
		{
			get { return mLastTime; }
			set { mLastTime = value; }
		}

		protected int mCommHealthTimeOut = 120;
		public int CommHealthTimeOut
		{
			set { mCommHealthTimeOut = value; }
		}

		protected int mCommInitialTimeOut = 180;
		public int CommInitialTimeOut
		{
			set { mCommInitialTimeOut = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pServer">Computer name or Main thread</param>
		/// <param name="pName">Thread name</param>
		public ReceiveStateBase(string pServer, string pName)
		{
			Server = pServer;
			Name = pName;
		}

		/// <summary>
		/// 狀態種類
		/// </summary>
		public enum StateType
		{
			/// <summary>
			/// 0,初始狀態
			/// </summary>
			Initial = 0,

			/// <summary>
			/// 1,正常狀態
			/// </summary>
			Healthy = 1,

			/// <summary>
			/// -1,不正常狀態
			/// </summary>
			NonHealthy = -1,

			/// <summary>
			/// -2,卸載狀態
			/// </summary>
			Unload = -2,

			/// <summary>
			/// -3,停止狀態
			/// </summary>
			Stop = -3
		}

		private StateType mState = StateType.Stop;
		public StateType State
		{
			get { return mState; }
			set
			{
				lock (this)
				{
					if (mState != value &&
						value != StateType.Unload)
					{
						LogProcStatus(value);
					}

					mState = value;
					mLastTime = DateTime.Now;
				}
			}
		}

		/// <summary>
		/// 檢查通訊狀態
		/// </summary>
		/// <returns></returns>
		public virtual int CheckHealthy()
		{
			int iDiffSec = (int)new TimeSpan(DateTime.Now.Ticks - mLastTime.Ticks).TotalSeconds;

			if (State == StateType.Initial &&
				mCommInitialTimeOut > 0 &&
				iDiffSec > mCommInitialTimeOut)
			{
				State = StateType.NonHealthy;
			}
			else if (State == StateType.Healthy &&
					 mCommHealthTimeOut > 0 &&
					 iDiffSec > mCommHealthTimeOut)
			{
				State = StateType.NonHealthy;
			}

			return (int)State;
		}

		/// <summary>
		/// 記錄通訊狀態
		/// </summary>
		/// <param name="pNow"></param>
		/// <returns></returns>
		protected virtual int LogProcStatus(StateType pNow)
		{
			return 1;
		}
	}

	public abstract class SendStateBase
	{
		private string mParameter = "";
		public string Parameter
		{
			get { return mParameter; }
		}

		private DateTime mLastXmt = DateTime.Now.AddDays(-1);
		public DateTime LastXmt
		{
			set { mLastXmt = value; }
			get { return mLastXmt; }
		}

		protected int mXmtTimeOut = 60;
		public int XmtTimeOut
		{
			set { mXmtTimeOut = value; }
		}

		public SendStateBase(string pParameter)  // Parameter : Send Destination
		{
			mParameter = pParameter;
		}

		/// <summary>
		/// 檢查前次傳送時間是否已逾時
		/// </summary>
		/// <returns></returns>
		public virtual int CheckXmtExpire()
		{
			if (mXmtTimeOut > 0 &&
				mXmtTimeOut <= (int)new TimeSpan(DateTime.Now.Ticks - mLastXmt.Ticks).TotalSeconds)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
	}
}
