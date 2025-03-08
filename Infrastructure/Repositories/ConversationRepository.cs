using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        private readonly AppDbContext appDbContext;

        public ConversationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Conversation> GetConversationByMembersAsync(string memberOneId, string memberTwoId, int directMessagesLimit = 15)
        {
            if (string.IsNullOrEmpty(memberOneId) || string.IsNullOrEmpty(memberTwoId))
            {
                throw new ArgumentException("Member IDs cannot be null or empty.");
            }

            var conversation = await appDbContext.Conversations
                .Where(c => (c.MemberOneId == memberOneId && c.MemberTwoId == memberTwoId) ||
                            (c.MemberOneId == memberTwoId && c.MemberTwoId == memberOneId))
                .Include(c => c.DirectMessages
                    .OrderByDescending(dm => dm.CreatedAt)
                    .Take(directMessagesLimit))
                .Include(c => c.MemberOne)
                .Include(c => c.MemberTwo)
                .FirstOrDefaultAsync();

            return conversation;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByMemberIdAsync(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                throw new ArgumentException("Member ID cannot be null or empty.", nameof(memberId));
            }

            var conversations = await appDbContext.Conversations
                .Where(c => c.MemberOneId == memberId || c.MemberTwoId == memberId)
                .Include(c => c.DirectMessages)
                .Include(c => c.MemberOne)
                .Include(c => c.MemberTwo)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return conversations;
        }
    }
}