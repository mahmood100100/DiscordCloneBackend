using AutoMapper;
using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.Data;
using DiscordCloneBackend.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordCloneBackend.Application.Services
{
    public class ServerService :
        GenericService<Server, ServerRequestDTO, ServerResponseDTO>,
    IServerService
    {
        private readonly IFileService fileService;
        private readonly IMemberService memberService;
        private readonly IChannelService channelService;
        private readonly IServerNotificationService serverNotificationService;

        public ServerService(IUnitOfWork unitOfWork ,
            IMapper mapper ,
            IFileService fileService , IMemberService memberService , IChannelService channelService , IServerNotificationService serverNotificationService) :
            base(mapper , unitOfWork)
        {
            this.fileService = fileService;
            this.memberService = memberService;
            this.channelService = channelService;
            this.serverNotificationService = serverNotificationService;
        }

        public async Task<ServerResponseDTO> AddServerWithImageAsync(ServerRequestDTO dto)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var profile = await unitOfWork.Profiles.GetByIdAsync(dto.ProfileId);
                if (profile == null)
                {
                    throw new KeyNotFoundException("Invalid ProfileId. Profile does not exist.");
                }

                var imageUrl = await fileService.UploadFileAsync(dto.Image, "ServersImages");
                if (string.IsNullOrEmpty(imageUrl))
                {
                    throw new Exception("Failed to upload server image.");
                }

                var inviteCode = Guid.NewGuid().ToString();

                var server = new Server
                {
                    Name = dto.Name,
                    ImageUrl = imageUrl,
                    ProfileId = dto.ProfileId,
                    InviteCode = inviteCode,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await unitOfWork.Servers.AddAsync(server);
                await unitOfWork.CompleteAsync();

                var memberRequest = new MemberRequestDTO
                {
                    ProfileId = dto.ProfileId,
                    ServerId = server.Id
                };

                var member = await memberService.AddMemberToServerAsync(memberRequest, MemberRole.Admin);

                var channelRequest = new ChannelRequestDTO
                {
                    Name = "General",
                    Type = 0,
                    ServerId = server.Id,
                    RequesterProfileId = dto.ProfileId,
                };

                await channelService.CreateChannelAsync(channelRequest);

                return mapper.Map<ServerResponseDTO>(server);
            });
        }

        public async Task DeleteServersByProfileIdAsync(string profileId)
        {
            var servers = await unitOfWork.Servers.GetServersByProfileIdAsync(profileId);
            if (servers == null || !servers.Any())
            {
                return;
            }

            var deletedServerIds = new List<string>();

            foreach (var server in servers)
            {
                var members = await unitOfWork.Members.GetMembersByServerIdAsync(server.Id);
                foreach (var member in members)
                {
                    await memberService.CleanUpMemberDependenciesAsync(member.Id, skipMessages: true);
                }

                if (!string.IsNullOrEmpty(server.ImageUrl))
                {
                    var fileName = Path.GetFileNameWithoutExtension(server.ImageUrl.Split('/').Last().Split('?').First());
                    try
                    {
                        fileService.DeleteFile(fileName, "ServersImages");
                    }
                    catch (Exception ex)
                    {
                    }
                }

                await unitOfWork.Servers.DeleteAsync(server.Id);
                deletedServerIds.Add(server.Id);
            }

            await unitOfWork.CompleteAsync();
            await serverNotificationService.NotifyServersDeleted(deletedServerIds);
        }

        public async Task DeleteServerAsync(string serverId, string profileId)
        {
            var member = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(profileId, serverId);

            if (member == null)
            {
                throw new UnauthorizedAccessException("You are not a member of this server.");
            }

            if (member.Role != MemberRole.Admin)
            {
                throw new UnauthorizedAccessException("Only Admins can delete a server.");
            }

            var server = await unitOfWork.Servers.GetByIdAsync(serverId);
            if (server == null)
            {
                throw new KeyNotFoundException("Server not found.");
            }

            if (!string.IsNullOrEmpty(server.ImageUrl))
            {
                var fileName = Path.GetFileNameWithoutExtension(server.ImageUrl.Split('/').Last().Split('?').First());
                fileService.DeleteFile(fileName, "ServersImages");
            }

            await unitOfWork.Servers.DeleteAsync(server.Id);
            await unitOfWork.CompleteAsync();
            await serverNotificationService.NotifyServerDeleted(server.Id);
        }

        public async Task<string> GenerateNewInviteCodeAsync(string serverId)
        {
            var server = await unitOfWork.Servers.GetByIdAsync(serverId);
            if (server == null)
            {
                throw new KeyNotFoundException("Server not found.");
            }

            var inviteCode = Guid.NewGuid().ToString();

            server.InviteCode = inviteCode;

            unitOfWork.Servers.Update(server);

            await unitOfWork.CompleteAsync();
            return inviteCode;
        }


        public async Task<ServerResponseDTO> GetServerByInviteCodeAsync(string inviteCode)
        {
            var server = await unitOfWork.Servers.GetServerByInviteCodeAsync(inviteCode);
            return mapper.Map<ServerResponseDTO>(server);
        }

        public async Task<IEnumerable<ServerResponseDTO>> GetServersByProfileIdAsync(string profileId)
        {
            var profile = await unitOfWork.Profiles.GetByIdAsync(profileId);
            if(profile == null)
            {
                throw new KeyNotFoundException("no profile found with this id");
            }
            var servers = await unitOfWork.Servers.GetServersByProfileIdAsync(profileId);
            return mapper.Map<IEnumerable<ServerResponseDTO>>(servers);
        }

        public async Task<ServerResponseDTO> GetServerByIdWithDetailsAsync(string serverId)
        {
            var server = await unitOfWork.Servers.GetServerByIdIncludingMembersAndChannelsAsync(serverId);

            if (server == null)
            {
                throw new KeyNotFoundException($"Server with this id : {serverId} not found");
            }

            var serverDto = mapper.Map<ServerResponseDTO>(server);
            return serverDto;
        }

        public async Task<ServerResponseDTO> GetServerByInviteCodeAndProfileIdAsync(string inviteCode, string profileId)
        {
            var profile = await unitOfWork.Profiles.GetByIdAsync(profileId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found.");
            }

            var server = await unitOfWork.Servers.GetServerByInviteCodeAndProfileIdAsync(inviteCode, profileId);

            if (server == null)
            {
                var invitedServer = await unitOfWork.Servers.GetServerByInviteCodeAsync(inviteCode);

                if (invitedServer != null)
                {
                    throw new UnauthorizedAccessException("You are not a member of this server.");
                }
                else
                {
                    throw new KeyNotFoundException("Server not found with the given invite code.");
                }
            }

            return mapper.Map<ServerResponseDTO>(server);
        }

        public async Task<ServerResponseDTO> UpdateServerWithImageAsync(string serverId, ServerRequestDTO dto)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var profile = await unitOfWork.Profiles.GetByIdAsync(dto.ProfileId);
                if (profile == null)
                {
                    throw new KeyNotFoundException("Invalid ProfileId. Profile does not exist.");
                }

                var server = await unitOfWork.Servers.GetByIdAsync(serverId , includeProperties:"Members,Channels");
                if (server == null)
                {
                    throw new KeyNotFoundException("Server not found.");
                }

                if (dto.Image != null)
                {
                    var imageUrl = await fileService.UploadFileAsync(dto.Image, "ServersImages");
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        throw new Exception("Failed to upload server image.");
                    }
                    server.ImageUrl = imageUrl;
                }

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    server.Name = dto.Name;
                }

                await unitOfWork.CompleteAsync();

                return mapper.Map<ServerResponseDTO>(server);
            });
        }
    }
}
