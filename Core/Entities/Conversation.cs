using System.Text.Json.Serialization;

namespace DiscordCloneBackend.Core.Entities
{
    public class Conversation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MemberOneId { get; set; }
        public string MemberTwoId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [JsonIgnore]
        public Member MemberOne { get; set; }
        [JsonIgnore]
        public Member MemberTwo { get; set; }
        public ICollection<DirectMessage> DirectMessages { get; set; } = new List<DirectMessage>();
    }
}
