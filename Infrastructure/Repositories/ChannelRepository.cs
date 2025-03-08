using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class ChannelRepository : GenericRepository<Channel>, IChannelRepository
    {
        private readonly AppDbContext appDbContext;

        public ChannelRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Channel?> GetByServerIdAndNameAsync(string serverId, string name)
        {
            return await appDbContext.Channels
                .FirstOrDefaultAsync(c => c.ServerId == serverId && c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Channel>> GetChannelsByServerIdAsync(string serverId)
        {
            var channels =  await appDbContext.Channels
                                     .Where(c => c.ServerId == serverId)
                                     .ToListAsync();
            return channels;
        }

        public async Task<IEnumerable<Channel>> GetChannelsByTypeAsync(ChannelType type)
        {
            var channels = await appDbContext.Channels
                                     .Where(c => c.Type == type)
                                     .ToListAsync();
            return channels;
        }
    }
}
