using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        private readonly AppDbContext appDbContext;

        public MemberRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<IEnumerable<Member>> GetMembersByRoleAsync(MemberRole role)
        {
            var members = await appDbContext.Members.Where(m => m.Role == role).ToListAsync();
            return members;
        }

        public async Task<IEnumerable<Member>> GetMembersByServerIdAsync(string serverId)
        {
            var members = await appDbContext.Members.Where(m => m.ServerId == serverId).ToListAsync();
            return members;
        }

        public async Task<Member> GetMemberByProfileIdAsync(string profileId)
        {
            return await appDbContext.Members
                .FirstOrDefaultAsync(m => m.ProfileId == profileId);
        }

        public async Task<Member> GetMemberByProfileIdAndServerIdAsync(string profileId, string serverId)
        {
            return await appDbContext.Members
                .FirstOrDefaultAsync(m => m.ProfileId == profileId && m.ServerId == serverId);
        }

        public async Task<int> CountAdminMembersAsync(string serverId)
        {
            return await appDbContext.Members.CountAsync(m => m.ServerId == serverId && m.Role == MemberRole.Admin);
        }

    }
}
