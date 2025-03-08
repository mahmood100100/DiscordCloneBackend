namespace DiscordCloneBackend.Application.DTOs
{
    public class MessageResponseDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public string MemberId { get; set; }
        public string ChannelId { get; set; }
        public string MemberProfileName { get; set; }
        public string MemberProfileImageUrl { get; set; }
        public string ChannelName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; }
    }
}
