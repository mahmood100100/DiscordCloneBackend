using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ProfileResponseDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public ICollection<ServerResponseDTO> Servers { get; set; } = new List<ServerResponseDTO>();
        public ICollection<MemberResponseDTO> Members { get; set; } = new List<MemberResponseDTO>();

        public ICollection<ChannelResponseDTO> Channels { get; set; } = new List<ChannelResponseDTO>();

    }
}
