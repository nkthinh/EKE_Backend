//using AutoMapper;
//using Repository.Entities;
//using Service.DTO.Request;
//using Service.DTO.Response;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Service.Mapping
//{
//    public class UpdatedMessageMappingProfile : Profile
//    {
//        public UpdatedMessageMappingProfile()
//        {
//             Message mappings (updated for existing entity structure)
//            CreateMap<MessageCreateDto, Message>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
//                .ForMember(dest => dest.IsRead, opt => opt.Ignore())
//                .ForMember(dest => dest.ReadAt, opt => opt.Ignore())
//                .ForMember(dest => dest.FileUrl, opt => opt.Ignore());

//            CreateMap<Message, MessageResponseDto>()
//                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
//                .ForMember(dest => dest.SenderAvatar, opt => opt.MapFrom(src => src.Sender.ProfileImage));

//             Conversation mappings
//            CreateMap<Conversation, ConversationResponseDto>()
//                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.GetLastMessage()))
//                .ForMember(dest => dest.ParticipantId, opt => opt.Ignore()) // Will be set in service
//                .ForMember(dest => dest.ParticipantName, opt => opt.Ignore()) // Will be set in service
//                .ForMember(dest => dest.ParticipantAvatar, opt => opt.Ignore()) // Will be set in service
//                .ForMember(dest => dest.ParticipantRole, opt => opt.Ignore()) // Will be set in service
//                .ForMember(dest => dest.UnreadCount, opt => opt.Ignore()) // Will be calculated in service
//                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
//        }
//    }

//    public class UpdatedTutorMappingProfile : Profile
//    {
//        public UpdatedTutorMappingProfile()
//        {
//             Tutor search mappings (updated for existing entity structure)
//            CreateMap<Tutor, TutorSearchResultDto>()
//                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
//                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
//                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
//                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
//                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.User.District))
//                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => ts.Subject.Name)));

//             Tutor profile mappings
//            CreateMap<Tutor, TutorProfileDto>()
//                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
//                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
//                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
//                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
//                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.User.Bio))
//                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
//                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.User.District))
//                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address));

//             Tutor update mappings
//            CreateMap<TutorUpdateDto, Tutor>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.UserId, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

//             Review mappings (updated for existing entity structure)
//            CreateMap<Review, ReviewResponseDto>()
//                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
//                    src.IsAnonymous ? "Ẩn danh" : src.Student.User.FullName))
//                .ForMember(dest => dest.StudentAvatar, opt => opt.MapFrom(src =>
//                    src.IsAnonymous ? null : src.Student.User.ProfileImage));

//             Booking mappings
//            CreateMap<Booking, BookingScheduleDto>()
//                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
//                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name));
//        }
//    }
//}
