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
            // 建立日誌訊息，包含當前的 Modbus 地址
            string logMessage = $"目前的 Address {currentAddress}：\n";

            int index = 1;
            foreach (var data in byteList)
            {
                logMessage += $"接收第 {index} 個資料: {data}\n";
                index++;
            }

            // 將資料寫入日誌中
            ASI.Lib.Log.DebugLog.Log("OCS", logMessage);
        }



        #region method  
        /// <summary>
        /// 處理註冊緩衝區的資料，並依據特殊索引進行不同的顯示和資料組合操作。
        /// </summary>
        /// <param name="registerBuffer">包含 ushort 數據的註冊緩衝區</param>
        /// <param name="newByteList">儲存轉換後 byte 數據的列表</param>
        /// <param name="textBox">用於顯示資料的 RichTextBox 控件</param>
        void Process(ushort[] registerBuffer, List<byte> newByteList, RichTextBox textBox)
        {
            for (int i = 0; i < registerBuffer.Length; i++)
            {
                // 如果索引為特殊索引，進行特殊資料處理
                if (IsSpecialIndex(i))
                {
                    ushort firstValue = registerBuffer[i];
                    ushort secondValue = registerBuffer[i + 1];

                    // 顯示特殊資料組合的值
                    DisplaySpecialValue(i, firstValue, secondValue, textBox);

                    // 將兩個 ushort 組合成 byte 數組並加入到 newByteList 中
                    byte[] combinedBytes = CombineBytes(firstValue, secondValue);
                    newByteList.AddRange(combinedBytes);

                    i++; // 跳過下一個索引
                }
                else
                {
                    // 將單個 ushort 轉換為 byte 數組
                    byte[] ushortBytes = BitConverter.GetBytes(registerBuffer[i]);
                    newByteList.AddRange(ushortBytes);

                    // 顯示單個 ushort 的值
                    DisplayUshortValue(i, registerBuffer[i], ushortBytes, textBox);
                }
            }
        }

        /// <summary>
        /// 顯示普通 ushort 資料的值及其對應的 byte 數據。
        /// </summary>
        /// <param name="index">當前索引位置</param>
        /// <param name="value">ushort 值</param>
        /// <param name="bytes">轉換後的 byte 數組</param>
        /// <param name="textBox">用於顯示資料的 RichTextBox 控件</param>
        void DisplayUshortValue(int index, ushort value, byte[] bytes, RichTextBox textBox)
        {
            textBox.Text += $"接收第 {index + 1} 個資料: {value}\n";
            textBox.Text += $"接收第 {index + 1} 個資料(低8位): {bytes[0]}\n";
            textBox.Text += $"接收第 {index + 1} 個資料(高8位): {bytes[1]}\n";
        }

        /// <summary>
        /// 顯示特殊索引對應的兩個 ushort 資料及其組合後的數字。
        /// </summary>
        /// <param name="index">當前索引位置</param>
        /// <param name="firstValue">第一個 ushort 值</param>
        /// <param name="secondValue">第二個 ushort 值</param>
        /// <param name="textBox">用於顯示資料的 RichTextBox 控件</param>
        void DisplaySpecialValue(int index, ushort firstValue, ushort secondValue, RichTextBox textBox)
        {
            int combinedNumber = CombineUshortToInt(firstValue, secondValue);
            textBox.Text += $"接收第 {index + 1} 個資料: {firstValue}\n";
            textBox.Text += $"接收第 {index + 2} 個資料: {secondValue}\n";
            textBox.Text += $"組合後的數字: {combinedNumber}\n";
        }

        /// <summary>
        /// 將兩個 ushort 組合為一個 int 整數值。
        /// </summary>   
        /// <param name="highOrder">高位 ushort 數值</param>
        /// <param name="lowOrder">低位 ushort 數值</param>
        /// <returns>組合後的整數值</returns>
        int CombineUshortToInt(ushort highOrder, ushort lowOrder)
        {
            // 創建 4 個 byte 的數組來表示 int
            byte[] bytes = new byte[4];
            // 將兩個 ushort 數值的高低位分別放入 byte 數組中
            bytes[3] = (byte)(lowOrder >> 8);
            bytes[2] = (byte)lowOrder;
            bytes[1] = (byte)(highOrder >> 8);
            bytes[0] = (byte)highOrder;
            // 使用 BitConverter 將 byte 數組轉換為 int
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 將兩個 ushort 的高低位組合為 4 個 byte 數組。
        /// </summary>
        /// <param name="highOrder">高位 ushort 數值</param> 
        /// <param name="lowOrder">低位 ushort 數值</param>
        /// <returns>組合後的 byte 數組</returns>
        byte[] CombineBytes(ushort highOrder, ushort lowOrder)
        {
            // 創建 4 個 byte 的數組來表示組合的結果
            byte[] bytes = new byte[4];
            // 將兩個 ushort 數值的高低位分別放入 byte 數組中  
            bytes[3] = (byte)(lowOrder >> 8);
            bytes[2] = (byte)lowOrder;
            bytes[1] = (byte)(highOrder >> 8);
            bytes[0] = (byte)highOrder;
            return bytes;
        }

        /// <summary>
        /// 判斷當前索引是否為特殊索引。 
        /// </summary>
        /// <param name="index">當前索引</param>
        /// <returns>若為特殊索引則返回 true，否則返回 false</returns>
        bool IsSpecialIndex(int index)
        {
            // 定義特殊索引集合
            HashSet<int> specialIndices = new HashSet<int> { 11, 13, 27, 29 };
            return specialIndices.Contains(index);
        }
      
        #endregion


        private static void PoressTime()
        {
            // 十進位的值  19111169  十六進位 01 23 9D 01   UNIX = 10/08/2018 04:39:29
            // 十進位的值  19113989  十六進位 01 23 A8 05   UNIX = 10/08/2018 05:26:29
            //  ArrivalTime 由四個Byte組成  DepartureTime 也是
            // 定義各個位元組的值 
            byte byte1 = 0x01;
            byte byte2 = 0x9D;
            byte byte3 = 0x23;
            byte byte4 = 0x01;

            // 將四個位元組組合成一個 32 位元整數
            int unixTime = (byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4;
            // 設定基準時間 (假設基準時間是 2018-01-01 00:00:00) 
            DateTime baseTime = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // 計算對應的日期時間
            DateTime arrivalTime = baseTime.AddSeconds(unixTime);

            // 顯示結果
            ASI.Lib.Log.DebugLog.Log("到達時間", $"Arrival Time (UNIX): {unixTime}");
            ASI.Lib.Log.DebugLog.Log("到達精準時間", $"Arrival Time (Human Readable): {arrivalTime:yyyy-MM-dd HH:mm:ss}");
        }

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
