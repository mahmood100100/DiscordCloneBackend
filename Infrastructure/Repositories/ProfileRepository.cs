using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<LocalUser> userManager;

        public ProfileRepository(AppDbContext appDbContext , UserManager<LocalUser> userManager) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        public async Task<Profile> GetProfileByUserIdAsync(string userId)
        {
            var profile = await appDbContext.Profiles.Where(p => p.UserId == userId).FirstOrDefaultAsync();
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with userId {userId} not found");
            }
            else
            {
                return profile;
            }
        }

        public async Task<Profile> GetProfileByEmailAsync(string email)
        {
            var profile = await appDbContext.Profiles.Where(p => p.Email == email).FirstOrDefaultAsync();
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with email {email} not found");
            }
            else
            {
                return profile;
            }
        }

        public async Task<IEnumerable<Profile>> GetProfilesByRoleAsync(string role)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var profiles = await appDbContext.Profiles
                .Where(p => userIds.Contains(p.UserId))
                .ToListAsync();

            return profiles;
        }

        public async Task DeleteProfileAsync(string userId)
        {
            var profile = await appDbContext.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile != null)
            {
                appDbContext.Profiles.Remove(profile);
                await appDbContext.SaveChangesAsync();
            }
        }

    }
}
