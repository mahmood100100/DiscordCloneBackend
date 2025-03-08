using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class DirectMessageNotificationService : IDirectMessageNotificationService
    {
        private readonly IHubContext<DirectMessageHub> hubContext;

        public DirectMessageNotificationService(IHubContext<DirectMessageHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task NotifyDirectMessageAdded(string conversationId, DirectMessageResponseDTO addedDirectMessage)
        {
            await hubContext.Clients.Group(conversationId).SendAsync("ReceiveDirectMessageAdded", new
            {
                ConversationId = conversationId,
                Message = addedDirectMessage
            });
        }

        public async Task NotifyDirectMessageHardDeleted(string conversationId, DirectMessageResponseDTO deletedDirectMessage)
        {
            await hubContext.Clients.Group(conversationId).SendAsync("ReceiveDirectMessageDeleted", new
            {
                ConversationId = conversationId,
                MessageId = deletedDirectMessage.Id
            });
        }

        public async Task NotifyDirectMessageSoftDeleted(string conversationId, string directMessageId)
        {
            await hubContext.Clients.Group(conversationId).SendAsync("ReceiveDirectMessageSoftDeleted", new
            {
                ConversationId = conversationId,
                MessageId = directMessageId
            });
        }

        public async Task NotifyDirectMessageUpdate(string conversationId, DirectMessageResponseDTO updatedDirectMessage)
        {
            await hubContext.Clients.Group(conversationId).SendAsync("ReceiveDirectMessageUpdated", new
            {
                ConversationId = conversationId,
                UpdatedMessage = updatedDirectMessage
            });
        }
    }
}
