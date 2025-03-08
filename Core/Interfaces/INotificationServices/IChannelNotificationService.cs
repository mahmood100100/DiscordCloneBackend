using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IChannelNotificationService
    {
        Task NotifyChannelDeleted(string serverId, string deletedChannelId);
        Task NotifyChannelAdded(string serverId, ChannelResponseDTO addedChannel);
        Task NotifyChannelUpdated(string serverId, ChannelResponseDTO updatedChannel);
    }
}
