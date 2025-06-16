using ASI.Wanda.DMD.JsonObject.DCU.FromDMD;
using NModbus;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class OCSClientPoller
{

    private readonly string _procName = "OCSClientPoller";
    private readonly int _pollingIntervalMs = 1000;

    private readonly Dictionary<string, ushort[][]> _previousDataDict = new Dictionary<string, ushort[][]>();
    private readonly Dictionary<string, OCSPlatform[]> _platformDict = new Dictionary<string, OCSPlatform[]>();
 
    private readonly Action<int, int, string> _sendToTaskDCU;

    private readonly Dictionary<string, ClientModbusConfig> _clients;

    public OCSClientPoller(Dictionary<string, ClientModbusConfig> clients, Action<int, int, string> sendToTaskDCU)
    {
        _clients = clients;
        _sendToTaskDCU = sendToTaskDCU; 
    }


  

    public void StartPollingAllClients()
    {
        foreach (var client in _clients.Keys)
        {
            string clientName = client;
            Task.Factory.StartNew(() => PollingLoop(clientName), TaskCreationOptions.LongRunning);
        }
    }
    public class ClientModbusConfig
    {
        public string IP { get; set; }
        public int Port { get; set; } = 502;
        public byte SlaveId { get; set; } = 0;
        public List<ushort> StartAddresses { get; set; }
    }
    

    private void PollingLoop(string clientName)
    {
        var tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        
        var clientConfig = _clients[clientName]; 
        string clientIP = clientConfig.IP;
        int clientPort = clientConfig.Port;
        List<ushort> startAddresses = clientConfig.StartAddresses;

        _previousDataDict[clientName] = new ushort[startAddresses.Count][];
        _platformDict[clientName] = Enumerable.Range(0, startAddresses.Count)
                                              .Select(_ => new OCSPlatform())
                                              .ToArray();

        while (!token.IsCancellationRequested)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(clientIP, clientPort);
                    var factory = new ModbusFactory();
                    using (var master = factory.CreateMaster(tcpClient))
                    {
                        master.Transport.ReadTimeout = 1000;
                        
                        for (int groupIndex = 0; groupIndex < startAddresses.Count; groupIndex++)
                        {
                            ushort startAddress = startAddresses[groupIndex];
                            ushort numRegisters = 38;

                            ushort[] currentData = master.ReadInputRegisters(clientConfig.SlaveId, startAddress, numRegisters); 
                            byte[] byteArray = new byte[currentData.Length * 2];

                            for (int i = 0; i < currentData.Length; i++)
                            {
                                byteArray[i * 2] = (byte)(currentData[i] >> 8);
                                byteArray[i * 2 + 1] = (byte)(currentData[i] & 0xFF);
                            }
                            // 取出前次資料來進行變更比對
                            var previousGroupData = _previousDataDict[clientName];
                            if (previousGroupData[groupIndex] == null ||
                                !AreArraysEqual(previousGroupData[groupIndex], currentData))
                            {
                                Console.WriteLine($"[Debug] 內容變動：GroupIndex={groupIndex}");
                                Console.WriteLine($"[Debug] 舊資料: {string.Join(",", previousGroupData[groupIndex] ?? new ushort[0])}");
                                Console.WriteLine($"[Debug] 新資料: {string.Join(",", currentData)}");
                                previousGroupData[groupIndex] = (ushort[])currentData.Clone();

                                var platform = _platformDict[clientName][groupIndex];
                                platform.UpdateFromUShortArray(currentData);

                                int special1 = DetermineTrainStatus(byteArray);
                                int special2 = DetermineTrainStatus(byteArray);

                               
                                var oJsonObject = new TrainMSG(ASI.Wanda.DMD.Enum.Station.OCC)
                                {
                                    Start_Address = startAddress,
                                    Type = "Train",
                                    Command = "Update",
                                    Platform_id = platform.PlatformID,
                                    Arrive_time1 = platform.ArrivalTime1,
                                    Depart_time1 = platform.DepartureTime1,
                                    Destination1 = platform.DestinationNumber1,
                                    Arrive_time2 = platform.ArrivalTime2,
                                    Depart_time2 = platform.DepartureTime2,
                                    Destination2 = platform.DestinationNumber2,
                                    Special1 = special1,
                                    Special2 = special2,
                                };

                                Console.WriteLine($"[Send] client={clientName}, groupIndex={groupIndex}, startAddr={startAddress}");
                                var jsonString = ASI.Lib.Text.Parsing.Json.SerializeObject(oJsonObject);
                                _sendToTaskDCU(2, 0, jsonString);

                                //updateTrainMessage(currentData);
                            }
          


                            Thread.Sleep(20);
                        }
                    }
                }
            }
            catch (IOException ioEx)
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} IO 錯誤: {ioEx.Message}，嘗試重新連線...");
                TryReconnect();
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} 錯誤: {ex.Message}");
            }

            Thread.Sleep(_pollingIntervalMs);
        }
    }


    private bool AreArraysEqual(ushort[] a, ushort[] b)
    {
        if (a == null || b == null) return false;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }


    private int DetermineTrainStatus(byte[] data)
    {
        if (data.Length <= 36) return 0;

        if (data[17] != 0 || data[19] != 0 || data[20] != 0 ||
            data[33] != 36 || data[35] != 0 || data[36] != 0)
        {
            return 1;
        }

        return 0;
    }

    private void TryReconnect()
    {
        const int maxRetries = 3;
        const int delayBetweenRetriesMs = 3000; // 3 秒

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"重新連線中... 第 {attempt} 次嘗試");

                // 模擬連線測試（如果有 ping 或心跳）

                Thread.Sleep(delayBetweenRetriesMs);
                
                break;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"重新連線失敗（第 {attempt} 次）: {ex.Message}");

                if (attempt == maxRetries)
                {
                    ASI.Lib.Log.ErrorLog.Log(_procName, "已達最大重試次數，放棄重連");
                }
            }
        }
    }
    private void TryReconnectUntilSuccess(string clientIP, int clientPort)
    {
        while (true)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(clientIP, clientPort);
                    ASI.Lib.Log.ErrorLog.Log(_procName, "重新連線成功！");
                    break;
                }
            }
            catch
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, "重新連線失敗，3 秒後再試...");
                Thread.Sleep(3000);
            }
        }
    }


    private void UpdateOCSData(ushort[] ID)
    {
        try
        {
            int intID = ID[1];

            ASI.Wanda.DMD.DB.Tables.Train.ocsData.updatePlatform_ID(intID);
        }
        catch (Exception updateException)
        {
            ASI.Lib.Log.ErrorLog.Log("Error updating dmd_playlist", updateException);
        }
    }
    private void updateTrainMessage(ushort[] platform_id)
    {
        try
        {
            int intID = platform_id[1];//尚未與OCS確認id 的index

            ASI.Wanda.DMD.DB.Tables.Train.trainMessage.UpdateTrain_MSG(intID);
        }
        catch (Exception)
        {

            throw;
        }
    }


}
