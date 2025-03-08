using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class DirectMessageRepository : GenericRepository<DirectMessage>, IDirectMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public DirectMessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IEnumerable<DirectMessage>> GetDirectMessagesByConversationIdAsync(string conversationId , int page = 1 ,int pageSize = 10)
        {
            var directMessages = await appDbContext.DirectMessages
                .Where(d => d.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return directMessages;
        }

        public async Task<IEnumerable<DirectMessage>> GetDirectMessagesByMemberIdAsync(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                throw new ArgumentException("Member ID cannot be null or empty.", nameof(memberId));
            }

            var directMessages = await appDbContext.DirectMessages
                .Where(d => (d.SenderMemberId == memberId || d.ReceiverMemberId == memberId) && !d.Deleted)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            return directMessages;
        }

        public async Task<IEnumerable<DirectMessage>> GetDirectMessagesBySenderMemberIdAsync(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                throw new ArgumentException("Member ID cannot be null or empty.", nameof(memberId));
            }

            var directMessages = await appDbContext.DirectMessages
                .Where(d => (d.SenderMemberId == memberId))
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            return directMessages;
        }

        public async Task<IEnumerable<DirectMessage>> GetDirectMessagesByReceiverMemberIdAsync(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                throw new ArgumentException("Member ID cannot be null or empty.", nameof(memberId));
            }

            var directMessages = await appDbContext.DirectMessages
                .Where(d => (d.ReceiverMemberId == memberId))
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            return directMessages;
        }
    }
}
