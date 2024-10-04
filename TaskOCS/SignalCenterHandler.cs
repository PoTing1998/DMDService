using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using EasyModbus; // 引入 EasyModbus 函式庫

public class SignalCenterHandler
{
    // 每個月台有各自的 ModbusClient，使用 Dictionary 儲存
    private Dictionary<int, ModbusClient> modbusClients = new Dictionary<int, ModbusClient>();

    // 儲存每個月台的資料
    public Dictionary<int, List<byte>> StationData { get; private set; } = new Dictionary<int, List<byte>>();

    // 初始化每個月台的 Modbus 連線，並設置初始資料
    public void InitializeStationData(Dictionary<int, (string ipAddress, int port)> stationAddresses)
    {
        foreach (var station in stationAddresses)
        {
            int stationId = station.Key;
            string ipAddress = station.Value.ipAddress;
            int port = station.Value.port;

            try
            {
                // 為每個月台建立 ModbusClient 並連接
                ModbusClient client = new ModbusClient(ipAddress, port);
                client.ConnectionTimeout = 5000; // 設定連線超時時間
                client.Connect(); // 連線到 Modbus 伺服器

                // 儲存連線物件
                modbusClients[stationId] = client;
                Console.WriteLine($"Connected to Station {stationId} at {ipAddress}:{port}.");

                // 初始化該月台的資料 (假設每個月台初始資料為 40 bytes)
                StationData[stationId] = new List<byte>(new byte[40]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to Station {stationId} at {ipAddress}:{port} - {ex.Message}");
            }
        }
        Console.WriteLine("Initialized station data and connections for all stations.");
    }

    // 模擬從所有月台接收新資料
    public Dictionary<int, byte[]> GetNewDataFromStations()
    {
        var newData = new Dictionary<int, byte[]>();

        foreach (var clientEntry in modbusClients)
        {
            int stationId = clientEntry.Key;
            ModbusClient client = clientEntry.Value;

            try
            {
                // 讀取月台資料（假設每個月台資料長度為 20 個暫存器，40 個位元組）
                byte[] stationData = ReadStationDataFromModbus(client, 0, 20); // 起始位址為 0，讀取 20 個暫存器 
                newData[stationId] = stationData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read data from Station {stationId} - {ex.Message}");
            }
        }
        return newData;
    }

    // 從 Modbus 伺服器讀取月台資料
    private byte[] ReadStationDataFromModbus(ModbusClient client, int startAddress, int numberOfRegisters)
    {
        // 使用 ReadHoldingRegisters 方法讀取指定數量的暫存器
        int[] registerValues = client.ReadHoldingRegisters(startAddress, numberOfRegisters);

        // 將每個暫存器的值轉換成 byte 陣列（每個暫存器值為 2 bytes）
        byte[] byteArray = new byte[numberOfRegisters * 2];
        for (int i = 0; i < numberOfRegisters; i++)
        {
            byteArray[i * 2] = (byte)(registerValues[i] >> 8); // 高位元組
            byteArray[i * 2 + 1] = (byte)(registerValues[i] & 0xFF); // 低位元組
        }
        return byteArray;
    }
    
    // 比對並更新所有月台的資料
    public void CompareAndUpdateStationData(Dictionary<int, byte[]> newStationData)
    {
        foreach (var stationEntry in newStationData)
        {
            int stationId = stationEntry.Key;
            byte[] newByteArray = stationEntry.Value;

            // 比對並更新單個月台資料
            CompareAndUpdateSingleStation(stationId, newByteArray);
        }
    }

    // 比對並更新單個月台的資料
    private void CompareAndUpdateSingleStation(int stationId, byte[] newByteArray)
    {
        if (!StationData.ContainsKey(stationId))
        {
            Console.WriteLine($"Station ID {stationId} does not exist.");
            return;
        }

        List<byte> currentData = StationData[stationId];

        if (currentData.Count != newByteArray.Length) 
        {
            Console.WriteLine($"Length mismatch for Station ID {stationId}.");
            return;
        }

        // 使用 HashSet 來記錄變更的索引
        HashSet<int> changedIndices = new HashSet<int>();

        // 逐一比對每個索引位置的 byte 值，使用 XOR 判斷是否相同
        for (int i = 0; i < currentData.Count; i++)
        {
            if ((currentData[i] ^ newByteArray[i]) != 0) // 當 XOR 結果不為 0 時，表示值不同 
            {
                changedIndices.Add(i); // 記錄變更的索引
            }
        }

        // 若有變更的索引，進行更新
        if (changedIndices.Count > 0)
        {
            foreach (int index in changedIndices)
            {
                currentData[index] = newByteArray[index];
                Console.WriteLine($"Station {stationId} - Updated index {index} with value {newByteArray[index]}.");
            }

            Console.WriteLine($"Station {stationId} - Data has been updated.");
        }
        else
        {
            Console.WriteLine($"Station {stationId} - No changes detected.");
        }
    }

    // 顯示所有月台的資料（僅顯示前 5 bytes）
    public void PrintStationData()
    {
        foreach (var stationEntry in StationData)
        {
            int stationId = stationEntry.Key;
            List<byte> byteArray = stationEntry.Value;

            Console.Write($"Station {stationId} data (first 5 bytes): ");
            for (int i = 0; i < Math.Min(5, byteArray.Count); i++)
            {
                Console.Write($"{byteArray[i]} ");
            }
            Console.WriteLine();
        }
    }
}


