using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<LocalUser> userManager;
        private readonly SignInManager<LocalUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        // Repositories
        private readonly Lazy<IProfileRepository> profiles;
        private readonly Lazy<IServerRepository> servers;
        private readonly Lazy<IMemberRepository> members;
        private readonly Lazy<IChannelRepository> channels;
        private readonly Lazy<IMessageRepository> messages;
        private readonly Lazy<IConversationRepository> conversations;
        private readonly Lazy<IDirectMessageRepository> directMessages;
        private readonly Lazy<IUserRepository> users;

        public UnitOfWork(AppDbContext appDbContext,
                          UserManager<LocalUser> userManager,
                          SignInManager<LocalUser> signInManager,
                          RoleManager<IdentityRole> roleManager)
        {
            this.appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));

            profiles = new Lazy<IProfileRepository>(() => new ProfileRepository(appDbContext, userManager));
            servers = new Lazy<IServerRepository>(() => new ServerRepository(appDbContext));
            members = new Lazy<IMemberRepository>(() => new MemberRepository(appDbContext));
            channels = new Lazy<IChannelRepository>(() => new ChannelRepository(appDbContext));
            messages = new Lazy<IMessageRepository>(() => new MessageRepository(appDbContext));
            conversations = new Lazy<IConversationRepository>(() => new ConversationRepository(appDbContext));
            directMessages = new Lazy<IDirectMessageRepository>(() => new DirectMessageRepository(appDbContext));
            users = new Lazy<IUserRepository>(() => new UserRepository(userManager, signInManager, roleManager));
        }

        // Repositories' Lazy Initialization
        public IProfileRepository Profiles => profiles.Value;
        public IServerRepository Servers => servers.Value;
        public IMemberRepository Members => members.Value;
        public IChannelRepository Channels => channels.Value;
        public IMessageRepository Messages => messages.Value;
        public IConversationRepository Conversations => conversations.Value;
        public IDirectMessageRepository DirectMessages => directMessages.Value;
        public IUserRepository Users => users.Value;

        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(appDbContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            using var transaction = await appDbContext.Database.BeginTransactionAsync();
            try
            {
                T result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public void Dispose()
        {
            appDbContext?.Dispose();
        }
    }
}
