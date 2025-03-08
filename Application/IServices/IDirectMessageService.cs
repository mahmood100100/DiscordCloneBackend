using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IDirectMessageService : IGenericService<DirectMessage, DirectMessageRequestDTO, DirectMessageResponseDTO>
    {
        Task<DirectMessageResponseDTO> CreateMessageAsync(DirectMessageRequestDTO messageDto);
        Task SoftDeleteAsync(string messageId);
        Task HardDeleteAsync(string messageId);
        Task<IEnumerable<DirectMessageResponseDTO>> GetDirectMessagesByConversationIdAsync(
            string conversationId, int page = 1, int pageSize = 10
            );
        Task<IEnumerable<DirectMessageResponseDTO>> GetDirectMessagesByMemberIdAsync(string memberId);
        Task<DirectMessageResponseDTO> UpdateDirectMessageAsync(ChangeMessageRequestDTO request);
    }
}
