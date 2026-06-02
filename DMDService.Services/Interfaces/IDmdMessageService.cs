using System;
using System.Collections.Generic;
using ASI.Wanda.DMD.DB.Models;
using DMDService.Services.Models;

namespace DMDService.Services.Interfaces
{
    public interface IDmdMessageService
    {
        event Action<string> LogMessage;

        bool InitializeDatabase(string ip, string port, string dbName, string userId, string password);

        List<dmd_pre_record_message> GetPreRecordMessages();
        List<dmd_instant_message> GetInstantMessages();
        List<TargetDevice> GetTargetDevices(string stationFilter);

        dmd_instant_message GetInstantMessage(string messageId);
        bool SaveInstantMessage(dmd_instant_message message);

        SendResult SendPreRecordMessage(string seatId, string station, string messageId,
            List<string> targetDuList, int priority, int moveSpeed, int moveMode);

        SendResult SendInstantMessage(string seatId, string station, string messageId,
            List<string> targetDuList, int priority, int moveSpeed, int moveMode);

        int Connect(string ip, string port, string type);
        void Disconnect();
        bool IsConnected { get; }
    }
}
