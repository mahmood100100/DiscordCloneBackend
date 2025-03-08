using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class UserNotificationService : IUserNotificationService
    {
        public Task NotifyUserUpdated(string serverId, UserResponseDTO updatedUser)
        {
            throw new NotImplementedException();
        }
    }
}
