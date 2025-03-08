using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<(bool IsUnique, string[] TakenFields)> CheckUserValidityAsync(string? username = null, string? email = null);
        Task<IEnumerable<LocalUser>> GetAllUsersAsync();
        Task<LocalUser> GetUserByIdAsync(string id);
        Task<LocalUser> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(LocalUser user, string password);
        Task<IdentityResult> AddToRoleAsync(LocalUser user, string role);
        Task<bool> CheckPasswordSignInAsync(LocalUser user, string password);
        Task<IList<string>> GetRolesAsync(LocalUser user);
        Task<IList<string>> GetRolesAsync(string userId);
        Task<string> GeneratePasswordResetTokenAsync(LocalUser user);
        Task<IdentityResult> ResetPasswordAsync(LocalUser user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(LocalUser user, string currentPassword, string newPassword);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(LocalUser user);
        Task RemoveFromAllRolesAsync(LocalUser user);
        Task<bool> RoleExistsAsync(string roleName);
        Task<LocalUser> GetUserByRefreshTokenAsync(string refreshToken);
        Task<string> GenerateEmailConfirmationTokenAsync(LocalUser user);
        Task<bool> ConfirmEmailAsync(LocalUser user, string token);
    }
}