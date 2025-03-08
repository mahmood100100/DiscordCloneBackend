using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ConversationResponseDTO
    {
        public string Id { get; set; }
        public string MemberOneId { get; set; }
        public string MemberTwoId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<DirectMessage> DirectMessages { get; set; }
    }
}
