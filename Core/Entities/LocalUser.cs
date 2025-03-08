using Microsoft.AspNetCore.Identity;

namespace DiscordCloneBackend.Core.Entities
{
    public class LocalUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastPasswordResetRequested { get; set; }
        public DateTime? LastVerificationEmailSent { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
