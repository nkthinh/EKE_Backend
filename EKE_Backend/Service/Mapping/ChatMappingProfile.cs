using AutoMapper;
using Repository.Entities;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapping
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            // Conversation mappings
            CreateMap<Conversation, ConversationResponseDto>()
                .ForMember(dest => dest.Partner, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.GetLastMessage() != null ? src.GetLastMessage()!.Content : null))
                .ForMember(dest => dest.UnreadCount, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => false)); // TODO: Implement

            // Message mappings
            CreateMap<Message, MessageResponseDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.SenderAvatar, opt => opt.MapFrom(src => src.Sender.ProfileImage))
                .ForMember(dest => dest.IsMine, opt => opt.Ignore()); // Will be set manually

            // User mappings
            CreateMap<User, UserBasicInfoDto>()
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => false)) // TODO: Implement online status
                .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => src.UpdatedAt));

            // Conversation to ConversationBasicDto
            CreateMap<Conversation, ConversationBasicDto>()
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.GetLastMessage() != null ? src.GetLastMessage()!.Content : null))
                .ForMember(dest => dest.UnreadCount, opt => opt.Ignore()); // Will be set manually
        }
    }
}
