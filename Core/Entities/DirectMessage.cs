using System.Text.Json.Serialization;

namespace DiscordCloneBackend.Core.Entities
{
    public class DirectMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public string SenderMemberId { get; set; }
        public string ReceiverMemberId { get; set; }
        public string ConversationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool Deleted { get; set; } = false;

        // Navigation Properties
        public Member Sender { get; set; }
        public Member Receiver { get; set; }
        [JsonIgnore] 
        public Conversation Conversation { get; set; }
    }
}
