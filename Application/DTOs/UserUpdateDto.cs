namespace DiscordCloneBackend.Application.DTOs
{
    public class UserUpdateDto
    {
        public string UserId { get; set; }
        public IFormFile? Image { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }
}
