using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IMessageService : IGenericService<Message, MessageRequestDTO, MessageResponseDTO>
    {
        Task<MessageResponseDTO> CreateMessageAsync(MessageRequestDTO request);
        Task SoftDeleteAsync(string id);
        Task HardDeleteAsync(string id);
        Task<IEnumerable<MessageResponseDTO>> GetMessagesByChannelIdAsync(string channelId,int page = 1,int pageSize = 10);
        Task<IEnumerable<MessageResponseDTO>> GetMessagesByMemberIdAsync(string memberId);
        Task<MessageResponseDTO> UpdateMessageAsync(ChangeMessageRequestDTO request);
        Task<int> GetMessagesCountByChannelIdAsync(string channelId);
    }
}
