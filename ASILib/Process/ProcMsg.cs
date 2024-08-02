using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ASI.Lib.Process
{
	/// <summary>
	/// Proccess之間傳遞的訊息，以MSMQ為通訊介面
	/// </summary>
    public class ProcMsg
    {
        public static int SendMessage(string pIP, string pQueue, string pBody, System.Messaging.MessagePriority pPriority)
        {
			ASI.Lib.Comm.MSMQ.MsgQueLib oQueue = new ASI.Lib.Comm.MSMQ.MsgQueLib();

            try
            { 
                if (oQueue.Open(pIP, pQueue, false) > 0)
                {
                    return oQueue.Write(pBody, pPriority);
                }
            }
            catch (System.Exception ex)
            {
				ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Process.ProcMsg", ex);
			}
            finally
            {
                if (oQueue != null)
                {
                    oQueue.Dispose();
                }
            }

            return -1;
        }

        public static int SendMessage(string pQueue, string pBody, System.Messaging.MessagePriority pPriority)
        {
            string[] sSplits = MSGFrameBase.DeIPQueue(pQueue);
            if (sSplits == null)
            {
                return SendMessage(".", pQueue, pBody, pPriority);
            }
            else
            {
                return SendMessage(sSplits[1], sSplits[0], pBody, pPriority);
            }
        }

        public static int SendMessage(string pQueue, string pBody)
        {
            return SendMessage(pQueue, pBody, System.Messaging.MessagePriority.Normal);
        }

        public static int SendMessage(MSGPacketBase pMessage)
        {
            return SendMessage(pMessage.Frame.Destination, pMessage.Pack(), pMessage.priority);
        }
    }

	/// <summary>
	///  Proccess之間傳遞的訊息物件，訊息內容的格式為：[source]:[destination];[message body]
	/// </summary>
	public class MSGFrameBase  
	{
		/// <summary>
		/// 訊息傳送方(ProcName)，如TaskMain、TaskHMI... 
		/// </summary>
		public string Source = ""; 

		/// <summary>
		/// 訊息接收方(ProcName)，如TaskMain、TaskHMI...
		/// </summary>
		public string Destination = "";

		public MSGFrameBase(string pDest)
		{
			Source = System.Environment.MachineName;
			Destination = pDest;
		}

		public MSGFrameBase(string pSource, string pDest) 
		{
			Source = pSource; 
			Destination = pDest;
		}

		public string PackFrame(string pData)
		{
			string amsg = Source + ":" + Destination;
			return amsg + ";" + pData;
		}
		
		public string UnPackFrame(string pFrame)
		{
			string[] str1 = pFrame.Split(new char[] { ';' }, 2);

			if (str1.Length == 2)
			{
				string[] str2 = str1[0].Split(new char[] { ':' }, 2);
				if (str2.Length == 2)
				{
					Source = str2[0];
					Destination = str2[1];
					return str1[1];
				}
			}
			return null;
		}

		public static string[] DeIPQueue(string pIPQueue)
		{
			string[] sSplits = pIPQueue.Split(new char[] { '@' }, 2);
			if (sSplits.Length == 2)
			{
				return sSplits;
			}
			else
			{
				return null;
			}
		}

		public static string ENIPQueue(string pIP, string pQueue)
		{
			return pQueue + "@" + pIP;
		}
	}

	public abstract class MSGPacketBase
	{
		public DateTime SendTime = DateTime.Now;
		public string mLabel;
		public System.Messaging.MessagePriority priority = System.Messaging.MessagePriority.Normal;

		public MSGFrameBase Frame;
		public MSGPacketBase(MSGFrameBase pFrmae)
		{
			Frame = pFrmae;
		}

		protected string PackPacket(string pData)
		{
			return Frame.PackFrame(mLabel + ";" + SendTime.ToString("yyyyMMddHHmmss.fff")
									+ ";" + pData);
		}

		protected string UnPackPacket(string pFrame)
		{
			string rets = Frame.UnPackFrame(pFrame);
			if (rets != null)
			{
				string[] sSplits = rets.Split(new char[] { ';' }, 3);
				if (sSplits.Length == 3)
				{
					mLabel = sSplits[0];
					SendTime = ASI.Lib.Utility.TimeLib.MakeTime(sSplits[1]);
					return sSplits[2];
				}
			}
			return null;
		}

		public abstract string Pack();
	}

	public class MSGSimple : MSGPacketBase
	{
		public const string Label = "MSG_SIMPLE";
		public string Content = "";

		public MSGSimple(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
		}

		public override string Pack()
		{
			return base.PackPacket(Content);
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				Content = ret;
				return 1;
			}

			return -1;
		}
	}

	public class MSGReqTime : MSGSimple
	{
		public new const string Label = "MSG_REQTIME";
		public MSGReqTime(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Low;
		}
	}

	public class MSGStart : MSGSimple
	{
		public new const string Label = "MSG_START";
		public MSGStart(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.VeryHigh;
		}
	}

	public class MSGTimer : MSGPacketBase
	{
		public const string Label = "MSG_TIMER";
		public DateTime TimeNow = DateTime.Now;

		public MSGTimer(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = "MSG_TIMER";
			priority = System.Messaging.MessagePriority.High;
		}

		public override string Pack()
		{
			return base.PackPacket(TimeNow.ToString("yyyyMMddHHmmss.fff"));
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				TimeNow = ASI.Lib.Utility.TimeLib.MakeTime(ret);
				return 1;
			}

			return -1;
		}
	}

	public class MSGTimeSync : MSGTimer
	{
		public new const string Label = "MSG_TIMESYNC";
		public MSGTimeSync(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.VeryHigh;
		}
	}

	public class MSGStopProc : MSGPacketBase
	{
		public const string Label = "MSG_STOPPROC";
		public bool StopFlag = false;

		public MSGStopProc(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = "MSG_STOPPROC";
			priority = System.Messaging.MessagePriority.Highest;
		}

		public override string Pack()
		{
			string body = "N";
			if (StopFlag == true) body = "Y";

			return base.PackPacket(body);
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				if (ret == "Y" || ret == "y")
					StopFlag = true;
				else
					StopFlag = false;
				return 1;
			}

			return -1;
		}
	}

	public class MSGFinish : MSGPacketBase
	{
		public const string Label = "MSG_FINISH";
		public const int TYPE_AUTO = 1;
		public const int TYPE_MANU = 0;

		public int Type = TYPE_AUTO;
		public int Count = 0;

		public MSGFinish(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = "MSG_FINISH";
			priority = System.Messaging.MessagePriority.Normal;
		}

		/// <summary>
		/// 組成訊息格式([Source]:[Destination];[Label;yyyyMMddHHmmss.fff;Type])
		/// </summary>
		/// <returns></returns>
		public override string Pack()
		{
			return base.PackPacket(Count.ToString());
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				Count = int.Parse(ret);
				return 1;
			}

			return -1;
		}
	}

	public class MSGEquipStatus : MSGPacketBase
	{
		public const string Label = "MSG_EQUIP_STATUS";
		/// <summary>
		/// 來源系統別 PA、DMD、CCTV...
		/// </summary>
		public string SourceSystem = "";

		/// <summary>
		/// sys_equip_status資料表equip_id欄位
		/// </summary>
		public string EquipID = "";

		/// <summary>
		/// sys_equip_alarm_match資料表status_type欄位
		/// </summary>
		public string StatusType = "";

		/// <summary>
		/// sys_equip_alarm_match資料表status_value欄位
		/// </summary>
		public string StatusValue = "";

		public MSGEquipStatus(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.High;
		}

		public override string Pack()
		{
			return base.PackPacket($"{SourceSystem}|{EquipID}|{StatusType}|{StatusValue}");
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				string[] sSplits = ret.Split(new char[] { '|' }, 4);
				if (sSplits.Length == 4)
				{
					SourceSystem = sSplits[0];
					EquipID = sSplits[1];
					StatusType = sSplits[2];
					StatusValue = sSplits[3];
				}

				return 1;
			}

			return -1;
		}
	}

	public class MSGHealth : MSGPacketBase
	{
		public const string Label = "MSG_HEALTH";
		public bool HealthFlag = true;

		public MSGHealth(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.VeryHigh;
		}

		public override string Pack()
		{
			string body = "Y";
			if (HealthFlag == false) body = "N";

			return base.PackPacket(body);
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				if (ret == "Y" || ret == "y")
					HealthFlag = true;
				else
					HealthFlag = false;
				return 1;
			}

			return -1;
		}
	}

	public class MSGCommError : MSGHealth
	{
		public new const string Label = "MSG_COMMERR";
		public MSGCommError(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}
	}

	public class MSGStopChild : MSGSimple
	{
		public new const string Label = "MSG_STOP_CHILD";
		public MSGStopChild(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.VeryHigh;
		}
	}

	public class MSGTransfer : MSGSimple
	{
		public new const string Label = "MSG_TRANSFER";
		public byte[] ByteContent = null;

		public MSGTransfer(MSGFrameBase pFrame)
			: base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}

		public override string Pack()
		{
			if (ByteContent != null)
			{
				Content = ASI.Lib.Text.Parsing.String.BytesToHexString(ByteContent);
			}

			return base.PackPacket(Content);
		}

		public new int UnPack(string pFrame)
		{
			Content = base.UnPackPacket(pFrame);

			if (Content != null &&
				Content != "")
			{
				ByteContent = ASI.Lib.Text.Parsing.String.HexStringToBytes(Content, 2);
			}

			return 1;
		}
	}

	/// <summary>
	/// 批次作業訊息
	/// </summary>
	public class MSGBatch : MSGPacketBase
	{
		public const string CMD_UPDDATA = "UPDDATA";

		public const string Label = "MSG_BATCH";
		public string Command = "None";

		public MSGBatch(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.VeryLow;
		}

		/// <summary>
		/// 組成訊息格式([Source]:[Destination];[Label;yyyyMMddHHmmss.fff;Command])
		/// </summary>
		/// <returns></returns>
		public override string Pack()
		{
			return base.PackPacket(Command);
		}

		/// <summary>
		/// 解出資料內容(Command)
		/// </summary>
		/// <param name="pFrame"></param>
		/// <returns></returns>
		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			Command = ret;
			return 1;
		}
	}

}
