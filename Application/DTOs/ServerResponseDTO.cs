using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.DTOs
{
    public class ServerResponseDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string InviteCode { get; set; }
        public string ProfileId { get; set; }
        public string ProfileUserName { get; set; }
        public ICollection<MemberResponseDTO> Members { get; set; } = new List<MemberResponseDTO>();
        public ICollection<ChannelResponseDTO> Channels { get; set; } = new List<ChannelResponseDTO>();
    }
}
