using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.ExternalServices;
using DiscordCloneBackend.Infrastructure.NotificationServices;
using DiscordCloneBackend.Infrastructure.Repositories;

namespace DiscordCloneBackend.Application.Services
{
    public class DirectMessageService : GenericService<DirectMessage, DirectMessageRequestDTO, DirectMessageResponseDTO>
        , IDirectMessageService
    {
        private readonly IFileService fileService;
        private readonly IDirectMessageNotificationService directMessageNotificationService;

        public DirectMessageService(IMapper mapper
            , IUnitOfWork unitOfWork, IFileService fileService , IDirectMessageNotificationService directMessageNotificationService ) :
            base(mapper , unitOfWork)
        {
            this.fileService = fileService;
            this.directMessageNotificationService = directMessageNotificationService;
        }
        public async Task<IEnumerable<DirectMessageResponseDTO>> GetDirectMessagesByConversationIdAsync(
            string conversationId ,int page = 1 ,int pageSize = 10
            )
        {
            var directMessages = await unitOfWork.DirectMessages.GetDirectMessagesByConversationIdAsync(conversationId , page , pageSize);
            return mapper.Map<IEnumerable<DirectMessageResponseDTO>>(directMessages);
        }

        public async Task<IEnumerable<DirectMessageResponseDTO>> GetDirectMessagesByMemberIdAsync(string memberId)
        {
            var directMessages = await unitOfWork.DirectMessages.GetDirectMessagesByMemberIdAsync(memberId);
            return mapper.Map<IEnumerable<DirectMessageResponseDTO>>(directMessages);
        }

        public async Task<DirectMessageResponseDTO> CreateMessageAsync(DirectMessageRequestDTO messageDto)
        {
            string? fileUrl = null;

            if (messageDto.File != null)
            {
                fileUrl = await fileService.UploadFileAsync(messageDto.File, "DirectMessagesFiles");
            }

            var conversation = await unitOfWork.Conversations.GetByIdAsync(messageDto.ConversationId);
            if (conversation == null)
            {
                throw new KeyNotFoundException("conversation not found");
            }

            var directMessage = mapper.Map<DirectMessage>(messageDto);

            await unitOfWork.DirectMessages.AddAsync(directMessage);
            await unitOfWork.CompleteAsync();
            var directMessageResponse =  mapper.Map<DirectMessageResponseDTO>(directMessage);
            await directMessageNotificationService.NotifyDirectMessageAdded(conversation.Id , directMessageResponse);
            return directMessageResponse;
        }

        public async Task SoftDeleteAsync(string messageId)
        {
            var directMessage = await unitOfWork.DirectMessages.GetByIdAsync(messageId);
            if (directMessage == null)
            {
                throw new KeyNotFoundException("directMessage not found.");
            }

            directMessage.Deleted = true;
            await unitOfWork.CompleteAsync();
            await directMessageNotificationService.NotifyDirectMessageSoftDeleted(directMessage.ConversationId, directMessage.Id);
        }

        public async Task HardDeleteAsync(string messageId)
        {
            var directMessage = await unitOfWork.DirectMessages.GetByIdAsync(messageId);
            if (directMessage != null)
            {
                if (!string.IsNullOrEmpty(directMessage.FileUrl))
                {
                    var fileName = Path.GetFileNameWithoutExtension(directMessage.FileUrl.Split('/').Last().Split('?').First());
                    fileService.DeleteFile(fileName , "DirectMessagesFiles");
                }
                await unitOfWork.DirectMessages.DeleteAsync(directMessage.Id);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task<DirectMessageResponseDTO> UpdateDirectMessageAsync(ChangeMessageRequestDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            var directMessage = await unitOfWork.DirectMessages.GetByIdAsync(request.MessageId, "Sender,Receiver,Conversation");
            if (directMessage == null)
            {
                throw new KeyNotFoundException($"Direct message with ID {request.MessageId} not found.");
            }

            var conversation = await unitOfWork.Conversations.GetByIdAsync(directMessage.ConversationId);
            if (conversation == null)
            {
                throw new KeyNotFoundException("Conversation not found");
            }

            if (request.File != null)
            {
                try
                {
                    directMessage.FileUrl = await fileService.UploadFileAsync(request.File, "DirectMessagesFiles");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to upload file.", ex);
                }
            }
            else if (request.File == null && !string.IsNullOrEmpty(directMessage.FileUrl))
            {
                fileService.DeleteFile(directMessage.FileUrl, "DirectMessagesFiles");
                directMessage.FileUrl = null;
            }

            if (!string.IsNullOrEmpty(request.Content))
            {
                directMessage.Content = request.Content;
            }

            directMessage.UpdatedAt = DateTime.UtcNow;

            unitOfWork.DirectMessages.Update(directMessage);
            await unitOfWork.CompleteAsync();

            var directMessageResponse = mapper.Map<DirectMessageResponseDTO>(directMessage);

            await directMessageNotificationService.NotifyDirectMessageUpdate(conversation.Id, directMessageResponse);

            return directMessageResponse;
        }
    }
}
