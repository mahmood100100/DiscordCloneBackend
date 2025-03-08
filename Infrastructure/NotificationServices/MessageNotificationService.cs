using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class MessageNotificationService : IMessageNotificationService
    {
        private readonly IHubContext<MessageHub> hubContext;

        public MessageNotificationService(IHubContext<MessageHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        private static string GetChannelGroupName(string serverId, string channelId)
        {
            if (string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentException("ServerId and ChannelId cannot be null or empty.");
            }
            return $"Server_{serverId}_Channel_{channelId}";
        }

        public async Task NotifyMessageAdded(string serverId, string channelId, MessageResponseDTO addedMessage)
        {
            string groupName = GetChannelGroupName(serverId, channelId);
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveMessageAdded", new
            {
                serverId,
                channelId,
                message = addedMessage
            });
            Console.WriteLine($"Notified message added to group: {groupName}");
        }

        public async Task NotifyMessageHardDeleted(string serverId, string channelId, string deletedMessageId)
        {
            string groupName = GetChannelGroupName(serverId, channelId);
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveMessageDeleted", new
            {
                serverId,
                channelId,
                messageId = deletedMessageId
            });
            Console.WriteLine($"Notified hard delete to group: {groupName}");
        }

        public async Task NotifyMessageSoftDeleted(string serverId, string channelId, string messageId)
        {
            string groupName = GetChannelGroupName(serverId, channelId);
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveMessageSoftDeleted", new
            {
                serverId,  
                channelId, 
                messageId 
            });
            Console.WriteLine($"Notified soft delete to group: {groupName}");
        }

        public async Task NotifyMessageUpdate(string serverId, string channelId, MessageResponseDTO updatedMessage)
        {
            string groupName = GetChannelGroupName(serverId, channelId);
            await hubContext.Clients.Group(groupName).SendAsync("ReceiveMessageUpdated", new
            {
                serverId,
                channelId,
                message = updatedMessage
            });
            Console.WriteLine($"Notified message update to group: {groupName}");
        }
    }
}