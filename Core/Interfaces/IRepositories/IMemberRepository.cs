using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;

namespace DiscordCloneBackend.Core.Interfaces.IRepositories
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Task<IEnumerable<Member>> GetMembersByServerIdAsync(string serverId);
        Task<IEnumerable<Member>> GetMembersByRoleAsync(MemberRole role);
        Task<Member> GetMemberByProfileIdAndServerIdAsync(string profileId, string serverId);
        Task<Member> GetMemberByProfileIdAsync(string profileId);
        Task<int> CountAdminMembersAsync(string serverId);
    }
}
