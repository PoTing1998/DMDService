using System;

namespace DMDService.Services.Interfaces
{
    public interface IDmdConnectionService
    {
        event Action<string> LogMessage;
        event Action<ASI.Wanda.DMD.Message.Message> MessageReceived;

        bool IsConnected { get; }
        int Connect(string ip, string port, string type);
        void Disconnect();
        int Send(ASI.Wanda.DMD.Message.Message message);
    }
}
