namespace DiscordCloneBackend.Application.DTOs
{
    public class ChangePasswordDTO
    {
        public string Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
