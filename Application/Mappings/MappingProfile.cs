using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DiscordCloneBackend.Application.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<LocalUser, UserResponseDTO>()
    .ForMember(dest => dest.Roles, opt => opt.Ignore())
    .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile));

            // UserRequestDTO to LocalUser mapping
            CreateMap<UserRequestDTO, LocalUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // LoginRequestDTO to LocalUser mapping (mapping Email)
            CreateMap<LoginRequestDTO, LocalUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // ResetPasswordDTO to LocalUser mapping (mapping Email)
            CreateMap<ResetPasswordDTO, LocalUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Mapping for ProfileRequestDTO to Profile entity
            CreateMap<ProfileRequestDTO, DiscordCloneBackend.Core.Entities.Profile>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

            // Mapping for Profile to ProfileResponseDTO
            CreateMap<DiscordCloneBackend.Core.Entities.Profile, ProfileResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

            // Mapping Server to ServerResponseDTO(including related entities)
            CreateMap<Server, ServerResponseDTO>()
                .ForMember(dest => dest.ProfileUserName , opt => opt.MapFrom(src => src.Profile.UserName))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members))
                .ForMember(dest => dest.Channels, opt => opt.MapFrom(src => src.Channels));

            CreateMap<ChannelRequestDTO, Channel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.RequesterProfileId))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());


            CreateMap<Channel, ChannelResponseDTO>()
             .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
             .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.Server.Name))
             .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
             .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));


            CreateMap<MessageRequestDTO, Message>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.FileUrl, opt => opt.Ignore())
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.MemberId))
            .ForMember(dest => dest.ChannelId, opt => opt.MapFrom(src => src.ChannelId))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Deleted, opt => opt.Ignore())
            .ForMember(dest => dest.Member, opt => opt.Ignore())
            .ForMember(dest => dest.Channel, opt => opt.Ignore());

            CreateMap<Message, MessageResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.MemberId))
                .ForMember(dest => dest.ChannelId, opt => opt.MapFrom(src => src.ChannelId))
                .ForMember(dest => dest.MemberProfileName, opt => opt.MapFrom(src => src.Member.Profile.Name))
                .ForMember(dest => dest.MemberProfileImageUrl, opt => opt.MapFrom(src => src.Member.Profile.ImageUrl))
                .ForMember(dest => dest.ChannelName, opt => opt.MapFrom(src => src.Channel.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => src.Deleted));

            CreateMap<Member, MemberResponseDTO>()
           .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
           .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile.Name))
           .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.Profile.ImageUrl))
           .ForMember(dest => dest.ProfileEmail, opt => opt.MapFrom(src => src.Profile.Email))
           .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.Server.Name))
           .ForMember(dest => dest.ConversationsInitiated, opt => opt.MapFrom(src =>
               src.ConversationsInitiated.Where(c => c.MemberOneId == src.Id).ToList()))
           .ForMember(dest => dest.ConversationsReceived, opt => opt.MapFrom(src =>
               src.ConversationsReceived.Where(c => c.MemberTwoId == src.Id).ToList()));

            CreateMap<MemberRequestDTO, Member>();

            CreateMap<ConversationRequestDTO, Conversation>()
            .ForMember(dest => dest.MemberOneId, opt => opt.MapFrom(src => src.MemberOneId))
            .ForMember(dest => dest.MemberTwoId, opt => opt.MapFrom(src => src.MemberTwoId))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MemberOne, opt => opt.Ignore())
            .ForMember(dest => dest.MemberTwo, opt => opt.Ignore())
            .ForMember(dest => dest.DirectMessages, opt => opt.Ignore());

            CreateMap<Conversation, ConversationResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MemberOneId, opt => opt.MapFrom(src => src.MemberOneId))
                .ForMember(dest => dest.MemberTwoId, opt => opt.MapFrom(src => src.MemberTwoId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DirectMessages, opt => opt.MapFrom(src => src.DirectMessages));

            CreateMap<DirectMessageRequestDTO, DirectMessage>()
               .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
               .ForMember(dest => dest.FileUrl, opt => opt.Ignore())
               .ForMember(dest => dest.SenderMemberId, opt => opt.MapFrom(src => src.SenderMemberId))
               .ForMember(dest => dest.ReceiverMemberId, opt => opt.MapFrom(src => src.ReceiverMemberId))
               .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.Deleted, opt => opt.Ignore())
               .ForMember(dest => dest.Sender, opt => opt.Ignore())
               .ForMember(dest => dest.Receiver, opt => opt.Ignore())
               .ForMember(dest => dest.Conversation, opt => opt.Ignore());

            CreateMap<DirectMessage, DirectMessageResponseDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
               .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
               .ForMember(dest => dest.SenderMemberId, opt => opt.MapFrom(src => src.SenderMemberId))
               .ForMember(dest => dest.ReceiverMemberId, opt => opt.MapFrom(src => src.ReceiverMemberId))
               .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
               .ForMember(dest => dest.SenderMemberProfileName, opt => opt.MapFrom(src => src.Sender.Profile.UserName))
               .ForMember(dest => dest.ReceiverMemberProfileName, opt => opt.MapFrom(src => src.Receiver.Profile.UserName))
               .ForMember(dest => dest.SenderMemberProfileImageUrl, opt => opt.MapFrom(src => src.Sender.Profile.ImageUrl))
               .ForMember(dest => dest.ReceiverMemberProfileImageUrl, opt => opt.MapFrom(src => src.Receiver.Profile.ImageUrl))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
               .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
               .ForMember(dest => dest.Deleted, opt => opt.MapFrom(src => src.Deleted));
        }
    }
}
