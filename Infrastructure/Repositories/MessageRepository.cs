using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public MessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IEnumerable<Message>> GetMessagesByChannelIdAsync(
            string channelId,
            int page = 1,
            int pageSize = 10)
        {
            var messages = await appDbContext.Messages
                .Where(m => m.ChannelId == channelId)
                .Include(m => m.Channel)
                .Include(m => m.Member)
                .ThenInclude(m => m.Profile)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return messages;
        }

        public async Task<int> GetMessagesCountByChannelIdAsync(string channelId)
        {
            return await appDbContext.Messages
                .CountAsync(m => m.ChannelId == channelId && !m.Deleted);
        }

        public async Task<IEnumerable<Message>> GetMessagesByMemberIdAsync(string memberId)
        {
            var messages = await appDbContext.Messages.Where(m => m.MemberId == memberId && !m.Deleted).ToListAsync();
            return messages;
        }
    }
}
