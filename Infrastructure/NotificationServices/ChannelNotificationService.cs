using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class ChannelNotificationService : IChannelNotificationService
    {
        private readonly IHubContext<ChannelHub> hubContext;

        public ChannelNotificationService(IHubContext<ChannelHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task NotifyChannelAdded(string serverId, ChannelResponseDTO addedChannel)
        {
            await hubContext.Clients.All.SendAsync("ReceiveChannelAdded", new { serverId, addedChannel });
        }

        public async Task NotifyChannelDeleted(string serverId, string deletedChannelId)
        {
            await hubContext.Clients.All.SendAsync("ReceiveChannelDeleted", new { serverId, deletedChannelId });
        }

        public async Task NotifyChannelUpdated(string serverId, ChannelResponseDTO updatedChannel)
        {
            await hubContext.Clients.All.SendAsync("ReceiveChannelUpdated", new { serverId, updatedChannel });
        }
    }
}
