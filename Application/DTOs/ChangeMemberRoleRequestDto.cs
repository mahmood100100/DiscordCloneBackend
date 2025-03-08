using DiscordCloneBackend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ChangeMemberRoleRequestDto
    {
        [Required(ErrorMessage = "Requester profile Id is required.")]
        public string RequesterProfileId { get; set; }
        [Required(ErrorMessage = "Target Member Id is required.")]
        public string TargetMemberId { get; set; }
        [Required(ErrorMessage = "Member Role is required.")]
        public int NewRole { get; set; }
        [Required(ErrorMessage = "Server Id is required.")]
        public string ServerId { get; set; }
    }
}
