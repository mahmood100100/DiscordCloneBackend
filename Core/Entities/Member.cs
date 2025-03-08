using DiscordCloneBackend.Core.Enums;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace DiscordCloneBackend.Core.Entities
{
    public class Member
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public MemberRole Role { get; set; } // Enum for roles
        public string ProfileId { get; set; } // Foreign Key to Profile
        public string ServerId { get; set; } // Foreign Key to Server
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool Deleted { get; set; } = false;

        // Navigation Properties
        [JsonIgnore]
        public Profile Profile { get; set; }
        [JsonIgnore]
        public Server Server { get; set; }
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        [JsonIgnore]
        public ICollection<Conversation> ConversationsInitiated { get; set; } = new List<Conversation>();
        [JsonIgnore]
        public ICollection<Conversation> ConversationsReceived { get; set; } = new List<Conversation>();
        [JsonIgnore]
        public ICollection<DirectMessage> SentDirectMessages { get; set; } = new List<DirectMessage>();
        [JsonIgnore]
        public ICollection<DirectMessage> ReceivedDirectMessages { get; set; } = new List<DirectMessage>();
    }
}
