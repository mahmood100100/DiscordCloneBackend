using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Presentation.Hubs
{
    public class MessageHub : BaseHub<MessageHub>
    {
        private string GetChannelGroupName(string serverId, string channelId)
        {
            if (string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentException("ServerId and ChannelId cannot be null or empty.");
            }
            return $"Server_{serverId}_Channel_{channelId}";
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client {Context.ConnectionId} connected to MessageHub");
            await base.OnConnectedAsync();
        }

        public override async Task JoinGroup(string groupId)
        {
            Console.WriteLine($"JoinGroup called by {Context.ConnectionId} with groupId: {groupId}");
            var parts = groupId.Split(',');
            if (parts.Length != 2)
            {
                Console.WriteLine($"Invalid groupId format from {Context.ConnectionId}: {groupId}");
                throw new ArgumentException("GroupId must be in the format 'serverId,channelId'.");
            }

            string serverId = parts[0];
            string channelId = parts[1];
            string groupName = GetChannelGroupName(serverId, channelId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} joined group {groupName} (serverId: {serverId}, channelId: {channelId})");
        }

        public override async Task LeaveGroup(string groupId)
        {
            Console.WriteLine($"LeaveGroup called by {Context.ConnectionId} with groupId: {groupId}");
            var parts = groupId.Split(',');
            if (parts.Length != 2)
            {
                Console.WriteLine($"Invalid groupId format from {Context.ConnectionId}: {groupId}");
                throw new ArgumentException("GroupId must be in the format 'serverId,channelId'.");
            }

            string serverId = parts[0];
            string channelId = parts[1];
            string groupName = GetChannelGroupName(serverId, channelId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} left group {groupName}");
        }
    }
}