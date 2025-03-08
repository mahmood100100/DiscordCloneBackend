namespace DiscordCloneBackend.Application.DTOs
{
    public class DeleteMemberRequestDto
    {
        public string RequesterProfileId { get; set; }
        public string TargetMemberId { get; set; }
        public string ServerId { get; set; }
    }
}