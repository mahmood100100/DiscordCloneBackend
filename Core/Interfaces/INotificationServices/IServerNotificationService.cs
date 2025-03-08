namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IServerNotificationService
    {
        Task NotifyServerDeleted(string serverId);
        Task NotifyServersDeleted(IEnumerable<string> serverIds);
    }
}
