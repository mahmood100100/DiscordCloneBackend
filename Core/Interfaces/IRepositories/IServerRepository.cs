using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IServerRepository : IGenericRepository<Server>
    {
        Task<IEnumerable<Server>> GetServersByProfileIdAsync(string profileId);
        Task<Server> GetServerByInviteCodeAsync(string inviteCode);
        Task<Server> GetServerByIdIncludingMembersAndChannelsAsync(string serverId);
        Task<Server> GetServerByInviteCodeAndProfileIdAsync(string inviteCode, string profileId);
    }
}
