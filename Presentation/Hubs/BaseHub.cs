using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Presentation.Hubs
{
    public class BaseHub<T> : Hub where T : class
    {
        public virtual async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the group {groupId}");
        }

        public virtual async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            await Clients.Group(groupId).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the group {groupId}");
        }

        public override async Task OnConnectedAsync()
        {
            var groupId = Context.GetHttpContext()?.Request.Query["groupId"];
            if (!string.IsNullOrEmpty(groupId))
            {
                await JoinGroup(groupId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var groupId = Context.GetHttpContext()?.Request.Query["groupId"];
            if (!string.IsNullOrEmpty(groupId))
            {
                await LeaveGroup(groupId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}