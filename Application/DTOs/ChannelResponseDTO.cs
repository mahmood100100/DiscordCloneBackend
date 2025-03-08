using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ChannelResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ChannelType Type { get; set; }
        public string ProfileId { get; set; }
        public string ServerId { get; set; }
        public string ProfileName { get; set; }
        public string ServerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
