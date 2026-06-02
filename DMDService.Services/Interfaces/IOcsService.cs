using System;
using System.Collections.Generic;

namespace DMDService.Services.Interfaces
{
    /// <summary>
    /// OCS Modbus 通訊服務介面
    /// 封裝 ModbusTcpClient 連線、寄存器讀取、OCSClientPoller 輪詢及 SendToTaskDCU 轉發
    /// </summary>
    public interface IOcsService
    {
        /// <summary>
        /// 資料接收事件，由 UI 訂閱後顯示於對應 RichTextBox
        /// channel 1 = 地址 30001-30600，2 = 30601-31200，3 = 31201-31800
        /// </summary>
        event Action<int, string> LogData;

        /// <summary>
        /// 錯誤訊息事件
        /// </summary>
        event Action<string> LogError;

        /// <summary>
        /// 連線至 Modbus 伺服器並讀取寄存器（對應原 buttonInit_Click_1 邏輯）
        /// </summary>
        void ConnectAndRead(string ip, int port, byte slaveAddress, ushort startAddress, ushort endAddress);

        /// <summary>
        /// 啟動 OCSClientPoller 持續輪詢三組客戶端（對應原 StartOCSClients 邏輯）
        /// </summary>
        void StartOCSPolling();

        /// <summary>
        /// 停止輪詢並關閉 Modbus 連線
        /// </summary>
        void Stop();
    }
}
