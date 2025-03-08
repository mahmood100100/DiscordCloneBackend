using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation> GetConversationByMembersAsync(string memberOneId, string memberTwoId, int directMessagesLimit);
        Task<IEnumerable<Conversation>> GetConversationsByMemberIdAsync(string memberId);
    }
}
