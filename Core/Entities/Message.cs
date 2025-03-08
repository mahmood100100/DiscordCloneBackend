namespace DiscordCloneBackend.Core.Entities
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public string MemberId { get; set; }
        public string ChannelId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool Deleted { get; set; } = false;

        // Navigation Properties
        public Member Member { get; set; }
        public Channel Channel { get; set; }
    }
}
