using System;

namespace Blezzenger.Models
{
    public class MessageModel
    {
        public DateTimeOffset Timestamp { get; private set; }
        public string Message { get; private set; }
        public string Sender { get; private set; }
        public bool IsSystemMessage { get; private set; }
        public bool IsSentByMe { get; private set; }
        public bool IsConsecutiveFromSender { get; private set; }
        public MessageModel(string message, string sender, bool sentByMe, bool isSystemMessage = false, bool isConsecutiveFromSender = false)
        {
            Timestamp = DateTimeOffset.UtcNow;
            Message = message;
            Sender = sender;
            IsSentByMe = sentByMe;
            IsSystemMessage = isSystemMessage;
            IsConsecutiveFromSender = isConsecutiveFromSender;
        }
    }
}
