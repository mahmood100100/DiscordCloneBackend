using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using Microsoft.AspNetCore.SignalR;

namespace DiscordCloneBackend.Infrastructure.NotificationServices
{
    public class MemberNotificationService : IMemberNotificationService
    {
        private readonly IHubContext<MemberHub> hubContext;

        public MemberNotificationService(IHubContext<MemberHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task NotifyRoleUpdate(string serverId, MemberResponseDTO updatedMember)
        {
            await hubContext.Clients.Group(serverId).SendAsync("ReceiveRoleUpdated", new { serverId, updatedMember });
        }

        public async Task NotifyMemberDeleted(string serverId, MemberResponseDTO deletedMember, string profileId , bool isKicked)
        {
            await hubContext.Clients.Group(serverId).SendAsync("ReceiveMemberDeleted", new { serverId, deletedMember, profileId , isKicked });
        }

        public async Task NotifyMemberAdded(string serverId, MemberResponseDTO addedMember)
        {
            await hubContext.Clients.Group(serverId).SendAsync("ReceiveMemberAdded", new { serverId, addedMember });
        }

        public async Task NotifyMemberSoftDeleted(string serverId, MemberResponseDTO softDeletedMember, string profileId , bool isKicked)
        {
            await hubContext.Clients.Group(serverId).SendAsync("ReceiveMemberSoftDeleted", new { serverId, softDeletedMember, profileId , isKicked });
        }
    }
}