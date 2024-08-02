using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Messaging;
using System.Net;

namespace ASI.Lib.Queue
{
	/// <summary>
	/// 訊息佇列類別
	/// </summary>
	public class utyMsgQueue : IDisposable
	{
		private static string mHostName = Dns.GetHostName();
		public static string HostName { get { return mHostName; } }
		//		private static string mHostName = ".";
		private MessageQueue mQueue;
		public int TimeOutInMilliSeconds = 10000;

		/// <summary>
		/// 建構式
		/// </summary>
		public utyMsgQueue()
		{
			MessageQueue.EnableConnectionCache = false;
		}

		/// <summary>
		/// 判斷訊息佇列是否存在
		/// </summary>
		/// <param name="pName"></param>
		/// <returns></returns>
		public static bool IsExist(string pName)
		{
			string path = mHostName + "\\Private$\\" + pName;  // private queue
			return MessageQueue.Exists(path);
		}

		/// <summary>
		/// 建立訊息佇列
		/// </summary>
		/// <param name="pName"></param>
		/// <returns></returns>
		public static int Create(string pName)
		{
			Delete(pName);

			string path = mHostName + "\\Private$\\" + pName;  // private queue

			try
			{
				MessageQueue aQueue = MessageQueue.Create(path);
				AccessControlList list = new AccessControlList();
				list.Add(new AccessControlEntry(new Trustee("Everyone"),
								GenericAccessRights.All, StandardAccessRights.All,
								AccessControlEntryType.Allow));
				list.Add(new AccessControlEntry(new Trustee("ANONYMOUS LOGON"),
								GenericAccessRights.All, StandardAccessRights.All,
								AccessControlEntryType.Allow));
				aQueue.SetPermissions(list);

				return 1;
			}
			catch (Exception me)
			{
				Console.WriteLine(pName + " Create : " + me.Message);
			}
			return -1;
		}

		/// <summary>
		/// 開啟訊息佇列
		/// </summary>
		/// <param name="pIP"></param>
		/// <param name="pName"></param>
		/// <param name="pDenySharedRead"></param>
		/// <returns></returns>
		public int Open(string pIP, string pName, bool pDenySharedRead)
		{
			Close();

			try
			{
				string path = "\\Private$\\" + pName;
				if (pIP == mHostName || pIP == ".")
					path = pIP + path;
				else
					path = "FormatName:DIRECT=TCP:" + pIP + path;  // remote private queue

				mQueue = new MessageQueue(path, pDenySharedRead);
				mQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
				return 1;
			}
			catch (Exception ae)
			{
				Console.WriteLine(pName + " Open : " + ae.Message);
				mQueue = null;
			}
			return -1;
		}

		/// <summary>
		/// 開啟訊息佇列
		/// </summary>
		/// <param name="pName"></param>
		/// <param name="pDenySharedRead"></param>
		/// <returns></returns>
		public int Open(string pName, bool pDenySharedRead)
		{
			return Open(mHostName, pName, false);
		}

		/// <summary>
		/// 開啟訊息佇列
		/// </summary>
		/// <param name="pName"></param>
		/// <returns></returns>
		public int Open(string pName)
		{
			return Open(pName, false);
		}

		/// <summary>
		/// 關閉訊息佇列
		/// </summary>
		public void Close()
		{
			try
			{
				mQueue.Close();
				mQueue = null;
			}
			catch { }
		}

		/// <summary>
		/// 移除訊息佇列
		/// </summary>
		/// <param name="pName"></param>
		static public void Delete(string pName)
		{
			string path = mHostName + "\\Private$\\" + pName;  // private queue
			try
			{
				MessageQueue.Delete(path);
			}
			catch (Exception me)
			{
				Console.WriteLine(pName + " Delete : " + me.Message);
			}
		}

		/// <summary>
		/// 使用訊息佇列傳送資料
		/// </summary>
		/// <param name="pBody"></param>
		/// <returns></returns>
		public int Write(string pBody)
		{
			return Write(pBody, MessagePriority.Normal);
		}

		/// <summary>
		/// 使用訊息佇列傳送資料
		/// </summary>
		/// <param name="pBody"></param>
		/// <param name="pPriority"></param>
		/// <returns></returns>
		public int Write(string pBody, MessagePriority pPriority)
		{
			try
			{
				Message amsg = new Message(pBody, new XmlMessageFormatter(new Type[] { typeof(string) }));
				amsg.Priority = pPriority;
				mQueue.Send(amsg, MessageQueueTransactionType.None);
				return 1;
			}
			catch (Exception me)
			{
				Console.WriteLine(mQueue.Path + " Write : " + me.Message);
			}
			return -1;
		}

		/// <summary>
		/// 讀取訊息佇列資料
		/// </summary>
		/// <param name="pBody"></param>
		/// <returns></returns>
		public int Read(out string pBody)
		{
			try
			{
				Message amsg;
				if (TimeOutInMilliSeconds > 0)
					amsg = mQueue.Receive(new TimeSpan(0, 0, 0, 0, TimeOutInMilliSeconds));
				else
					amsg = mQueue.Receive();

				pBody = (string)amsg.Body.ToString();
				return 1;
			}
			catch (Exception) { }

			pBody = "";
			return -1;
		}

		/// <summary>
		/// 取得佇列內訊息數量
		/// </summary>
		/// <returns></returns>
		public int MessageCount()
		{
			int ret = 0;
			MessageEnumerator aenum = mQueue.GetMessageEnumerator2();
			while (aenum.MoveNext()) ret = ret + 1;

			return ret;
		}


		/// <summary>
		/// 開始訊息佇列非同步讀取
		/// </summary>
		/// <returns></returns>
		public int StartAsynReceive()
		{
			try
			{
				mQueue.ReceiveCompleted += new System.Messaging.ReceiveCompletedEventHandler(mQueue_ReceiveCompleted);
				if (TimeOutInMilliSeconds > 0)
					mQueue.BeginReceive(new TimeSpan(0, 0, 0, 0, TimeOutInMilliSeconds));
				else
					mQueue.BeginReceive();
				return 1;
			}
			catch (Exception) { }

			return -1;
		}

		/// <summary>
		/// 停止訊息佇列非同步讀取
		/// </summary>
		/// <param name="pResult"></param>
		/// <returns></returns>
		public Message StopAsynReceive(IAsyncResult pResult)
		{
			try
			{
				mQueue.ReceiveCompleted -= new System.Messaging.ReceiveCompletedEventHandler(mQueue_ReceiveCompleted);
				Message am = mQueue.EndReceive(pResult);
				return am;
			}
			catch (Exception ae)
			{
				Console.WriteLine(" StopAsynReceive : " + ae.Message);
			}

			return null;
		}

		/// <summary>
		/// 訊息佇列完整讀取事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mQueue_ReceiveCompleted(object sender, System.Messaging.ReceiveCompletedEventArgs e)
		{
			Message am = StopAsynReceive(e.AsyncResult);
			if (am != null)
			{
				XmlMessageFormatter afm = new XmlMessageFormatter(new Type[] { typeof(String) });
				OnReceived((string)afm.Read(am));
			}
			StartAsynReceive();
		}

		public delegate void MessageRceiveCompletedEventHandler(object sender, string pBody);
		public event MessageRceiveCompletedEventHandler AsyncReadCompleted;
		protected virtual void OnReceived(string pBody)
		{
			if (AsyncReadCompleted != null)
				AsyncReadCompleted(this, pBody);
		}

#pragma warning disable CS0414 // 已指派欄位 'utyMsgQueue.mDisposed'，但從未使用過其值。
		private bool mDisposed = false;
#pragma warning restore CS0414 // 已指派欄位 'utyMsgQueue.mDisposed'，但從未使用過其值。

		/// <summary>
		/// 釋放物件
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool pDisposing)
		{
			if (pDisposing)
			{
				Close();
			}
			mDisposed = true;
		}

		~utyMsgQueue()
		{
			Dispose(false);
		}
	}

}
