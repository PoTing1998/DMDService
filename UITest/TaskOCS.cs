using Microsoft.Extensions.Configuration;

using NModbus;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UITest
{
    public partial class TaskOCS : UserControl
    {
        #region constructor
        private static ModbusFactory modbusFactory;
        private static IModbusMaster master;
        private static ushort[] registerBuffer;
        private static byte slaveAddress = 0;
        private static ushort numberOfPoints = 38;
        private static int port = 502;
        private static string client1IP;
        private static int transactionTimeout;
        private static int connectionTries;
        private static int waitToRetryMilliseconds;
        private const int SpecialValueCheckInterval = 18;
        private const int RegisterAddressIncrement = 100;
        private const int SpecialIndexCheckInterval = 6; // 檢查特殊索引的間隔

        #endregion
        public TaskOCS()
        {
            InitializeComponent();
            InitializeATSModbusAPI();
        }
        private void InitializeATSModbusAPI()
        {
            registerBuffer = new ushort[] { };
            transactionTimeout = 1000;
            connectionTries = 20;
            waitToRetryMilliseconds = 500;
        }
        private bool isReadingData = false;
        /// <summary>
        /// 將接收的資料記錄到日誌檔案中。
        /// </summary>
        /// <param name="byteList">要記錄的 byte 資料列表</param>
        /// <param name="currentAddress">當前操作的 Modbus 地址</param>
        private void LogReceivedData(List<byte> byteList, ushort currentAddress)
        {
            if (byteList == null || byteList.Count == 0) return;

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"目前的 Address {currentAddress}：");

            for (int i = 0; i < byteList.Count; i++)
            {
                logBuilder.AppendLine($"接收第 {i + 1} 個資料: {byteList[i]}");
            }

            ASI.Lib.Log.DebugLog.Log("OCS", logBuilder.ToString());
        }




        #region method  
        // 可重複使用的特殊索引集合
        private readonly HashSet<int> _specialIndices = new HashSet<int> { 11, 13, 27, 29 };
        /// <summary>
        /// 處理註冊緩衝區的資料，並依據特殊索引進行不同的顯示和資料組合操作。
        /// </summary>
        /// <param name="registerBuffer">包含 ushort 數據的註冊緩衝區</param>
        /// <param name="newByteList">儲存轉換後 byte 數據的列表</param>
        /// <param name="textBox">用於顯示資料的 RichTextBox 控件</param>
        public void Process(ushort[] registerBuffer, List<byte> newByteList, RichTextBox textBox)
        {
            if (registerBuffer == null || newByteList == null || textBox == null) return;

            StringBuilder logBuilder = new StringBuilder();

            for (int i = 0; i < registerBuffer.Length; i++)
            {
                if (IsSpecialIndex(i) && i + 1 < registerBuffer.Length)
                {
                    ushort high = registerBuffer[i];
                    ushort low = registerBuffer[i + 1];

                    // 顯示特殊資料組合值
                    DisplaySpecialValue(i, high, low, logBuilder);

                    // 組合為 byte 陣列並儲存
                    newByteList.AddRange(CombineBytes(high, low));

                    i++; // 跳過下一個
                }
                else
                {
                    ushort value = registerBuffer[i];
                    byte[] ushortBytes = BitConverter.GetBytes(value);

                    newByteList.AddRange(ushortBytes);
                    DisplayUshortValue(i, value, ushortBytes, logBuilder);
                }
            }

            // 一次性更新 UI
            textBox.Text = logBuilder.ToString();
        }
        /// <summary>
        /// 判斷是否為特殊索引。
        /// </summary>
        private bool IsSpecialIndex(int index)
        {
            return _specialIndices.Contains(index);
        }

        /// <summary>
        /// 顯示普通 ushort 的資料與 byte 表現形式。
        /// </summary>
        private void DisplayUshortValue(int index, ushort value, byte[] bytes, StringBuilder builder)
        {
            builder.AppendLine($"接收第 {index + 1} 個資料: {value}");
            builder.AppendLine($"  ↳ 低8位: {bytes[0]}");
            builder.AppendLine($"  ↳ 高8位: {bytes[1]}");
        }

        /// <summary>
        /// 顯示特殊索引對應的兩個 ushort 組合成 int 的結果。
        /// </summary>
        private void DisplaySpecialValue(int index, ushort high, ushort low, StringBuilder builder)
        {
            int combined = CombineUshortToInt(high, low);
            builder.AppendLine($"接收第 {index + 1} 個資料: {high}");
            builder.AppendLine($"接收第 {index + 2} 個資料: {low}");
            builder.AppendLine($"  ↳ 組合後的整數: {combined}");
        }

        /// <summary>
        /// 將兩個 ushort 組合為 int 整數。
        /// </summary>
        private int CombineUshortToInt(ushort high, ushort low)
        {
            return (high << 16) | low; // 更簡潔無需 BitConverter
        }

        /// <summary>
        /// 將兩個 ushort 組合為 4 個 byte 陣列。
        /// </summary>
        private byte[] CombineBytes(ushort high, ushort low)
        {
            byte[] result = new byte[4];
            result[0] = (byte)high;
            result[1] = (byte)(high >> 8);
            result[2] = (byte)low;
            result[3] = (byte)(low >> 8);
            return result;
        }

        #endregion

        private void buttonInit_Click_1(object sender, EventArgs e)
        {
            // 設定 Modbus 連接的 IP 地址與端口號
            client1IP = textBoxConnIP.Text;
            port = int.Parse(textBoxConnPort.Text);

            // 解析並設定從屬設備的地址
            slaveAddress = BitConverter.GetBytes(int.Parse(slaveAddressText.Text))[0];

            try
            {
                // 初始化 Modbus 工廠及其主站
                modbusFactory = new ModbusFactory();
                master = modbusFactory.CreateMaster(new TcpClient(client1IP, port));

                // 設置傳輸層的超時、重試和等待時間
                master.Transport.ReadTimeout = transactionTimeout;
                master.Transport.Retries = connectionTries;
                master.Transport.WaitToRetryMilliseconds = waitToRetryMilliseconds;

                // 解析起始和結束地址
                ushort startAddress = ushort.Parse(adressText.Text);
                ushort endAddress = ushort.Parse(adressTextEND.Text);

                const int numIterations = 18; // 設定要讀取的迴圈次數

                // 開始資料讀取迴圈
                for (int iteration = 0; iteration < numIterations; iteration++)
                {
                    List<byte> newByteList = new List<byte>();

                    // 若起始地址超過結束地址，則終止迴圈
                    if (startAddress > endAddress) break;

                    // 讀取 Modbus 註冊的資料
                    registerBuffer = master.ReadInputRegisters(slaveAddress, startAddress, (ushort)numberOfPoints);

                    // 根據不同的起始地址範圍選擇不同的 TextBox 顯示資料
                    switch (startAddress)
                    {
                        case ushort address when address > 30000 && address < 30600:
                            ReceDataText.Text += $"=====目前的 address: {startAddress} ======\n";
                            Process(registerBuffer, newByteList, ReceDataText);
                            break;
                        case ushort address when address > 30600 && address < 31200:
                            ReceDataText2.Text += $"=====目前的 address: {startAddress} ======\n";
                            Process(registerBuffer, newByteList, ReceDataText2);
                            break;
                        case ushort address when address > 31200 && address < 31800:
                            ReceDataText3.Text += $"=====目前的 address: {startAddress} ======\n";
                            Process(registerBuffer, newByteList, ReceDataText3);
                            break;
                        default:
                            // 如果地址不在任何指定範圍內，執行預設處理邏輯
                            ReceDataText.Text += $"=====目前的 address: {startAddress} (不在範圍內) ======\n";
                            Process(registerBuffer, newByteList, ReceDataText);
                            MessageBox.Show("Address 不在範圍內");
                            break;
                    }

                    // 每次迴圈結束後將起始地址增加 100，進行下一次讀取
                    startAddress += 100;

                    // 紀錄接收資料至日誌中
                    LogReceivedData(newByteList, startAddress);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("錯誤問題: " + ex.Message);
            }
        }

        private void stopButton_Click_1(object sender, EventArgs e)
        {
            if (isReadingData)
            {
                isReadingData = true; // 停止執行
            }
        }

        private void clearBT_Click_1(object sender, EventArgs e)
        {
            ReceDataText.Text = String.Empty;
            ReceDataText2.Text = String.Empty;
            ReceDataText3.Text = String.Empty;
        }
    }
}
