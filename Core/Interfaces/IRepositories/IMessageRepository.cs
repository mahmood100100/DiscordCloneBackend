using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByChannelIdAsync(string channelId, int page = 1, int pageSize = 10);
        Task<IEnumerable<Message>> GetMessagesByMemberIdAsync(string memberId);
        Task<int> GetMessagesCountByChannelIdAsync(string channelId);
    }
}
