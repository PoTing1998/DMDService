using ASI.Lib.Config;
using ASI.Lib.Log;
using ASI.Lib.Process;
using NModbus;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.TaskOCS
{
    /// 上一站的 離站 > 軌 > 進站 > 停靠 > 離站  要考慮 降級模式   
    /// <summary>
    /// 處理 OCS 模組執行的連續資料獲取過程。
    /// </summary>
    public class ProcTaskOCS : ProcBase
    {
        private CancellationTokenSource _cancellationTokenSource;
        private OCSData _ocsData;
        private string _ocsServerConnStr; 
        private int _fetchInterval; // 資料獲取間隔（毫秒）

        public ProcTaskOCS(int fetchInterval = 100) // 預設間隔為0.1秒
        {
            _fetchInterval = fetchInterval;
            _cancellationTokenSource = new CancellationTokenSource();
            _ocsData = InitializeOcsData();
        }

        /// <summary>
        /// 開始連續的資料獲取過程。
        /// </summary>
        public async Task StartContinuousDataFetchAsync()
        {
            await Task.Run(() => ContinuousDataFetch(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// 停止連續的資料獲取過程。
        /// </summary>
        public void StopContinuousDataFetch()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// 連續性地以指定間隔獲取資料。
        /// </summary>
        private async void ContinuousDataFetch(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await FetchAndProcessDataAsync();
                    await Task.Delay(_fetchInterval, cancellationToken); // 等待指定的間隔
                }
                catch (TaskCanceledException)
                {
                    // 處理取消任務
                    DebugLog.Log("Info", "資料獲取過程已被停止。");
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("Error", $"資料獲取過程中出現錯誤: {ex.Message}");
                    // 可選：此處可加入重試邏輯
                }
            }
        }

        /// <summary>
        /// 從 Modbus 伺服端獲取資料並處理  
        /// </summary>
        private async Task FetchAndProcessDataAsync()
        {
            ushort startAddress = 30001;
            const int numIterations = 18;

            for (int i = 0; i < numIterations; i++)
            {
                try
                {
                    var processedBytes = new List<byte>();
                    _ocsData.RegisterBuffer = await _ocsData.Master.ReadInputRegistersAsync(_ocsData.SlaveAddress, startAddress, _ocsData.NumberOfPoints);

                    LogRawData(_ocsData.RegisterBuffer);
                    ProcessData(_ocsData.RegisterBuffer, processedBytes); 

                    startAddress += 100;
                    LogProcessedData(processedBytes, startAddress); 
                }
                catch (Exception ex)
                {
                    ErrorLog.Log("Error", $"在 FetchAndProcessDataAsync 中出現異常: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 初始化 OCSData 與 Modbus 設定。
        /// </summary>   
        private OCSData InitializeOcsData(string clientIP = null, int port = 0)
        {
            var ocsData = new OCSData(clientIP, (byte)port)
            {
                ModbusFactory = new ModbusFactory(),
                Master = new ModbusFactory().CreateMaster(new TcpClient(clientIP ?? ConfigApp.Instance.GetConfigSetting("TcpClientIP"), int.Parse(ConfigApp.Instance.GetConfigSetting("TcpClientPort"))))
            };

            ocsData.Master.Transport.ReadTimeout = ocsData.TransactionTimeout;
            ocsData.Master.Transport.Retries = ocsData.ConnectionTries;
            ocsData.Master.Transport.WaitToRetryMilliseconds = ocsData.WaitToRetryMilliseconds; 

            return ocsData;
        }

        /// <summary>
        /// 處理原始的 Modbus 資料。
        /// </summary>
        private void ProcessData(ushort[] registerBuffer, List<byte> outputBytes)
        {
            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i)) 
                {
                    i++; // 跳過下一個索引  
                }
                else
                {
                    byte[] bytes = BitConverter.GetBytes(registerBuffer[i]);    
                    outputBytes.AddRange(bytes);
                }
            }
        }

        /// <summary>
        /// 記錄原始資料。 
        /// </summary>
        private void LogRawData(ushort[] registerBuffer)
        {
            string logMessage = string.Join("\n", registerBuffer.Select((data, idx) => $"原始資料 {idx + 1}: {data}"));
            DebugLog.Log("Raw_OCS_Data", logMessage);  
        }

        /// <summary> 
        /// 記錄處理後的資料。
        /// </summary>
        private void LogProcessedData(List<byte> dataList, ushort address)
        {
            string logMessage = $"目前的 Address {address}:\n" +
                                string.Join("\n", dataList.Select((data, idx) => $"處理後資料 {idx + 1}: {data}"));
            DebugLog.Log("Processed_OCS_Data", logMessage);
        }

        /// <summary>
        /// 判斷該索引是否需要特殊處理。
        /// </summary>
        private bool IsSpecialIndex(int index)
        {
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }


       
    }
}
