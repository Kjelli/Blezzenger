using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blezzenger.Models
{
    public class Message
    {
        public Guid Id { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public Guid SenderId { get; private set; }
        public string SenderName { get; private set; }
        public string Content { get; private set; }

        [NotMapped]
        public bool IsConsecutive { get; set; }
        [NotMapped]
        public bool IsSentByMe { get; set; }
        [NotMapped]
        public bool IsSystemMessage { get; set; }

        public Message(Guid senderId, string senderName, string content)
        {
            Timestamp = DateTimeOffset.UtcNow;
            SenderId = senderId;
            SenderName = senderName;
            Content = content;
        }
    }
}
