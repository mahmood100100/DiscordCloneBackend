using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordCloneBackend.Application.Services
{
    public class MemberService : GenericService<Member, MemberRequestDTO, MemberResponseDTO>,
        IMemberService
    {
        private readonly IMemberNotificationService memberNotificationService;

        public MemberService(IMapper mapper, 
            IUnitOfWork unitOfWork , 
            IMemberNotificationService memberNotificationService) :
            base (mapper , unitOfWork)
        {
            this.memberNotificationService = memberNotificationService;
        }

        public async Task<MemberResponseDTO> AddMemberToServerAsync(MemberRequestDTO memberRequestDTO, MemberRole role)
        {
            if (memberRequestDTO == null)
            {
                throw new ArgumentNullException(nameof(memberRequestDTO), "Member request DTO cannot be null.");
            }

            if (string.IsNullOrEmpty(memberRequestDTO.ProfileId))
            {
                throw new ArgumentException("Profile ID cannot be null or empty.", nameof(memberRequestDTO.ProfileId));
            }

            if (string.IsNullOrEmpty(memberRequestDTO.ServerId))
            {
                throw new ArgumentException("Server ID cannot be null or empty.", nameof(memberRequestDTO.ServerId));
            }

            try
            {
                var existingMember = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(memberRequestDTO.ProfileId, memberRequestDTO.ServerId);

                if (existingMember != null)
                {
                    if (!existingMember.Deleted)
                    {
                        throw new InvalidOperationException("This member is already part of the server.");
                    }

                    existingMember.Deleted = false;
                    existingMember.Role = role;

                    unitOfWork.Members.Update(existingMember);
                    await unitOfWork.CompleteAsync();

                    var restoredMemberResponse = await GetByIdAsync(existingMember.Id, "Profile,Server");
                    await memberNotificationService.NotifyMemberAdded(memberRequestDTO.ServerId, restoredMemberResponse);
                    return restoredMemberResponse;
                }

                var member = mapper.Map<Member>(memberRequestDTO);
                member.Role = role;

                await unitOfWork.Members.AddAsync(member);
                await unitOfWork.CompleteAsync();

                var newMemberResponseDTO = await GetByIdAsync(member.Id, "Profile,Server");
                await memberNotificationService.NotifyMemberAdded(memberRequestDTO.ServerId, newMemberResponseDTO);
                return newMemberResponseDTO;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (AutoMapperMappingException ex)
            {
                throw new Exception("Failed to map member data.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add member to the server.", ex);
            }
        }


        public async Task DeleteMemberAsync(DeleteMemberRequestDto deleteMemberRequestDto)
        {
            var requester = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(deleteMemberRequestDto.RequesterProfileId, deleteMemberRequestDto.ServerId);
            var target = await unitOfWork.Members.GetByIdAsync(deleteMemberRequestDto.TargetMemberId);

            if (requester == null || target == null || requester.ServerId != deleteMemberRequestDto.ServerId || target.ServerId != deleteMemberRequestDto.ServerId)
                throw new KeyNotFoundException("One or both members not found in this server.");

            var allServerMembers = await unitOfWork.Members.GetMembersByServerIdAsync(deleteMemberRequestDto.ServerId);
            int adminCount = allServerMembers.Count(m => m.Role == MemberRole.Admin);
            int memberCount = allServerMembers.Count();

            if (memberCount == 1)
                throw new InvalidOperationException("Cannot delete the only member of the server.");

            var deletedMember = await GetByIdAsync(target.Id, "Profile,Server");

            if (requester.Id == deleteMemberRequestDto.TargetMemberId)
            {
                await CleanUpMemberDependenciesAsync(target.Id);
                await unitOfWork.Members.DeleteAsync(target.Id);
                await unitOfWork.CompleteAsync();
                await memberNotificationService.NotifyMemberDeleted(deleteMemberRequestDto.ServerId, deletedMember, deleteMemberRequestDto.RequesterProfileId , false);
                return;
            }

            if (requester.Role != MemberRole.Admin)
                throw new UnauthorizedAccessException("Only admins can delete members.");

            if (target.Role == MemberRole.Admin && requester.Id != deleteMemberRequestDto.TargetMemberId)
                throw new InvalidOperationException("Admins cannot delete other admins.");

            if (target.Role == MemberRole.Admin && adminCount == 1)
                throw new InvalidOperationException("Cannot delete the last admin. Assign another admin first.");

            await CleanUpMemberDependenciesAsync(target.Id);
            await unitOfWork.Members.DeleteAsync(target.Id);
            await unitOfWork.CompleteAsync();
            await memberNotificationService.NotifyMemberDeleted(deleteMemberRequestDto.ServerId, deletedMember, deleteMemberRequestDto.RequesterProfileId , true);
        }

        public async Task SoftDeleteMemberAsync(DeleteMemberRequestDto deleteMemberRequestDto)
        {
            var requester = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(deleteMemberRequestDto.RequesterProfileId, deleteMemberRequestDto.ServerId);
            var target = await unitOfWork.Members.GetByIdAsync(deleteMemberRequestDto.TargetMemberId);

            if (requester == null || target == null || requester.ServerId != deleteMemberRequestDto.ServerId || target.ServerId != deleteMemberRequestDto.ServerId)
                throw new KeyNotFoundException("One or both members not found in this server.");

            var allServerMembers = await unitOfWork.Members.GetMembersByServerIdAsync(deleteMemberRequestDto.ServerId);
            var activeMembers = allServerMembers.Where(m => !m.Deleted).ToList();
            int adminCount = activeMembers.Count(m => m.Role == MemberRole.Admin);
            int memberCount = activeMembers.Count();

            if (memberCount == 1)
                throw new InvalidOperationException("Cannot soft-delete the only active member of the server.");

            var softDeletedMember = await GetByIdAsync(target.Id, "Profile,Server");

            if (requester.Id == deleteMemberRequestDto.TargetMemberId)
            {
                target.Deleted = true;
                unitOfWork.Members.Update(target);
                await unitOfWork.CompleteAsync();
                await memberNotificationService.NotifyMemberSoftDeleted(deleteMemberRequestDto.ServerId, softDeletedMember, deleteMemberRequestDto.RequesterProfileId , false);
                return;
            }

            if (requester.Role != MemberRole.Admin)
                throw new UnauthorizedAccessException("Only admins can soft-delete members.");

            if (target.Role == MemberRole.Admin && requester.Id != deleteMemberRequestDto.TargetMemberId)
                throw new InvalidOperationException("Admins cannot soft-delete other admins.");

            if (target.Role == MemberRole.Admin && adminCount == 1)
                throw new InvalidOperationException("Cannot soft-delete the last admin. Assign another admin first.");

            target.Deleted = true;
            unitOfWork.Members.Update(target);
            await unitOfWork.CompleteAsync();
            await memberNotificationService.NotifyMemberSoftDeleted(deleteMemberRequestDto.ServerId, softDeletedMember, deleteMemberRequestDto.RequesterProfileId, true);
        }

        public async Task CleanUpMemberDependenciesAsync(string memberId, bool skipMessages = false)
        {
            if (!skipMessages)
            {
                var messages = await unitOfWork.Messages.GetMessagesByMemberIdAsync(memberId);
                foreach (var message in messages)
                {
                    await unitOfWork.Messages.DeleteAsync(message.Id);
                }
            }

            var receivedDirectMessages = await unitOfWork.DirectMessages.GetDirectMessagesByReceiverMemberIdAsync(memberId);
            foreach (var dm in receivedDirectMessages)
            {
                await unitOfWork.DirectMessages.DeleteAsync(dm.Id);
            }

            var conversations = await unitOfWork.Conversations.GetConversationsByMemberIdAsync(memberId);
            foreach (var conversation in conversations)
            {
                await unitOfWork.Conversations.DeleteAsync(conversation.Id);
            }
        }

        public async Task<MemberResponseDTO> ChangeMemberRoleAsync(ChangeMemberRoleRequestDto dto)
        {
            var requester = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(dto.RequesterProfileId, dto.ServerId);
            var target = await unitOfWork.Members.GetByIdAsync(dto.TargetMemberId, "Profile,Server");

            if (requester == null || target == null)
            {
                throw new KeyNotFoundException("One or both members not found in this server.");
            }

            if (requester.ServerId != dto.ServerId || target.ServerId != dto.ServerId)
            {
                throw new UnauthorizedAccessException("Both members must belong to the same server.");
            }

            if (requester.Role != MemberRole.Admin)
            {
                throw new UnauthorizedAccessException("Only admins can change the roles of members.");
            }

            var adminCount = await unitOfWork.Members.CountAdminMembersAsync(dto.ServerId);

            if (requester.Id == target.Id && adminCount == 1)
            {
                throw new InvalidOperationException("You cannot change your role because you are the only admin in this server.");
            }

            if (requester.Id != target.Id && target.Role == MemberRole.Admin)
            {
                throw new InvalidOperationException("You cannot change the role of another admin.");
            }

            if (!Enum.IsDefined(typeof(MemberRole), dto.NewRole))
            {
                throw new ArgumentException("Invalid role specified.", nameof(dto.NewRole));
            }

            var newRoleEnum = (MemberRole)dto.NewRole;
            target.Role = newRoleEnum;
            await unitOfWork.CompleteAsync();

            var updatedMember = mapper.Map<MemberResponseDTO>(target);
            await memberNotificationService.NotifyRoleUpdate(dto.ServerId, updatedMember);
            return updatedMember;
        }



        public async Task<IEnumerable<MemberResponseDTO>> GetMembersByRoleAsync(MemberRole role)
        {
            var members = await unitOfWork.Members.GetMembersByRoleAsync(role);
            return mapper.Map<IEnumerable<MemberResponseDTO>>(members);
        }

        public async Task<IEnumerable<MemberResponseDTO>> GetMembersByServerIdAsync(string serverId)
        {
            var members = await unitOfWork.Members.GetMembersByServerIdAsync(serverId);
            return mapper.Map<IEnumerable<MemberResponseDTO>>(members);
        }
    }
}
