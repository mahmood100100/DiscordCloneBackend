using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IServerService : IGenericService<Server , ServerRequestDTO , ServerResponseDTO>
    {
        Task<ServerResponseDTO> AddServerWithImageAsync(ServerRequestDTO dto);
        Task DeleteServerAsync(string serverId, string profileId);
        Task<string> GenerateNewInviteCodeAsync(string serverId);
        Task<ServerResponseDTO> GetServerByInviteCodeAsync(string inviteCode);
        Task<IEnumerable<ServerResponseDTO>> GetServersByProfileIdAsync(string profileId);
        Task<ServerResponseDTO> GetServerByIdWithDetailsAsync(string serverId);
        Task<ServerResponseDTO> GetServerByInviteCodeAndProfileIdAsync(string inviteCode, string profileId);
        Task<ServerResponseDTO> UpdateServerWithImageAsync(string serverId, ServerRequestDTO dto);
        Task DeleteServersByProfileIdAsync(string profileId);

    }
}
