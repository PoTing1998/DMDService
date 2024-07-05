using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Lib.Process;

namespace ASI.Wanda.CMFT.ProcMsg
{
	/// <summary>
	/// Server內部傳送訊息物件，從TaskHMI發出
	/// </summary>
	public class MSGFromTaskHMI : MSGPacketBase
	{
		public const string Label = "MSG_FROM_TASK_HMI";

		/// <summary>
		/// 訊息類別碼 分別為Ack = 1；Change/Command = 2；Response = 3
		/// </summary>
		public int MessageType { set; get; }

		/// <summary>
		/// 訊息識別碼 1~65535，若為0則由接收方自行編碼
		/// </summary>
		public int MessageID { set; get; }

		/// <summary>
		/// 訊息內容(Json格式)
		/// </summary>
		public string JsonData { set; get; }

		public MSGFromTaskHMI(MSGFrameBase pFrame): base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}

		public override string Pack()
		{
			return base.PackPacket($"{MessageType}##{MessageID}##{JsonData}");
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				string[] arrayStr = ASI.Lib.Text.Parsing.String.Split(ret, "##");
				if (arrayStr != null && arrayStr.Length == 3)
				{
					MessageType = int.Parse(arrayStr[0]);
					MessageID = int.Parse(arrayStr[1]);
					JsonData = arrayStr[2];
					return 1;
				}

				return -1;
			}

			return -1;
		}
	}

	/// <summary> 
	/// Server內部傳送訊息物件，從TaskPA發出
	/// </summary>
	public class MSGFromTaskPA : MSGPacketBase
	{
		public const string Label = "MSG_FROM_TASK_PA";

		/// <summary>
		/// 訊息類別碼 分別為Ack = 1；Change/Command = 2；Response = 3
		/// </summary>
		public int MessageType { set; get; }

		/// <summary>
		/// 訊息識別碼 1~65535，若為0則由接收方自行編碼 
		/// </summary>
		public int MessageID { set; get; } 

		/// <summary>
		/// 訊息內容(Json格式)
		/// </summary>
		public string JsonData { set; get; }

		public MSGFromTaskPA(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}

		public override string Pack()
		{
			return base.PackPacket($"{MessageType}##{MessageID}##{JsonData}");
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				string[] arrayStr = ASI.Lib.Text.Parsing.String.Split(ret, "##");
				if (arrayStr != null && arrayStr.Length == 3)
				{
					MessageType = int.Parse(arrayStr[0]);
					MessageID = int.Parse(arrayStr[1]);
					JsonData = arrayStr[2];
					return 1;
				}

				return -1;
			}

			return -1;
		}
	}

	/// <summary>
	/// Server內部傳送訊息物件，從TaskDMD發出
	/// </summary>
	public class MSGFromTaskDMD : MSGPacketBase
	{
		public const string Label = "MSG_FROM_TASK_DMD";

		/// <summary>
		/// 訊息類別碼 分別為Ack = 1；Change/Command = 2；Response = 3
		/// </summary>
		public int MessageType { set; get; }

		/// <summary>
		/// 訊息識別碼 1~65535，若為0則由接收方自行編碼
		/// </summary>
		public int MessageID { set; get; }

		/// <summary>
		/// 訊息內容(Json格式)
		/// </summary>
		public string JsonData { set; get; }

		public MSGFromTaskDMD(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}

		public override string Pack()
		{
			return base.PackPacket($"{MessageType}##{MessageID}##{JsonData}");
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				string[] arrayStr = ASI.Lib.Text.Parsing.String.Split(ret, "##");
				if (arrayStr != null && arrayStr.Length == 3)
				{
					MessageType = int.Parse(arrayStr[0]);
					MessageID = int.Parse(arrayStr[1]);
					JsonData = arrayStr[2];
					return 1;
				}

				return -1;
			}

			return -1;
		}
	}

	/// <summary>
	/// Server內部傳送訊息物件，從TaskTETRA發出
	/// </summary>
	public class MSGFromTaskTETRA : MSGPacketBase
	{
		public const string Label = "MSG_FROM_TASK_TETRA";

		/// <summary>
		/// 訊息類別碼 分別為Ack = 1；Change/Command = 2；Response = 3
		/// </summary>
		public int MessageType { set; get; }

		/// <summary>
		/// 訊息識別碼 1~65535，若為0則由接收方自行編碼
		/// </summary>
		public int MessageID { set; get; }

		/// <summary>
		/// 訊息內容(Json格式)
		/// </summary>
		public string JsonData { set; get; }

		public MSGFromTaskTETRA(MSGFrameBase pFrame) : base(pFrame)
		{
			mLabel = Label;
			priority = System.Messaging.MessagePriority.Normal;
		}

		public override string Pack()
		{
			return base.PackPacket($"{MessageType}##{MessageID}##{JsonData}");
		}

		public int UnPack(string pFrame)
		{
			string ret = base.UnPackPacket(pFrame);
			if (ret != null)
			{
				string[] arrayStr = ASI.Lib.Text.Parsing.String.Split(ret, "##");
				if (arrayStr != null && arrayStr.Length == 3)
				{
					MessageType = int.Parse(arrayStr[0]);
					MessageID = int.Parse(arrayStr[1]);
					JsonData = arrayStr[2];
					return 1;
				}

				return -1;
			}

			return -1;
		}
	}
}
