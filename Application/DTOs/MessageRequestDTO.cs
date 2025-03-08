using System.ComponentModel.DataAnnotations;

namespace DiscordCloneBackend.Application.DTOs
{
    public class MessageRequestDTO
    {
        [Required]
        public string Content { get; set; }

        public IFormFile? File { get; set; }

        [Required]
        public string ChannelId { get; set; }

        [Required]
        public string MemberId { get; set; }

        [Required]
        public string ServerId { get; set; }

    }
}
