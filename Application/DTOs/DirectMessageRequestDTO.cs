using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.DTOs
{
    public class DirectMessageRequestDTO
    {
        public string Content { get; set; }
        public IFormFile? File { get; set; }
        public string SenderMemberId { get; set; }
        public string ReceiverMemberId { get; set; }
        public string ConversationId { get; set; }
    }
}
