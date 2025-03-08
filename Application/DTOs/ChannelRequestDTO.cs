using AutoMapper.Execution;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ChannelRequestDTO
    {

        [Required(ErrorMessage = "Channel requester profile id is required.")]
        public string RequesterProfileId { get; set; }

        [Required(ErrorMessage = "Channel name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Channel type is required.")]
        public ChannelType Type { get; set; }

        [Required(ErrorMessage = "Server ID is required.")]
        public string ServerId { get; set; }
    }
}
