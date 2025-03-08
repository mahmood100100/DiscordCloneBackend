namespace DiscordCloneBackend.Application.DTOs
{
    public class ChangeMessageRequestDTO
    {
        public string MessageId { get; set; }
        public string Content { get; set; }
        public IFormFile? File { get; set; }
    }
}
