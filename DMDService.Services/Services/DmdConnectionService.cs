using System;
using DMDService.Services.Interfaces;

namespace DMDService.Services.Services
{
    public class DmdConnectionService : IDmdConnectionService
    {
        private ASI.Wanda.DMD.DMD_API mDMD_API;

        public event Action<string> LogMessage;
        public event Action<ASI.Wanda.DMD.Message.Message> MessageReceived;

        public bool IsConnected { get; private set; }

        public int Connect(string ip, string port, string type)
        {
            try
            {
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= OnReceivedEvent;
                    mDMD_API.Dispose();
                }

                string connString = $"IP={ip};Port={port};Type={type}";
                mDMD_API = new ASI.Wanda.DMD.DMD_API();
                mDMD_API.ReceivedEvent += OnReceivedEvent;

                int result = mDMD_API.Initial(connString);

                if (result == 0)
                {
                    IsConnected = true;
                    RaiseLog($"✓ 連線成功: {connString}");
                }
                else
                {
                    RaiseLog($"✗ 連線失敗，錯誤碼: {result}");
                }

                return result;
            }
            catch (Exception ex)
            {
                RaiseLog($"連線錯誤: {ex.Message}");
                return -1;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (mDMD_API != null)
                {
                    mDMD_API.ReceivedEvent -= OnReceivedEvent;
                    mDMD_API.Dispose();
                    mDMD_API = null;
                }

                IsConnected = false;
                RaiseLog("✓ 已斷開連線");
            }
            catch (Exception ex)
            {
                RaiseLog($"斷線錯誤: {ex.Message}");
            }
        }

        public int Send(ASI.Wanda.DMD.Message.Message message)
        {
            if (mDMD_API == null || !IsConnected)
                return -3;

            return mDMD_API.Send(message);
        }

        private void OnReceivedEvent(ASI.Wanda.DMD.Message.Message message)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string jsonData = message.JsonContent;
                string jsonObjectName = ASI.Lib.Text.Parsing.Json.GetValue(jsonData, "JsonObjectName");

                if (message.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Ack)
                {
                    RaiseLog($"[{timestamp}] 收到 ACK，訊息ID: {message.MessageID}");
                }
                else if (message.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Response)
                {
                    RaiseLog($"[{timestamp}] 收到回應: {jsonObjectName}");
                    RaiseLog($"內容: {jsonData}");
                }
                else if (message.MessageType == ASI.Wanda.DMD.Message.Message.eMessageType.Command)
                {
                    RaiseLog($"[{timestamp}] 收到命令: {jsonObjectName}");
                }

                MessageReceived?.Invoke(message);
            }
            catch (Exception ex)
            {
                RaiseLog($"接收訊息錯誤: {ex.Message}");
                ASI.Lib.Log.ErrorLog.Log("DmdConnectionService", ex);
            }
        }

        private void RaiseLog(string message)
        {
            LogMessage?.Invoke(message);
        }
    }
}
