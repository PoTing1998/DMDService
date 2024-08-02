using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.Comm.SNMP
{   
    /// <summary>
    /// 創建一個SnmpLib物件，並將主機、連接埠和社群傳遞給建構函式
    /// </summary>
    public class SnmpLib2
    {
        private readonly IPAddress mIPAddress;
        private readonly int mPort;

        public SnmpLib2(string hostIP, int port)
        {
            System.Net.IPAddress ipAddress;
            System.Net.IPAddress.TryParse(hostIP, out ipAddress);
            mIPAddress = ipAddress;
            mPort = port;
        }

        /// <summary>
        /// 取得指定OID的值
        /// </summary>
        /// <param name="oid">OID</param>
        public SNMPResponse Get(SNMPRequest snmpRequest)
        {
            var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(mIPAddress, mPort);
                       
            var buffer = snmpRequest.BuildSNMPGetRequest();
            socket.Send(buffer);

            var response = new SNMPResponse();
            byte[] bytes = new byte[buffer.Length];
            int bytesReceived = socket.Receive(bytes);
            response.FromBytes(bytes);

            Console.WriteLine(response.Error);
            Console.WriteLine(response.Value);

            socket.Close();

            return response;
        }
    }

    public class SNMPRequest
    {
        /// <summary>
        /// SNMP 封包的版本，0:SNMPv1；1:SNMPv2
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 某一管理群組織的共同體名稱，一般內定值為 Public
        /// </summary>
        public string Community { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Oid { get; set; }

        public byte[] BuildSNMPGetRequest()
        {
            // SNMP Version: SNMPv2c
            byte version = (byte)Version;

            // Community String
            byte[] community = System.Text.Encoding.ASCII.GetBytes(Community);

            // SNMP Get PDU Type: 0xA0
            byte pduType = 0xA0;

            // Request ID: You can set this to a unique value
            byte[] requestId = BitConverter.GetBytes((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Error Code and Error Index: Set to 0 for no error
            byte error = 0x00;
            byte errorIndex = 0x00;

            // Object Identifier (OID)
            byte[] oidBytes = ConvertOIDToBytes(Oid);

            // Construct the SNMP Get Request packet
            byte[] packet = new byte[26 + oidBytes.Length];
            packet[0] = version;
            packet[1] = (byte)community.Length;
            Array.Copy(community, 0, packet, 2, community.Length);
            packet[2 + community.Length] = pduType;
            packet[3 + community.Length] = 0x00; // Length placeholder
            Array.Copy(requestId, 0, packet, 4 + community.Length, 4);
            packet[8 + community.Length] = error;
            packet[9 + community.Length] = errorIndex;
            packet[10 + community.Length] = 0x30; // Sequence Tag
            packet[11 + community.Length] = (byte)(4 + oidBytes.Length); // Length of variable binding
            packet[12 + community.Length] = 0x06; // Object Identifier Tag
            packet[13 + community.Length] = (byte)oidBytes.Length;
            Array.Copy(oidBytes, 0, packet, 14 + community.Length, oidBytes.Length);
            packet[14 + community.Length + oidBytes.Length] = 0x05; // Null Tag
            packet[15 + community.Length + oidBytes.Length] = 0x00; // Null Length

            // Calculate the actual packet length and update the Length field
            int packetLength = 16 + community.Length + oidBytes.Length;
            packet[3 + community.Length] = (byte)(packetLength - 4);

            return packet;
        }

        public static byte[] ConvertOIDToBytes(string oid)
        {
            if (string.IsNullOrEmpty(oid))
            {
                throw new ArgumentException("OID cannot be empty or null.", nameof(oid));
            }

            string[] oidParts = oid.Split('.');
            List<byte> oidBytes = new List<byte>();

            // Process first two special OID parts (0x2B for 1.3)
            int firstPart = int.Parse(oidParts[0]);
            int secondPart = int.Parse(oidParts[1]);
            int combinedValue = (firstPart * 40) + secondPart;

            oidBytes.Add((byte)combinedValue);

            for (int i = 2; i < oidParts.Length; i++)
            {
                int oidPartValue = int.Parse(oidParts[i]);
                List<byte> tempBytes = new List<byte>();

                // Process OID value for multiple bytes
                while (oidPartValue > 0)
                {
                    byte tempByte = (byte)(oidPartValue % 128);
                    tempBytes.Insert(0, tempByte);
                    oidPartValue /= 128;
                }

                // Set the high bit for all bytes except the last one
                for (int j = 0; j < tempBytes.Count - 1; j++)
                {
                    tempBytes[j] |= 0x80;
                }

                oidBytes.AddRange(tempBytes);
            }

            return oidBytes.ToArray();
        }
    }

    public class SNMPResponse
    {
        /// <summary>
        /// 0:取得成功
        /// </summary>
        public int Error { get; set; }
        public string Value { get; set; }

        public void FromBytes(byte[] buffer)
        {
            Error = buffer[0];
            Value = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);
        }
    }
}
