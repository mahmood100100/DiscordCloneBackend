namespace DiscordCloneBackend.Application.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmedNewPassword { get; set; }
        public string Token { get; set; }
    }
}
