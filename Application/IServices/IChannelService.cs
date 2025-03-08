using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Enums;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IChannelService : IGenericService<Channel , ChannelRequestDTO ,  ChannelResponseDTO>
    {
        Task<ChannelResponseDTO> CreateChannelAsync(ChannelRequestDTO channelRequestDTO);
        Task DeleteChannelAsync(DeleteChannelRequestDto Dto);
        Task<ChannelResponseDTO> ChangeChannelNameAsync(ChangeChannelNameRequestDto dto);
        Task<IEnumerable<ChannelResponseDTO>> GetChannelsByServerIdAsync(string serverId);
        Task<IEnumerable<ChannelResponseDTO>> GetChannelsByTypeAsync(ChannelType type);
        Task<ChannelResponseDTO> GetChannelByIdAsync(string id, int messageLimit, string includeProperties = null);
    }
}
