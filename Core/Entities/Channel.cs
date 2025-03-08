using DiscordCloneBackend.Core.Enums;
using System.Text.Json.Serialization;

namespace DiscordCloneBackend.Core.Entities
{
    public class Channel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public ChannelType Type { get; set; } // Enum for channel types
        public string ProfileId { get; set; } // Foreign Key to Profile
        public string ServerId { get; set; } // Foreign Key to Server
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [JsonIgnore]
        public Profile Profile { get; set; }
        [JsonIgnore]
        public Server Server { get; set; }
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
