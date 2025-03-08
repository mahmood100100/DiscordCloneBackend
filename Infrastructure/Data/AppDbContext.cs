using DiscordCloneBackend.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscordCloneBackend.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<LocalUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties for each entity
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<DirectMessage> DirectMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LocalUser - Profile (1:1)
            modelBuilder.Entity<LocalUser>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a user deletes their profile

            // Profile - Server (1:Many)
            modelBuilder.Entity<Server>()
                .HasOne(s => s.Profile)
                .WithMany(p => p.Servers)
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent server deletion when profile is deleted

            // Profile - Member (1:Many)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Profile)
                .WithMany(p => p.Members)
                .HasForeignKey(m => m.ProfileId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a profile removes their memberships

            // Profile - Channel (1:Many)
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Profile)
                .WithMany(p => p.Channels)
                .HasForeignKey(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a profile deletes their channels

            // Server - Member (1:Many)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Server)
                .WithMany(s => s.Members)
                .HasForeignKey(m => m.ServerId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a server removes its members

            // Server - Channel (1:Many)
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Server)
                .WithMany(s => s.Channels)
                .HasForeignKey(c => c.ServerId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a server deletes its channels

            // Member - Message (1:Many)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Member)
                .WithMany(mb => mb.Messages)
                .HasForeignKey(m => m.MemberId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve messages when a member is deleted

            // Channel - Message (1:Many)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Channel)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a channel deletes its messages

            // Conversation - Member Relationships (1:Many for each)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.MemberOne)
                .WithMany(m => m.ConversationsInitiated)
                .HasForeignKey(c => c.MemberOneId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve conversations when a member is deleted

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.MemberTwo)
                .WithMany(m => m.ConversationsReceived)
                .HasForeignKey(c => c.MemberTwoId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve conversations when a member is deleted

            // Conversation - DirectMessage (1:Many)
            modelBuilder.Entity<DirectMessage>()
                .HasOne(dm => dm.Conversation)
                .WithMany(c => c.DirectMessages)
                .HasForeignKey(dm => dm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a conversation deletes its direct messages

            // Member - DirectMessage (Sender, 1:Many)
            modelBuilder.Entity<DirectMessage>()
                .HasOne(dm => dm.Sender)
                .WithMany(m => m.SentDirectMessages)
                .HasForeignKey(dm => dm.SenderMemberId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a member deletes their sent direct messages

            // Member - DirectMessage (Receiver, 1:Many)
            modelBuilder.Entity<DirectMessage>()
                .HasOne(dm => dm.Receiver)
                .WithMany(m => m.ReceivedDirectMessages)
                .HasForeignKey(dm => dm.ReceiverMemberId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve received messages when a member is deleted

            // Unique constraint for Conversation
            modelBuilder.Entity<Conversation>()
                .HasIndex(c => new { c.MemberOneId, c.MemberTwoId })
                .IsUnique()
                .HasDatabaseName("IX_Conversation_Members_Unique");

            // Entity keys (optional, as EF infers these by convention)
            modelBuilder.Entity<Member>().HasKey(m => m.Id);
            modelBuilder.Entity<Server>().HasKey(s => s.Id);
            modelBuilder.Entity<Channel>().HasKey(c => c.Id);
            modelBuilder.Entity<Message>().HasKey(m => m.Id);
            modelBuilder.Entity<DirectMessage>().HasKey(dm => dm.Id);
            modelBuilder.Entity<Conversation>().HasKey(c => c.Id);
            modelBuilder.Entity<Profile>().HasKey(p => p.Id);
        }
    }
}