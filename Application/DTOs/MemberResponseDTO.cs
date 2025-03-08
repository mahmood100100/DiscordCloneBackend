using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;

namespace DiscordCloneBackend.Application.DTOs
{
    public class MemberResponseDTO
    {
        public string Id { get; set; }
        public MemberRole Role { get; set; }
        public string ProfileId { get; set; }
        public string ServerId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileEmail { get; set; }
        public string ServerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Conversation> ConversationsInitiated { get; set; } = new List<Conversation>();
        public ICollection<Conversation> ConversationsReceived { get; set; } = new List<Conversation>();
        public ICollection<DirectMessage> DirectMessages { get; set; } = new List<DirectMessage>();
    }
}
