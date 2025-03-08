using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class ServerRepository : GenericRepository<Server>, IServerRepository
    {
        private readonly AppDbContext appDbContext;

        public ServerRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<Server> GetServerByInviteCodeAsync(string inviteCode)
        {
            var server = await appDbContext.Servers.FirstOrDefaultAsync(s => s.InviteCode == inviteCode);
            if (server == null)
            {
                throw new KeyNotFoundException($"Server with inviteCode {inviteCode} not found");
            } else
            {
                return server;
            }
        }

        public async Task<IEnumerable<Server>> GetServersByProfileIdAsync(string profileId)
        {
            return await appDbContext.Servers
                .Where(s => s.Members.Any(m => m.ProfileId == profileId))
                .Include(s => s.Members)
                   .ThenInclude(m => m.Profile)
                .Include(s => s.Channels)
                .ToListAsync();
        }

        public async Task<Server> GetServerByIdIncludingMembersAndChannelsAsync(string serverId)
        {
            return await appDbContext.Servers
                .Where(s => s.Id == serverId)
                .Include(s => s.Channels)
                .Include(s => s.Members)
                    .ThenInclude(m => m.Profile)
                .Select(s => new Server
                {
                    Id = s.Id,
                    Name = s.Name,
                    ImageUrl = s.ImageUrl,
                    InviteCode = s.InviteCode,
                    ProfileId = s.ProfileId,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    Channels = s.Channels.ToList(),
                    Members = s.Members
                        .OrderByDescending(m => m.Role == MemberRole.Admin)
                        .ThenByDescending(m => m.Role == MemberRole.Moderator)
                        .ThenBy(m => m.Role == MemberRole.Guest)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Server> GetServerByInviteCodeAndProfileIdAsync(string inviteCode, string profileId)
        {
            var server = await appDbContext.Servers
                .Where(s => s.InviteCode == inviteCode)
                .Include(s => s.Channels)
                .Include(s => s.Members)
                    .ThenInclude(m => m.Profile)
                .FirstOrDefaultAsync();

            if (server == null)
            {
                return null;
            }

            bool isMember = server.Members.Any(m => m.ProfileId == profileId);
            if (!isMember)
            {
                return null;
            }

            return server;
        }
    }
}
