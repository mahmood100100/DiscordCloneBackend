using DiscordCloneBackend.Core.Entities;
using System.Security.Claims;

namespace DiscordCloneBackend.Core.Interfaces.IExternalServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(LocalUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<string> RefreshAccessToken(string refreshToken);
        ClaimsPrincipal? ValidateAccessToken(string token);
    }
}
