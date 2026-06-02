using ASI.Lib.Process;
using DMDService.Services.Interfaces;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.Text;
using TaskOCS;
using static OCSClientPoller;

namespace DMDService.Services.Services
{
    public class OcsService : IOcsService
    {
        #region Fields

        private static readonly object _fileLock = new object();
        private readonly HashSet<int> _specialIndices = new HashSet<int> { 11, 13, 27, 29 };

        private ModbusTcpClient _master;

        private const ushort NumberOfPoints = 38;
        private const int TransactionTimeout = 1000;

        #endregion

        #region Events

        public event Action<int, string> LogData;
        public event Action<string> LogError;

        #endregion

        #region Public Methods

        /// <summary>
        /// 連線至 Modbus 伺服器並依地址範圍讀取寄存器（最多 18 次迭代）
        /// </summary>
        public void ConnectAndRead(string ip, int port, byte slaveAddress, ushort startAddress, ushort endAddress)
        {
            try
            {
                _master = new ModbusTcpClient();
                _master.ReadTimeout = TransactionTimeout;
                _master.Connect(ip, port);

                const int numIterations = 18;
                ushort currentAddress = startAddress;

                for (int iteration = 0; iteration < numIterations; iteration++)
                {
                    if (currentAddress > endAddress) break;

                    List<byte> newByteList = new List<byte>();
                    ushort[] buffer = _master.ReadInputRegisters(slaveAddress, currentAddress, NumberOfPoints);

                    int channel = GetChannel(currentAddress);
                    string logText = ProcessBuffer(buffer, newByteList);

                    RaiseLogData(channel, $"=====目前的 address: {currentAddress} ======\n");
                    RaiseLogData(channel, logText);

                    LogReceivedData(newByteList, currentAddress);

                    currentAddress += 100;
                }
            }
            catch (Exception ex)
            {
                LogError?.Invoke("錯誤問題: " + ex.Message);
            }
        }

        /// <summary>
        /// 啟動 OCSClientPoller，建立三組固定 IP 配置並開始輪詢
        /// </summary>
        public void StartOCSPolling()
        {
            try
            {
                var clients = new Dictionary<string, ClientModbusConfig>
                {
                    { "Client1", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 30001, 30101, 30201, 30301, 30401, 30501 } } },
                    { "Client2", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 30601, 30701, 30801, 30901, 31001, 31101 } } },
                    { "Client3", new ClientModbusConfig { IP = "10.107.26.99", StartAddresses = new List<ushort> { 31201, 31301, 31401, 31501, 31601, 31701 } } }
                };

                var poller = new OCSClientPoller(clients, SendToTaskDCU);
                poller.StartPollingAllClients();
            }
            catch (Exception ex)
            {
                LogError?.Invoke("初始化失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// 停止輪詢並關閉 Modbus 連線
        /// </summary>
        public void Stop()
        {
            try
            {
                _master?.Close();
                _master = null;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("OcsService", ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 依地址範圍回傳 channel 編號（1 / 2 / 3）
        /// </summary>
        private int GetChannel(ushort address)
        {
            if (address > 30000 && address < 30600) return 1;
            if (address > 30600 && address < 31200) return 2;
            if (address > 31200 && address < 31800) return 3;
            return 1;
        }

        /// <summary>
        /// 處理寄存器緩衝區：依特殊索引組合高低 byte，回傳顯示文字並填充 byteList
        /// </summary>
        private string ProcessBuffer(ushort[] registerBuffer, List<byte> newByteList)
        {
            if (registerBuffer == null || newByteList == null) return string.Empty;

            var logBuilder = new StringBuilder();

            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (_specialIndices.Contains(i) && i + 1 < registerBuffer.Length)
                {
                    ushort high = registerBuffer[i];
                    ushort low = registerBuffer[i + 1];
                    int combined = (high << 16) | low;

                    logBuilder.AppendLine($"接收第 {i + 1} 個資料: {high}");
                    logBuilder.AppendLine($"接收第 {i + 2} 個資料: {low}");
                    logBuilder.AppendLine($"  ↳ 組合後的整數: {combined}");

                    newByteList.Add((byte)high);
                    newByteList.Add((byte)(high >> 8));
                    newByteList.Add((byte)low);
                    newByteList.Add((byte)(low >> 8));

                    i++; // 跳過下一個（已合併）
                }
                else
                {
                    ushort value = registerBuffer[i];
                    byte[] bytes = BitConverter.GetBytes(value);
                    newByteList.AddRange(bytes);

                    logBuilder.AppendLine($"接收第 {i + 1} 個資料: {value}");
                    logBuilder.AppendLine($"  ↳ 低8位: {bytes[0]}");
                    logBuilder.AppendLine($"  ↳ 高8位: {bytes[1]}");
                }
            }

            return logBuilder.ToString();
        }

        /// <summary>
        /// 將接收的 byte 資料記錄到 Debug 日誌
        /// </summary>
        private void LogReceivedData(List<byte> byteList, ushort currentAddress)
        {
            if (byteList == null || byteList.Count == 0) return;

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"目前的 Address {currentAddress}：");
            for (int i = 0; i < byteList.Count; i++)
                logBuilder.AppendLine($"接收第 {i + 1} 個資料: {byteList[i]}");

            ASI.Lib.Log.DebugLog.Log("OCS", logBuilder.ToString());
        }

        /// <summary>
        /// 組裝 MSGFromTaskOCS 封包並透過 ProcMsg 傳送至 TaskDCU
        /// </summary>
        private void SendToTaskDCU(int msgType, int msgID, string jsonData)
        {
            try
            {
                var msg = new ASI.Wanda.DMD.ProcMsg.MSGFromTaskOCS(new MSGFrameBase("TaskOCS", "dmdserverTaskDCU"));
                msg.MessageType = msgType;
                msg.MessageID = msgID;
                msg.JsonData = jsonData;
                ASI.Lib.Process.ProcMsg.SendMessage(msg);

                lock (_fileLock)
                    ASI.Lib.Log.DebugLog.Log("SendToTaskDCU", jsonData);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("SendToTaskDCU", ex);
            }
        }

        private void RaiseLogData(int channel, string text)
        {
            LogData?.Invoke(channel, text);
        }

        #endregion
    }
}
