using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IDirectMessageRepository : IGenericRepository<DirectMessage>
    {
        Task<IEnumerable<DirectMessage>> GetDirectMessagesByConversationIdAsync(string conversationId, int page = 1, int pageSize = 10);
        Task<IEnumerable<DirectMessage>> GetDirectMessagesByMemberIdAsync(string memberId);
        Task<IEnumerable<DirectMessage>> GetDirectMessagesBySenderMemberIdAsync(string memberId);
        Task<IEnumerable<DirectMessage>> GetDirectMessagesByReceiverMemberIdAsync(string memberId);
    }
}
