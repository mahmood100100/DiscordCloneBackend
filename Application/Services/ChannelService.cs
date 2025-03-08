using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
namespace DiscordCloneBackend.Application.Services
{
    public class ChannelService : GenericService<Channel, ChannelRequestDTO, ChannelResponseDTO>
        , IChannelService
    {
        private readonly IMemberService memberService;
        private readonly IChannelNotificationService channelNotificationService;
        private readonly ILogger<ChannelService> logger;

        public ChannelService(IMapper mapper
            , IUnitOfWork unitOfWork
            ,IMemberService memberService,
            IChannelNotificationService channelNotificationService , ILogger<ChannelService> logger) :
            base(mapper , unitOfWork)
        {
            this.memberService = memberService;
            this.channelNotificationService = channelNotificationService;
            this.logger = logger;
        }
        public async Task<IEnumerable<ChannelResponseDTO>> GetChannelsByServerIdAsync(string serverId)
        {
            var channels = await unitOfWork.Channels.GetChannelsByServerIdAsync(serverId);
            return mapper.Map<IEnumerable<ChannelResponseDTO>>(channels);
        }

        public async Task<IEnumerable<ChannelResponseDTO>> GetChannelsByTypeAsync(ChannelType type)
        {
            var channels = await unitOfWork.Channels.GetChannelsByTypeAsync(type);
            return mapper.Map<IEnumerable<ChannelResponseDTO>>(channels);
        }

        public async Task<ChannelResponseDTO> CreateChannelAsync(ChannelRequestDTO channelRequestDTO)
        {
            var member = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(channelRequestDTO.RequesterProfileId, channelRequestDTO.ServerId);

            if (member == null || member.ServerId != channelRequestDTO.ServerId)
                throw new KeyNotFoundException("Member or server not found.");

            if (member.Role != MemberRole.Admin && member.Role != MemberRole.Moderator)
                throw new UnauthorizedAccessException("Only admins and moderators can create channels.");

            var existingGeneralChannel = await unitOfWork.Channels.GetByServerIdAndNameAsync(channelRequestDTO.ServerId, "general");
            if (channelRequestDTO.Name.ToLower() == "general" && existingGeneralChannel != null)
                throw new InvalidOperationException("A channel named 'general' already exists in this server.");

            var channel = mapper.Map<Channel>(channelRequestDTO);
            await unitOfWork.Channels.AddAsync(channel);
            await unitOfWork.CompleteAsync();
            var channelResponse = await GetByIdAsync(channel.Id, "Profile,Server");

            await channelNotificationService.NotifyChannelAdded(channelRequestDTO.ServerId, channelResponse);
            return mapper.Map<ChannelResponseDTO>(channelResponse);
        }

        public async Task DeleteChannelAsync(DeleteChannelRequestDto Dto)
        {
            Console.WriteLine($"Deleting channel with ID: {Dto.ChannelId} in server: {Dto.ServerId} by the requester {Dto.RequesterProfileId}");
            var member = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(Dto.RequesterProfileId, Dto.ServerId);

            if (member == null || member.ServerId != Dto.ServerId)
            {
                Console.WriteLine($"member ID: {member?.Id}");
                throw new KeyNotFoundException("Member or server not found.");
            }

            if (member.Role != MemberRole.Admin && member.Role != MemberRole.Moderator)
                throw new UnauthorizedAccessException("Only admins and moderators can delete channels.");

            var channel = await unitOfWork.Channels.GetByIdAsync(Dto.ChannelId);
            if (channel == null || channel.ServerId != Dto.ServerId)
                throw new KeyNotFoundException("Channel not found in this server.");

            if (channel.Name.ToLower() == "general")
                throw new InvalidOperationException("Cannot delete the 'general' channel.");

            Console.WriteLine($"Deleting channel with ID: {channel.Id} in server: {Dto.ServerId}");

            await unitOfWork.Channels.DeleteAsync(channel.Id);
            await unitOfWork.CompleteAsync();
            Console.WriteLine($"Deleted channel with ID: {channel.Id} in server: {Dto.ServerId}");
            await channelNotificationService.NotifyChannelDeleted(Dto.ServerId, channel.Id);
        }

        public async Task<ChannelResponseDTO> ChangeChannelNameAsync(ChangeChannelNameRequestDto dto)
        {
            var member = await unitOfWork.Members.GetMemberByProfileIdAndServerIdAsync(dto.RequesterProfileId, dto.ServerId);

            if (member == null || member.ServerId != dto.ServerId)
                throw new KeyNotFoundException("Member or server not found.");

            if (member.Role != MemberRole.Admin && member.Role != MemberRole.Moderator)
                throw new UnauthorizedAccessException("Only admins and moderators can change channel names.");

            var channel = await unitOfWork.Channels.GetByIdAsync(dto.ChannelId, "Profile,Server");
            if (channel == null)
                throw new KeyNotFoundException("Channel not found.");

            if (dto.ChannelName.ToLower() == "general" && channel.Name.ToLower() != "general")
                throw new InvalidOperationException("Cannot rename a channel to 'general'.");

            channel.Name = dto.ChannelName;
            channel.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Channels.Update(channel);
            await unitOfWork.CompleteAsync();
            var channelResponse = mapper.Map<ChannelResponseDTO>(channel);
            await channelNotificationService.NotifyChannelUpdated(dto.ServerId, channelResponse);
            return channelResponse;
        }

        public async Task<ChannelResponseDTO> GetChannelByIdAsync(string id , int messageLimit , string includeProperties = null)
        {
            var channel = await unitOfWork.Channels.GetByIdAsync(id, includeProperties);

            if (channel == null)
            {
                return null;
            }

            if (channel.Messages != null && channel.Messages.Any())
            {
                channel.Messages = channel.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(messageLimit)
                    .ToList();
            }

            return mapper.Map<ChannelResponseDTO>(channel);
        }
    }
}
