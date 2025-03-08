using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        Task<Profile> GetProfileByUserIdAsync(string userId);
        Task<Profile> GetProfileByEmailAsync(string email);
        Task<IEnumerable<Profile>> GetProfilesByRoleAsync(string role);
        Task DeleteProfileAsync(string userId);
    }
}
