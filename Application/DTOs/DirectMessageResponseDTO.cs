using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.DTOs
{
    public class DirectMessageResponseDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public string SenderMemberId { get; set; }
        public string ReceiverMemberId { get; set; }
        public string ConversationId { get; set; }
        public string SenderMemberProfileName { get; set; }
        public string ReceiverMemberProfileName { get; set; }
        public string SenderMemberProfileImageUrl { get; set; }
        public string ReceiverMemberProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; } = false;

    }
}
