namespace DMDService.Services.Models
{
    public class SendResult
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
        public string JsonContent { get; set; }

        public static SendResult Ok(int messageId, string jsonContent)
        {
            return new SendResult { Success = true, ErrorCode = 0, MessageId = messageId, JsonContent = jsonContent };
        }

        public static SendResult Fail(string message, int errorCode = -1)
        {
            return new SendResult { Success = false, ErrorCode = errorCode, Message = message };
        }
    }
}
