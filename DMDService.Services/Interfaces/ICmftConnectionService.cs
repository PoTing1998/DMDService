using System;

namespace DMDService.Services.Interfaces
{
    public interface ICmftConnectionService
    {
        event Action<string> LogMessage;
        event Action<ASI.Wanda.CMFT.Message.Message> MessageReceived;

        bool IsConnected { get; }
        int Connect(string ip, string port, string type);
        void Disconnect();
        int Send(ASI.Wanda.CMFT.Message.Message message);
    }
}
