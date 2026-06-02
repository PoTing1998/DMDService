using System;
using System.Collections.Generic;
using DMDService.Services.Models;

namespace DMDService.Services.Interfaces
{
    /// <summary>
    /// CMFT 商業邏輯服務介面
    /// </summary>
    public interface ICmftMessageService
    {
        /// <summary>連線狀態日誌（HeartBeat、連線變化）</summary>
        event Action<string> LogConnectionState;

        /// <summary>傳送資料日誌</summary>
        event Action<string> LogSendData;

        /// <summary>回復資料日誌</summary>
        event Action<string> LogResponseData;

        /// <summary>接收資料日誌</summary>
        event Action<string> LogReceiveData;

        /// <summary>黑名單更新日誌</summary>
        event Action<string> LogBlackList;

        /// <summary>黑名單分隔線</summary>
        event Action<string> LogBlackListSeparator;

        // === 連線 ===
        int Connect(string ip, string port, string type);
        void Disconnect();
        bool IsConnected { get; }

        // === 資料庫 ===
        bool InitializeDatabases(string dmdIp, string dmdPort, string dmdDb, string dmdUser, string dmdPass,
                                  string cmftIp, string cmftPort, string cmftDb, string cmftUser, string cmftPass);
        List<EquipStatusInfo> GetEquipStatuses();
        List<TargetPanelInfo> GetTargetPanels();

        // === DB 同步 ===
        void SyncPlayList();
        void SyncPreRecordMessages();
        void InitializeCmftDatabase(string ip, string port, string db, string user, string pass);

        // === 設備狀態 ===
        void SendEquipStatus(string equipId, bool equipStatus);
        void SendOperationMode(string mode, string station);

        // === 定時器 ===
        void StartTimers();
        void StopTimers();
    }
}
