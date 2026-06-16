using ASI.Wanda.DMD.JsonObject.DCU.FromDMD;
using OCS.Modbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskOCS;

public class OCSClientPoller
{
    private readonly string _procName = "OCSClientPoller";
    private readonly int _pollingIntervalMs = 1000;

    private readonly Dictionary<string, ushort[][]> _previousDataDict = new Dictionary<string, ushort[][]>();
    private readonly Dictionary<string, OCSPlatform[]> _platformDict = new Dictionary<string, OCSPlatform[]>();

    private readonly Action<int, int, string> _sendToTaskDCU;
    private readonly Dictionary<string, ClientModbusConfig> _clients;

    private CancellationTokenSource _tokenSource;

    public OCSClientPoller(Dictionary<string, ClientModbusConfig> clients, Action<int, int, string> sendToTaskDCU)
    {
        _clients = clients;
        _sendToTaskDCU = sendToTaskDCU;
    }

    public class ClientModbusConfig
    {
        public string IP { get; set; }
        public int Port { get; set; } = 502;
        public byte SlaveId { get; set; } = 1;
        public List<ushort> StartAddresses { get; set; }
    }

    public void StartPollingAllClients()
    {
        if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
        {
            ASI.Lib.Log.ErrorLog.Log(_procName, "StartPollingAllClients 已在執行中，忽略重複呼叫");
            return;
        }

        _tokenSource = new CancellationTokenSource();
        foreach (var client in _clients.Keys)
        {
            string clientName = client;
            Task.Factory.StartNew(() => PollingLoop(clientName, _tokenSource.Token), TaskCreationOptions.LongRunning);
        }
    }

    public void Stop()
    {
        _tokenSource?.Cancel();
    }

    // ── 1. 連線與輪詢 ────────────────────────────────────────────
    private void PollingLoop(string clientName, CancellationToken token)
    {
        var clientConfig = _clients[clientName];

        _previousDataDict[clientName] = new ushort[clientConfig.StartAddresses.Count][];
        _platformDict[clientName] = Enumerable.Range(0, clientConfig.StartAddresses.Count)
                                              .Select(_ => new OCSPlatform())
                                              .ToArray();

        while (!token.IsCancellationRequested)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            try
            {
                using (var master = new ModbusTcpClient())
                {
                    master.ReadTimeout = 1000;
                    master.Connect(clientConfig.IP, clientConfig.Port);

                    for (int groupIndex = 0; groupIndex < clientConfig.StartAddresses.Count; groupIndex++)
                    {
                        ushort startAddress = clientConfig.StartAddresses[groupIndex];
                        ushort[] currentData = master.ReadInputRegisters(clientConfig.SlaveId, startAddress, 38);

                        ASI.Lib.Log.DebugLog.Log(_procName, $"[{clientName}] 讀取地址:{startAddress}, 前3值:[{currentData[0]}, {currentData[1]}, {currentData[2]}]");

                        ProcessRegisterData(clientName, groupIndex, startAddress, currentData);

                        token.WaitHandle.WaitOne(20); // 可被取消中斷的短暫等待
                    }
                }
            }
            catch (TimeoutException timeoutEx)
            {
                // Connect() 逾時（5秒內無回應）
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} 連線逾時: {timeoutEx.Message}，嘗試重新連線...");
                TryReconnect(token);
            }
            catch (System.Net.Sockets.SocketException sockEx)
            {
                // TcpClient.Connect() 失敗時拋出 SocketException（非 IOException）
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} Socket 連線失敗: {sockEx.Message}，嘗試重新連線...");
                TryReconnect(token);
            }
            catch (IOException ioEx)
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} IO 錯誤: {ioEx.Message}，嘗試重新連線...");
                TryReconnect(token);
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log(_procName, $"{timestamp} 錯誤: {ex}");
            }

            token.WaitHandle.WaitOne(_pollingIntervalMs); // 可被取消中斷的輪詢等待
        }
    }

    // ── 2. 變更偵測 ──────────────────────────────────────────────
    private void ProcessRegisterData(string clientName, int groupIndex, ushort startAddress, ushort[] currentData)
    {
        var previousGroupData = _previousDataDict[clientName];

        if (previousGroupData[groupIndex] != null &&
            AreArraysEqual(previousGroupData[groupIndex], currentData))
        {
            return; // 資料未變更，略過
        }

        ASI.Lib.Log.DebugLog.Log(_procName, $"內容變動：client={clientName}, GroupIndex={groupIndex}");
        ASI.Lib.Log.DebugLog.Log(_procName, $"舊資料: {string.Join(",", previousGroupData[groupIndex] ?? new ushort[0])}");
        ASI.Lib.Log.DebugLog.Log(_procName, $"新資料: {string.Join(",", currentData)}");

        previousGroupData[groupIndex] = (ushort[])currentData.Clone();

        var platform = _platformDict[clientName][groupIndex];
        platform.UpdateFromUShortArray(currentData);
        ASI.Lib.Log.DebugLog.Log(_procName, $"解析結果:\n{platform.ToLogString()}");

        byte[] byteArray = ToByteArray(currentData);
        int special1 = DetermineTrainStatus(byteArray, trainIndex: 1);
        int special2 = DetermineTrainStatus(byteArray, trainIndex: 2);

        SendUpdate(platform, startAddress, special1, special2);
    }

    // ── 3. 組裝訊息 ──────────────────────────────────────────────
    private TrainMSG BuildTrainMessage(OCSPlatform platform, ushort startAddress, int special1, int special2)
    {
        return new TrainMSG(ASI.Wanda.DMD.Enum.Station.OCC)
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
    }

    // ── 4. 送出（MSMQ + DB）────────────────────────────────────
    private void SendUpdate(OCSPlatform platform, ushort startAddress, int special1, int special2)
    {
        var msg = BuildTrainMessage(platform, startAddress, special1, special2);
        var jsonString = ASI.Lib.Text.Parsing.Json.SerializeObject(msg);

        ASI.Lib.Log.DebugLog.Log(_procName, $"送出更新: platform_id={platform.PlatformID}, startAddr={startAddress}");
        _sendToTaskDCU(2, 0, jsonString);

        // 寫入資料庫（等 OCS_Data 資料表建立後啟用）
        // UpdateOCSData(platform);
    }

    // ── 工具方法 ─────────────────────────────────────────────────
    private static byte[] ToByteArray(ushort[] data)
    {
        byte[] byteArray = new byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            byteArray[i * 2]     = (byte)(data[i] >> 8);
            byteArray[i * 2 + 1] = (byte)(data[i] & 0xFF);
        }
        return byteArray;
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

    /// <summary>
    /// 判斷列車異常狀態。trainIndex=1 檢查 Train1 範圍，trainIndex=2 檢查 Train2 範圍。
    /// </summary>
    private int DetermineTrainStatus(byte[] data, int trainIndex)
    {
        // byte layout: Header(0–11) + Journey1(12–43) + Journey2(44–75)
        //
        // offset 是用來讓 (offset + index) 對應到正確的絕對 byte 位置：
        //   Train1: offset=0，data[0+17]=data[17]=ServiceNumber1高位元組 等
        //   Train2: offset=32，data[32+17]=data[49]=ServiceNumber2高位元組 等
        //
        // 原始 Train2 offset=38 是錯誤的，會指向 ArrivalTime2/DepartureTime2/TestTrain2 等不相關欄位
        int offset = (trainIndex == 2) ? 32 : 0;

        if (data.Length < offset + 37) return 0;

        if (data[offset + 17] != 0 || data[offset + 19] != 0 || data[offset + 20] != 0 ||
            data[offset + 33] != 36 || data[offset + 35] != 0 || data[offset + 36] != 0)
        {
            return 1;
        }

        return 0;
    }

    private void TryReconnect(CancellationToken token)
    {
        // IO 錯誤後等待一段時間，下一輪 PollingLoop 迭代會自動重新建立連線
        const int delayBeforeRetryMs = 3000;
        ASI.Lib.Log.ErrorLog.Log(_procName, $"等待 {delayBeforeRetryMs}ms 後重新連線...");
        token.WaitHandle.WaitOne(delayBeforeRetryMs); // 可被取消中斷
    }

    /// <summary>
    /// 將 OCSPlatform 資料寫入資料庫（自動判斷 Insert 或 Update）
    /// </summary>
    private void UpdateOCSData(OCS.Modbus.OCSPlatform platform)
    {
        try
        {
            var ocsData = new ASI.Wanda.DMD.DB.Models.Train.OCS_Data
            {
                number_of_platforms = platform.NumberOfPlatforms,
                platform_id = platform.PlatformID,
                arrival = platform.Arrival,
                departure = platform.Departure,
                skip_hold = platform.Skip | (platform.Hold << 8),
                number_of_journey_data = platform.NumberOfJourneyData,

                validity_field = platform.ValidityField1,
                train_unit_id = platform.TrainUnitID1,
                service_number = platform.ServiceNumber1,
                trip_number = platform.TripNumber1,
                destination_number = platform.DestinationNumber1,
                arrivaltime = (int)platform.ArrivalTime1,
                departuretime = (int)platform.DepartureTime1,
                delayatarrival = platform.DelayAtArrival1,
                delayatdeparture = platform.DelayAtDeparture1,
                cancelledtrain = platform.CancelledTrain1,
                trainend_of_service = platform.TrainEndOfService1,
                lasttrainoftheoperatingday = platform.LastTrainOfTheOperatingDay1,
                line_operation_mode = platform.LineOperationMode1,
                train_direction = platform.TrainDirection1,

                validity_field2 = platform.ValidityField2,
                train_unit_id2 = platform.TrainUnitID2,
                service_number2 = platform.ServiceNumber2,
                trip_number2 = platform.TripNumber2,
                destination_number2 = platform.DestinationNumber2,
                arrivaltime2 = (int)platform.ArrivalTime2,
                departuretime2 = (int)platform.DepartureTime2,
                delayatarrival2 = platform.DelayAtArrival2,
                delayatdeparture2 = platform.DelayAtDeparture2,
                cancelledtrain2 = platform.CancelledTrain2,
                trainend_of_service2 = platform.TrainEndOfService2,
                lasttrainoftheoperatingday2 = platform.LastTrainOfTheOperatingDay2,
                line_operation_mode2 = platform.LineOperationMode2,
                train_direction2 = platform.TrainDirection2
            };

            int affectedRows = ASI.Wanda.DMD.DB.Tables.Train.ocsData.InsertOrUpdateOCSData(ocsData);

            if (affectedRows > 0)
                ASI.Lib.Log.DebugLog.Log(_procName, $"成功寫入 OCS_Data，platform_id = {platform.PlatformID}，影響行數 = {affectedRows}");
            else
                ASI.Lib.Log.ErrorLog.Log(_procName, $"寫入 OCS_Data 失敗，platform_id = {platform.PlatformID}，影響行數 = 0");
        }
        catch (Exception ex)
        {
            ASI.Lib.Log.ErrorLog.Log(_procName, $"寫入 OCS_Data 異常: {ex.Message}");
            ASI.Lib.Log.ErrorLog.Log(_procName, ex);
        }
    }
}
