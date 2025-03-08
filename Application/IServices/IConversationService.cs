using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;

namespace DiscordCloneBackend.Application.IServices
{
    public interface IConversationService : IGenericService<Conversation, ConversationRequestDTO, ConversationResponseDTO>
    {
        Task<IEnumerable<ConversationResponseDTO>> GetConversationsByMemberIdAsync(string memberId);
        Task<ConversationResponseDTO> GetOrCreateConversationByMembersAsync(string memberOneId, string memberTwoId, int directMessagesLimit);
    }
}
