namespace DiscordCloneBackend.Application.DTOs
{
    public class ServerRequestDTO
    {
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public string ProfileId { get; set; }
    }
}
