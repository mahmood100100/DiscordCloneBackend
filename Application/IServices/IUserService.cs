using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using System.Security.Claims;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> GetUserByIdAsync(string id);
        Task DeleteUserAsync(string userId);
        Task<UserResponseDTO> UpdateUserAsync(UserUpdateDto request);
        Task<(List<string> Errors, LocalUser User)> RegisterUserAsync(UserRequestDTO request);
        Task<LoginResponseDTO> LoginUserAsync(LoginRequestDTO request);
        Task ResetPasswordAsync(ResetPasswordDTO request);
        Task<ApiResponse> SendPasswordResetEmailAsync(SendForEmailDTO sendForEmailDTO);
        Task ChangePasswordAsync(ChangePasswordDTO request);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<(bool IsUnique, string[] TakenFields)> CheckUserValidityAsync(string? username = null, string? email = null);
        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);
        TimeSpan? CanRequestPasswordResetAsync(string email, DateTime? LastPasswordResetRequested);
        Task<ApiResponse> SendVerificationEmailAsync(string email, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(string email);
        Task<ApiResponse> VerifyEmailAsync(string email, string token);
    }
}
