using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IUserNotificationService
    {
        Task NotifyUserUpdated(string serverId, UserResponseDTO updatedUser);
    }
}
