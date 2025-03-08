using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IMemberService : IGenericService<Member ,MemberRequestDTO, MemberResponseDTO>
    {
        Task<MemberResponseDTO> AddMemberToServerAsync(MemberRequestDTO memberRequestDTO, MemberRole role);
        Task DeleteMemberAsync(DeleteMemberRequestDto deleteMemberRequestDto);
        Task<MemberResponseDTO> ChangeMemberRoleAsync(ChangeMemberRoleRequestDto dto);
        Task<IEnumerable<MemberResponseDTO>> GetMembersByServerIdAsync(string serverId);
        Task<IEnumerable<MemberResponseDTO>> GetMembersByRoleAsync(MemberRole role);
        Task CleanUpMemberDependenciesAsync(string memberId, bool skipMessages = false);
        Task SoftDeleteMemberAsync(DeleteMemberRequestDto deleteMemberRequestDto);
    }
}
