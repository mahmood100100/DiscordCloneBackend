using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IProfileService : IGenericService<Profile ,
        ProfileRequestDTO , ProfileResponseDTO>
    {
        Task<ProfileResponseDTO> GetProfileByUserIdAsync(string id);
        Task CreateProfileAsync(UserRequestDTO request, string userId);
        Task DeleteProfileByUserIdAsync(string userId);
        Task<ProfileResponseDTO> GetProfileByEmailAsync(string email);
        Task<IEnumerable<ProfileResponseDTO>> GetProfilesByRoleAsync(string role);
    }
}
