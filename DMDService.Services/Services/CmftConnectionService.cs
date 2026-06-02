using System;
using DMDService.Services.Interfaces;

namespace DMDService.Services.Services
{
    public class CmftConnectionService : ICmftConnectionService
    {
        private ASI.Wanda.CMFT.CMFT_API _cmftApi;

        public event Action<string> LogMessage;
        public event Action<ASI.Wanda.CMFT.Message.Message> MessageReceived;

        public bool IsConnected { get; private set; }

        public int Connect(string ip, string port, string type)
        {
            try
            {
                if (_cmftApi != null)
                {
                    _cmftApi.ReceivedEvent -= OnReceivedEvent;
                    _cmftApi.Dispose();
                }

                string connString = $"IP={ip};Port={port};Type={type}";
                _cmftApi = new ASI.Wanda.CMFT.CMFT_API();
                _cmftApi.ReceivedEvent += OnReceivedEvent;

                int result = _cmftApi.Initial(connString, "CMFT");

                if (result == 0)
                {
                    IsConnected = true;
                    RaiseLog($"✓ CMFT 連線成功: {connString}");
                }
                else
                {
                    string message = GetConnectionResultMessage(result);
                    RaiseLog($"✗ CMFT 連線失敗: {message} (錯誤碼: {result})");
                }

                return result;
            }
            catch (Exception ex)
            {
                RaiseLog($"CMFT 連線錯誤: {ex.Message}");
                return -1;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_cmftApi != null)
                {
                    _cmftApi.ReceivedEvent -= OnReceivedEvent;
                    _cmftApi.Dispose();
                    _cmftApi = null;
                }

                IsConnected = false;
                RaiseLog("✓ CMFT 已斷開連線");
            }
            catch (Exception ex)
            {
                RaiseLog($"CMFT 斷線錯誤: {ex.Message}");
            }
        }

        public int Send(ASI.Wanda.CMFT.Message.Message message)
        {
            if (_cmftApi == null || !IsConnected)
                return -3;

            return _cmftApi.Send(message);
        }

        private void OnReceivedEvent(ASI.Wanda.CMFT.Message.Message message)
        {
            MessageReceived?.Invoke(message);
        }

        private string GetConnectionResultMessage(int result)
        {
            switch (result)
            {
                case 0: return "成功開啟";
                case -1: return "例外錯誤";
                case -2: return "未成功開啟";
                case -3: return "剖析連線字串發生錯誤";
                case -4: return "初始化 Socket 相關屬性發生錯誤";
                case -5: return "關閉所有 Sockets 時發生錯誤";
                case -6: return "Socket server 無法正常繫結通訊埠";
                default: return "未知的錯誤";
            }
        }

        private void RaiseLog(string message)
        {
            LogMessage?.Invoke(message);
        }
    }
}
