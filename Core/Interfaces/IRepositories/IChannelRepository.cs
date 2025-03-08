using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IChannelRepository : IGenericRepository<Channel>
    {
        Task<IEnumerable<Channel>> GetChannelsByServerIdAsync(string serverId);
        Task<IEnumerable<Channel>> GetChannelsByTypeAsync(ChannelType type);
        Task<Channel?>GetByServerIdAndNameAsync(string serverId, string name);
    }
}
