using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IDirectMessageNotificationService
    {
        Task NotifyDirectMessageUpdate(string conversationId, DirectMessageResponseDTO updatedDirectMessageMessage);
        Task NotifyDirectMessageSoftDeleted(string conversationId, string directMessageId);
        Task NotifyDirectMessageHardDeleted(string conversationId, DirectMessageResponseDTO deletedDirectMessage);
        Task NotifyDirectMessageAdded(string conversationId, DirectMessageResponseDTO addedDirectMessage);
    }
}
