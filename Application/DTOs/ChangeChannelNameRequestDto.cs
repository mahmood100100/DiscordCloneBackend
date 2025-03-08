using DiscordCloneBackend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ChangeChannelNameRequestDto
    {
        [Required(ErrorMessage = "Channel Id is required.")]
        public string ChannelId { get; set; }
        [Required(ErrorMessage = "Requester Proifle Id is required.")]
        public string RequesterProfileId { get; set; }
        [Required(ErrorMessage = "Server Id is required.")]
        public string ServerId { get; set; }
        [Required(ErrorMessage = "Channel Type is required.")]
        public string ChannelName { get; set; }
    }
}
