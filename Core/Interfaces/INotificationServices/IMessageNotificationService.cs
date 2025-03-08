using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IMessageNotificationService
    {
        Task NotifyMessageUpdate(string serverId, string channelId, MessageResponseDTO updatedMessage);
        Task NotifyMessageSoftDeleted(string serverId, string channelId, string messageId);
        Task NotifyMessageHardDeleted(string serverId, string channelId, string deletedMessageId);
        Task NotifyMessageAdded(string serverId, string channelId, MessageResponseDTO addedMessage);
    }
}
