namespace DiscordCloneBackend.Application.DTOs
{
    public class UserResponseDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
        public ProfileResponseDTO Profile { get; set; }
    }
}
