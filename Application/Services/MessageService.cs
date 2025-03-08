using AutoMapper;
using AutoMapper.Execution;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.INotificationServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.ExternalServices;
using DiscordCloneBackend.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiscordCloneBackend.Application.Services
{
    public class MessageService : GenericService<Message, MessageRequestDTO, MessageResponseDTO>,
        IMessageService
    {
        private readonly IFileService fileService;
        private readonly IMessageNotificationService messageNotificationService;

        public MessageService(IUnitOfWork unitOfWork,
            IMapper mapper , IFileService fileService , IMessageNotificationService messageNotificationService) :
            base(mapper , unitOfWork)
        {
            this.fileService = fileService;
            this.messageNotificationService = messageNotificationService;
        }

        public async Task<MessageResponseDTO> CreateMessageAsync(MessageRequestDTO request)
        {
            string? fileUrl = null;

            if (request.File != null)
            {
                fileUrl = await fileService.UploadFileAsync(request.File, "messagesFiles");
            }

            var channel = await unitOfWork.Channels.GetByIdAsync(request.ChannelId);
            if (channel == null)
            {
                throw new KeyNotFoundException("channel not found");
            }

            var message = new Message
            {
                Content = request.Content,
                FileUrl = fileUrl,
                MemberId = request.MemberId,
                ChannelId = request.ChannelId
            };

            await unitOfWork.Messages.AddAsync(message);
            await unitOfWork.CompleteAsync();
            var messageResponse =  mapper.Map<MessageResponseDTO>(message);
            await messageNotificationService.NotifyMessageAdded(channel.ServerId ,request.ChannelId , messageResponse);
            return messageResponse;
        }

        public async Task SoftDeleteAsync(string id)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(id);
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found.");
            }
            var channel = await unitOfWork.Channels.GetByIdAsync(message.ChannelId);
            if (channel == null)
            {
                throw new KeyNotFoundException("channel not found");
            }

            message.Deleted = true;
            await unitOfWork.CompleteAsync();
            await messageNotificationService.NotifyMessageSoftDeleted(channel.ServerId, message.ChannelId , message.Id);
        }

        public async Task HardDeleteAsync(string id)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(id);
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            var channel = await unitOfWork.Channels.GetByIdAsync(message.ChannelId);
            if (channel == null)
            {
                throw new KeyNotFoundException("channel not found");
            }

            if (!string.IsNullOrEmpty(message.FileUrl))
            {
                var fileName = Path.GetFileNameWithoutExtension(message.FileUrl.Split('/').Last().Split('?').First());
                fileService.DeleteFile(message.FileUrl , "messagesFiles");
            }

            await unitOfWork.Messages.DeleteAsync(message.Id);
            await unitOfWork.CompleteAsync();
            await messageNotificationService.NotifyMessageHardDeleted(channel.ServerId, message.ChannelId , message.Id);
        }

        public async Task<IEnumerable<MessageResponseDTO>> GetMessagesByChannelIdAsync(
         string channelId,
         int page = 1,
         int pageSize = 10)
        {
            var messages = await unitOfWork.Messages.GetMessagesByChannelIdAsync(channelId, page, pageSize);
            return mapper.Map<IEnumerable<MessageResponseDTO>>(messages);
        }

        public async Task<int> GetMessagesCountByChannelIdAsync(string channelId)
        {
            return await unitOfWork.Messages.GetMessagesCountByChannelIdAsync(channelId);
        }

        public async Task<IEnumerable<MessageResponseDTO>> GetMessagesByMemberIdAsync(string memberId)
        {
            var messages = await unitOfWork.Messages.GetMessagesByMemberIdAsync(memberId);
            return mapper.Map<IEnumerable<MessageResponseDTO>>(messages);
        }

        public async Task<MessageResponseDTO> UpdateMessageAsync(ChangeMessageRequestDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            var message = await unitOfWork.Messages.GetByIdAsync(request.MessageId , "Member,Member.Profile,Channel");
            if (message == null)
            {
                throw new KeyNotFoundException($"Message with ID {request.MessageId} not found.");
            }

            var channel = await unitOfWork.Channels.GetByIdAsync(message.ChannelId);
            if (channel == null)
            {
                throw new KeyNotFoundException("channel not found");
            }

            if (request.File != null)
            {
                try
                {
                    message.FileUrl = await fileService.UploadFileAsync(request.File, "messagesFiles");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to upload file.", ex);
                }
            }
            else if (request.File == null && !string.IsNullOrEmpty(message.FileUrl))
            {
                fileService.DeleteFile(message.FileUrl, "messagesFiles");
                message.FileUrl = null;
            }

            if (!string.IsNullOrEmpty(request.Content))
            {
                message.Content = request.Content;
            }

            message.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Messages.Update(message);
            await unitOfWork.CompleteAsync();

            var messageResponse = mapper.Map<MessageResponseDTO>(message);

            await messageNotificationService.NotifyMessageUpdate(channel.ServerId, message.ChannelId, messageResponse);

            return messageResponse;
        }
    }
}
