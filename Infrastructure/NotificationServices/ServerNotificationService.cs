using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class ServerNotificationService : IServerNotificationService
    {
        private readonly IHubContext<ServerHub> hubContext;

        public ServerNotificationService(IHubContext<ServerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task NotifyServerDeleted(string serverId)
        {
            Console.WriteLine($"NotifyServerDeleted called with serverId: {serverId}");
            await hubContext.Clients.Group(serverId).SendAsync("ReceiveServerDeleted", serverId);
        }

        public async Task NotifyServersDeleted(IEnumerable<string> serverIds)
        {
            Console.WriteLine($"NotifyServersDeleted called with serverIds: {string.Join(", ", serverIds)}");
            foreach (var serverId in serverIds)
            {
                await hubContext.Clients.Group(serverId).SendAsync("ReceiveServerDeleted", serverId);
            }
        }
    }
}