using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;

namespace DiscordCloneBackend.Application.Services
{
    public class ConversationService : GenericService<Conversation, ConversationRequestDTO, ConversationResponseDTO>,
        IConversationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ConversationService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ConversationResponseDTO> GetOrCreateConversationByMembersAsync(string memberOneId, string memberTwoId, int directMessagesLimit = 15)
        {
            try
            {
                if (string.IsNullOrEmpty(memberOneId) || string.IsNullOrEmpty(memberTwoId))
                {
                    throw new ArgumentException("Member IDs cannot be null or empty.");
                }

                if (unitOfWork == null || unitOfWork.Conversations == null)
                {
                    throw new InvalidOperationException("Unit of work or conversation repository is not initialized.");
                }

                if (mapper == null)
                {
                    throw new InvalidOperationException("Mapper is not initialized.");
                }

                var existingConversation = await unitOfWork.Conversations.GetConversationByMembersAsync(memberOneId, memberTwoId, directMessagesLimit);
                if (existingConversation != null)
                {
                    return mapper.Map<ConversationResponseDTO>(existingConversation);
                }

                var conversationDto = new ConversationRequestDTO
                {
                    MemberOneId = memberOneId,
                    MemberTwoId = memberTwoId
                };

                if (memberOneId == memberTwoId)
                {
                    throw new InvalidOperationException("A member cannot have a conversation with themselves.");
                }

                await base.AddAsync(conversationDto);
                await unitOfWork.CompleteAsync();
                existingConversation = await unitOfWork.Conversations.GetConversationByMembersAsync(memberOneId, memberTwoId, directMessagesLimit);

                if (existingConversation == null)
                {
                    throw new InvalidOperationException("Failed to retrieve the newly created conversation.");
                }

                return mapper.Map<ConversationResponseDTO>(existingConversation);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid member IDs: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving or creating the conversation: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ConversationResponseDTO>> GetConversationsByMemberIdAsync(string memberId)
        {
            try
            {
                var conversations = await unitOfWork.Conversations.GetConversationsByMemberIdAsync(memberId);
                return mapper.Map<IEnumerable<ConversationResponseDTO>>(conversations);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid member ID: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving conversations: {ex.Message}", ex);
            }
        }
    }
}