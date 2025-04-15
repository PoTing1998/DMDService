using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskOCS
{
    public class ModbusPollingService
    {
        private readonly OCSData _ocsData;
        private CancellationTokenSource _cts;
        private Task _pollingTask;
        private readonly int _pollingIntervalMs;
        private readonly string _procName = "ModbusPollingService";

        public ModbusPollingService(OCSData ocsData, int pollingIntervalMs = 10000)
        {
            _ocsData = ocsData;
            _pollingIntervalMs = pollingIntervalMs;
        }

        public void Start()
        {
            if (_pollingTask != null && !_pollingTask.IsCompleted)
                return;

            _cts = new CancellationTokenSource();
            _pollingTask = Task.Run(() => PollingLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async Task PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    ushort startAddress = 0;
                    ushort numRegisters = 10;
                    byte slaveId = 1;

                    ushort[] registers = _ocsData.Master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
                    Console.WriteLine($"讀取結果: {string.Join(", ", registers)}");
                }
                catch (IOException ioEx)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, $"IO 錯誤: {ioEx.Message}，嘗試重新連線...");
                    TryReconnect();
                }
                catch (Exception ex)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, $"Modbus 讀取錯誤: {ex.Message}");
                }

                await Task.Delay(_pollingIntervalMs, token);
            }

            ASI.Lib.Log.DebugLog.Log(_procName, "Modbus 輪詢已停止。");
        }

        private void TryReconnect()
        {
            for (int i = 0; i < _ocsData.ConnectionTries; i++)
            {
                try
                {
                    var tcpClient = new TcpClient(_ocsData.ClientIP, _ocsData.Port);
                    _ocsData.Master = _ocsData.ModbusFactory.CreateMaster(tcpClient);
                    _ocsData.Master.Transport.ReadTimeout = _ocsData.TransactionTimeout;
                    _ocsData.Master.Transport.Retries = _ocsData.ConnectionTries;
                    _ocsData.Master.Transport.WaitToRetryMilliseconds = _ocsData.WaitToRetryMilliseconds;

                    ASI.Lib.Log.DebugLog.Log(_procName, "重新連線成功");
                    return;
                }
                catch
                {
                    Thread.Sleep(_ocsData.WaitToRetryMilliseconds);
                }
            }

            ASI.Lib.Log.ErrorLog.Log(_procName, "無法重新連線 Modbus slave");
        }
    }

}
