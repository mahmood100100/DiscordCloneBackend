namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        IProfileRepository Profiles { get; }
        IServerRepository Servers { get; }
        IMemberRepository Members { get; }
        IChannelRepository Channels { get; }
        IMessageRepository Messages { get; }
        IConversationRepository Conversations { get; }
        IDirectMessageRepository DirectMessages { get; }
        IUserRepository Users { get; }

        IGenericRepository<T> GetGenericRepository<T>() where T : class;

        Task<int> CompleteAsync();
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
