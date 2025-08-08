using AutoMapper;
using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        // Conversation mappings
        CreateMap<Conversation, ConversationResponseDto>()
            .ForMember(dest => dest.Partner, opt => opt.Ignore()) // Partner will be set manually
            .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.GetLastMessage() != null ? src.GetLastMessage().Content : null))
            .ForMember(dest => dest.UnreadCount, opt => opt.Ignore()) // UnreadCount will be set manually
            .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => false)) // TODO: Implement online status check
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Match.StudentId))  // Map StudentId from Match
            .ForMember(dest => dest.TutorId, opt => opt.MapFrom(src => src.Match.TutorId))      // Map TutorId from Match
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Match.Student.User.FullName))  // Map Student Name from User
            .ForMember(dest => dest.TutorName, opt => opt.MapFrom(src => src.Match.Tutor.User.FullName));   // Map Tutor Name from User

        // **MessageCreateDto to Message mapping**
        CreateMap<MessageCreateDto, Message>()  // Thêm ánh xạ từ MessageCreateDto sang Message
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))//
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType))
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)); // Gán thời gian tạo tin nhắn

        // **Message to MessageResponseDto mapping**
        CreateMap<Message, MessageResponseDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
            .ForMember(dest => dest.SenderAvatar, opt => opt.MapFrom(src => src.Sender.ProfileImage))
            .ForMember(dest => dest.IsMine, opt => opt.Ignore()); // Will be set manually

        // **User mappings**
        CreateMap<User, UserBasicInfoDto>()
            .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => false)) // TODO: Implement online status
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => src.UpdatedAt));

        // Conversation to ConversationBasicDto
        CreateMap<Conversation, ConversationBasicDto>()
            .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.GetLastMessage() != null ? src.GetLastMessage().Content : null))
            .ForMember(dest => dest.UnreadCount, opt => opt.Ignore()); // Will be set manually
    }
}

