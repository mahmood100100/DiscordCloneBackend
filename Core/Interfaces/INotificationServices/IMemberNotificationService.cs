using DiscordCloneBackend.Application.DTOs;

namespace DiscordCloneBackend.Core.Interfaces.INotificationServices
{
    public interface IMemberNotificationService
    {
        Task NotifyRoleUpdate(string serverId, MemberResponseDTO updatedMember);
        Task NotifyMemberDeleted(string serverId, MemberResponseDTO deletedMember, string profileId, bool isKicked);
        Task NotifyMemberAdded(string serverId, MemberResponseDTO addedMember);
        Task NotifyMemberSoftDeleted(string serverId, MemberResponseDTO softDeletedMember, string profileId , bool isKicked);
    }
}