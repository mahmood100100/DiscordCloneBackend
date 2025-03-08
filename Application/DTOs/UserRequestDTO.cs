namespace DiscordCloneBackend.Application.DTOs
{
    public class UserRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
