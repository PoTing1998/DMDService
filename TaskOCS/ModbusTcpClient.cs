using System;
using System.Net.Sockets;

namespace TaskOCS
{
    /// <summary>
    /// 使用 System.Net.Sockets 實現的 Modbus TCP 客戶端
    /// </summary>
    public class ModbusTcpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private ushort _transactionId = 0;
        private readonly object _lock = new object();

        public int ReadTimeout { get; set; } = 1000;
        public int WriteTimeout { get; set; } = 1000;

        /// <summary>
        /// 連接到 Modbus TCP 伺服器（含 5 秒連線 timeout）
        /// </summary>
        public void Connect(string ipAddress, int port)
        {
            _tcpClient = new TcpClient();

            // TcpClient.Connect() 本身無 timeout，改用非同步方式限制等待時間
            var connectTask = _tcpClient.ConnectAsync(ipAddress, port);
            if (!connectTask.Wait(TimeSpan.FromSeconds(5)))
            {
                _tcpClient.Close();
                _tcpClient = null;
                throw new TimeoutException($"連線逾時（IP={ipAddress}, Port={port}）");
            }

            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = ReadTimeout;
            _stream.WriteTimeout = WriteTimeout;
        }

        /// <summary>
        /// 檢查是否已連接
        /// </summary>
        public bool IsConnected
        {
            get { return _tcpClient != null && _tcpClient.Connected; }
        }

        /// <summary>
        /// 讀取 Holding Registers (Function Code 03)
        /// </summary>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            return ReadRegisters(slaveId, 0x03, startAddress, numberOfPoints);
        }

        /// <summary>
        /// 讀取 Input Registers (Function Code 04)
        /// </summary>
        public ushort[] ReadInputRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            return ReadRegisters(slaveId, 0x04, startAddress, numberOfPoints);
        }

        /// <summary>
        /// 通用的讀取暫存器方法
        /// </summary>
        private ushort[] ReadRegisters(byte slaveId, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            lock (_lock)
            {
                if (!IsConnected)
                    throw new InvalidOperationException("未連接到 Modbus 伺服器");

                // 構建 Modbus TCP 請求
                byte[] request = BuildRequest(slaveId, functionCode, startAddress, numberOfPoints);

                // 發送請求
                _stream.Write(request, 0, request.Length);

                // 接收回應
                byte[] response = ReceiveResponse();

                // 解析回應
                return ParseResponse(response, numberOfPoints);
            }
        }

        /// <summary>
        /// 構建 Modbus TCP 請求封包
        /// </summary>
        private byte[] BuildRequest(byte slaveId, byte functionCode, ushort startAddress, ushort numberOfPoints)
        {
            _transactionId++;

            byte[] request = new byte[12];

            // MBAP Header
            request[0] = (byte)(_transactionId >> 8);      // Transaction ID High
            request[1] = (byte)(_transactionId & 0xFF);    // Transaction ID Low
            request[2] = 0x00;                              // Protocol ID High (0 = Modbus)
            request[3] = 0x00;                              // Protocol ID Low
            request[4] = 0x00;                              // Length High
            request[5] = 0x06;                              // Length Low (6 bytes following)
            request[6] = slaveId;                           // Unit ID

            // PDU (Protocol Data Unit)
            request[7] = functionCode;                      // Function Code
            request[8] = (byte)(startAddress >> 8);         // Start Address High
            request[9] = (byte)(startAddress & 0xFF);       // Start Address Low
            request[10] = (byte)(numberOfPoints >> 8);      // Quantity High
            request[11] = (byte)(numberOfPoints & 0xFF);    // Quantity Low

            return request;
        }

        /// <summary>
        /// 接收 Modbus TCP 回應
        /// </summary>
        private byte[] ReceiveResponse()
        {
            // 先讀取 MBAP Header (7 bytes) 
            byte[] header = new byte[7];
            int bytesRead = 0;
            while (bytesRead < 7)
            {
                int read = _stream.Read(header, bytesRead, 7 - bytesRead);
                if (read == 0)
                    throw new Exception("連接已關閉");
                bytesRead += read;
            }
            
            // 取得資料長度
            int length = (header[4] << 8) | header[5];
            if (length < 2)
                throw new Exception($"MBAP Length 欄位異常（值={length}），最小應為 2（Unit ID + Function Code）");

            // 讀取剩餘的資料 (PDU)
            byte[] pdu = new byte[length - 1]; // length 包含 Unit ID，已在 header 中
            bytesRead = 0;
            while (bytesRead < pdu.Length)
            {
                int read = _stream.Read(pdu, bytesRead, pdu.Length - bytesRead);
                if (read == 0)
                    throw new Exception("連接已關閉");
                bytesRead += read;
            }

            // 組合完整回應
            byte[] response = new byte[7 + pdu.Length];
            Array.Copy(header, 0, response, 0, 7);
            Array.Copy(pdu, 0, response, 7, pdu.Length);

            return response;
        }

        /// <summary>
        /// 解析 Modbus TCP 回應封包
        /// </summary>
        private ushort[] ParseResponse(byte[] response, ushort expectedCount)
        {
            // 檢查回應長度（MBAP 7 bytes + FunctionCode 1 byte + ByteCount 1 byte + 資料）
            int expectedLength = 9 + expectedCount * 2;
            if (response.Length < expectedLength)
                throw new Exception($"回應封包長度不足：預期 {expectedLength} bytes，實際 {response.Length} bytes");

            byte functionCode = response[7];

            // 檢查是否為錯誤回應
            if ((functionCode & 0x80) != 0)
            {
                byte exceptionCode = response[8];
                throw new Exception($"Modbus 異常回應: Function Code = {functionCode}, Exception Code = {exceptionCode}");
            }

            byte byteCount = response[8];
            int expectedBytes = expectedCount * 2;

            if (byteCount != expectedBytes)
                throw new Exception($"回應資料長度不符: 預期 {expectedBytes} bytes, 實際 {byteCount} bytes");

            // 解析暫存器資料
            ushort[] registers = new ushort[expectedCount];
            for (int i = 0; i < expectedCount; i++)
            {
                int offset = 9 + (i * 2);
                registers[i] = (ushort)((response[offset] << 8) | response[offset + 1]);
            }

            return registers;
        }

        /// <summary>
        /// 關閉連接
        /// </summary>
        public void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
