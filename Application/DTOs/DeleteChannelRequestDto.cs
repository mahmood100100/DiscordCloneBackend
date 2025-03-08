using DiscordCloneBackend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DiscordCloneBackend.Application.DTOs
{
    public class DeleteChannelRequestDto
    {
        [Required(ErrorMessage = "Channel Id is required.")]
        public string ChannelId { get; set; }
        [Required(ErrorMessage = "Server Id is required.")]
        public string ServerId { get; set; }

        [Required(ErrorMessage = "Channel Requester Member Id is required.")]
        public string RequesterProfileId { get; set; }
    }
}
