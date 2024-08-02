using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using SnmpSharpNet;

namespace ASI.Lib.Comm.SNMP
{
    public class SnmpLib
    {
		private bool mThreadRun = true;

		private System.Threading.Thread mThreadRecieveTrap = null;

		System.Net.Sockets.Socket mSocket = null;

		private int mPort = 162;

		public delegate void ReceivedTrapEventHandler(SnmpTrapObject snmpTrapObject);

		/// <summary>
		/// 接收Trap的事件
		/// </summary>
		public event ReceivedTrapEventHandler ReceivedTrapEvent;

		public SnmpLib()
        {
			
		}

		/// <summary>
		/// 開始接收SNMP的trap訊息
		/// </summary>
		/// <param name="port">指定port，SNMP預設使用Port 162來作為Manager接收Agent所傳來的Trap訊息的Port</param>
		public void StartRecieveTrap(int port)
		{
			mPort = port;
			mThreadRun = true;
			mThreadRecieveTrap = new System.Threading.Thread(RecieveTrap);
			mThreadRecieveTrap.Start();
		}

		private void RecieveTrap()
        {
			mSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			IPEndPoint ipep = new IPEndPoint(IPAddress.Any, mPort);
			EndPoint ep = (EndPoint)ipep;
			mSocket.Bind(ep);
			mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 0);

			int inlen = -1;
			while (mThreadRun)
			{
				byte[] byteData = new byte[16 * 1024];
				IPEndPoint peer = new IPEndPoint(IPAddress.Any, 0);
				EndPoint endPoint = (EndPoint)peer;

				try
				{
					inlen = mSocket.ReceiveFrom(byteData, ref endPoint);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception {0}", ex.Message);
					inlen = -1;
				}

				if (inlen > 0)
				{
					int ver = SnmpPacket.GetProtocolVersion(byteData, inlen);
					if (ver == (int)SnmpVersion.Ver1) //snmp版本1
					{
						#region Parse SNMP Version 1 TRAP packet 

						SnmpV1TrapPacket pkt = new SnmpV1TrapPacket();
						pkt.decode(byteData, inlen);
						Console.WriteLine("** SNMP Version 1 TRAP received from {0}:", endPoint.ToString());
						Console.WriteLine("*** Trap generic: {0}", pkt.Pdu.Generic); //snmp的pdu型別,在0-6之間的值表示:標準的trap型別的一種,如果這個值是6表示是廠家自定義trap型別,這時可以通過Pdu.Specific來判斷是廠家自定義的哪種trap型別
						Console.WriteLine("*** Trap specific: {0}", pkt.Pdu.Specific); //廠家自定義trap
						Console.WriteLine("*** Agent address: {0}", pkt.Pdu.AgentAddress.ToString());
						Console.WriteLine("*** Timestamp: {0}", pkt.Pdu.TimeStamp.ToString());
						Console.WriteLine("*** VarBind count: {0}", pkt.Pdu.VbList.Count);
						Console.WriteLine("*** VarBind content:");
						foreach (Vb v in pkt.Pdu.VbList)
						{  //遍歷輸出snmp的pdu變數
							Console.WriteLine("**** {0} {1}: {2}", v.Oid.ToString(), SnmpConstants.GetTypeName(v.Value.Type), v.Value.ToString());
						}
						Console.WriteLine("** End of SNMP Version 1 TRAP data.");

						#endregion
					}
					else
					{
						//解析snmp版本2資料包
						SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
						snmpV2Packet.decode(byteData, inlen);

						SnmpTrapObject snmpTrapObject = new SnmpTrapObject();

						snmpTrapObject.SourceIP = endPoint.ToString().Trim();
						Console.WriteLine("** SNMP Version 2 TRAP received from {0}:", endPoint.ToString());

						snmpTrapObject.ByteData = byteData;

						snmpTrapObject.PduType = snmpV2Packet.Pdu.Type;
						if ((SnmpSharpNet.PduType)snmpV2Packet.Pdu.Type != PduType.V2Trap)
						{
							Console.WriteLine("*** NOT an SNMPv2 trap ****");
						}
						else
						{
							snmpTrapObject.Community = snmpV2Packet.Community.ToString().Trim();
							Console.WriteLine("*** Community: {0}", snmpV2Packet.Community.ToString());

							snmpTrapObject.TrapOID = snmpV2Packet.Pdu.TrapObjectID.ToString().Trim();
							Console.WriteLine("*** TrapOID: {0}", snmpV2Packet.Pdu.TrapObjectID.ToString()); 
							Console.WriteLine("*** VarBind count: {0}", snmpV2Packet.Pdu.VbList.Count);
							Console.WriteLine("*** VarBind content:");
							
							foreach (Vb v in snmpV2Packet.Pdu.VbList)
							{
								snmpTrapObject.DicVarbinds.Add(v.Oid.ToString().Trim(), v.Value.ToString().Trim());
								Console.WriteLine("**** {0} {1}: {2}",
								   v.Oid.ToString(), SnmpConstants.GetTypeName(v.Value.Type), v.Value.ToString());
							}
							
                            Console.WriteLine("** End of SNMP Version 2 TRAP data.");

							//觸發接收Trap的事件
							ReceivedTrapEvent?.Invoke(snmpTrapObject);
						}
					}
				}
				else
				{
					if (inlen == 0)
						Console.WriteLine("Zero length packet received.");
				}

				System.Threading.Thread.Sleep(1);
			}
		}

		/// <summary>
		/// SNMP元素查詢
		/// </summary>
		public static string Query(SnmpSharpNet.SnmpVersion snmpVersion, string agentIP,int port,string oid)
		{
			string sReturn = "";

			// SNMP community name
			OctetString community = new OctetString("public");

			// Define agent parameters class
			AgentParameters param = new AgentParameters(community);

			// Set SNMP version to 1 (or 2)
			param.Version = snmpVersion;

			// Construct the agent address object
			// IpAddress class is easy to use here because it will try to resolve constructor parameter if it doesn't parse to an IP address
			IpAddress agent = new IpAddress(agentIP);

			// Construct target，SNMP預設會使用Port 161作為Manager傳送與接收Agent請求訊息的Port
			UdpTarget target = new UdpTarget((IPAddress)agent, port, 2000, 1);

			// Pdu class used for all requests
			Pdu pdu = new Pdu(PduType.Get);
			pdu.VbList.Add(oid); //udpindatagrams

			// Make SNMP request
			SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);
			
			// If result is null then agent didn‘t reply or we couldn‘t parse the reply.
			if (result != null)
			{
				// ErrorStatus other then 0 is an error returned by the Agent - see SnmpConstants for error definitions
				if (result.Pdu.ErrorStatus != 0)
				{
					// agent reported an error with the request                    
					sReturn = $"Error in SNMP reply. Error {result.Pdu.ErrorStatus} index {result.Pdu.ErrorIndex}";
				}
				else
				{
					// Reply variables are returned in the same order as they were added
					//  to the VbList
					sReturn = $"sysDescr({result.Pdu.VbList[0].Oid.ToString()}) ({SnmpConstants.GetTypeName(result.Pdu.VbList[0].Value.Type)}): {result.Pdu.VbList[0].Value.ToString()}";
				}
			}
			else
			{
				sReturn = $"No response received from SNMP agent.";
			}
			target.Close();

			Console.WriteLine(sReturn);
			return sReturn;
		}

		public void Dispose()
        {
			mThreadRun = false;
			System.Threading.Thread.Sleep(100);

			if (mSocket != null)
            {
				mSocket.Close();
				mSocket.Dispose();
				mSocket = null;
			}
		}
	}

	/// <summary>
	/// SNMP接收到的trap訊息物件
	/// </summary>
	public class SnmpTrapObject
    {
		/// <summary>
		/// 來源IP
		/// </summary>
		public string SourceIP = "";

		/// <summary>
		/// 原始封包內容
		/// </summary>
		public byte[] ByteData = null;

		/// <summary>
		/// SNMP通訊協定資料單元（Protocol Data Unit，PDU）
		/// </summary>
		public SnmpSharpNet.PduType PduType = SnmpSharpNet.PduType.V2Trap;

		/// <summary>
		/// 社群字串，大部分設備皆使用public作為預設SNMP community
		/// </summary>
		public string Community = "";

		/// <summary>
		/// 接收時間
		/// </summary>
		public System.DateTime RcvTime = System.DateTime.Now;

		/// <summary>
		/// SNMP Trap OID
		/// </summary>
		public string TrapOID = "";

		/// <summary>
		///  Variable Binding的索引物件，key:OID；value:OID Value
		/// </summary>
		public System.Collections.Generic.Dictionary<string, string> DicVarbinds = new Dictionary<string, string>();

	}
}
