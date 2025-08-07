using Application.DTOs;
using AutoMapper;
using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapping
{
    public class CompleteMappingProfile : Profile
    {
        public CompleteMappingProfile()
        {
            // User mappings
            CreateMap<User, UserResponseDto>();
            CreateMap<User, UserBasicInfoDto>()
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeen, opt => opt.Ignore());

            // Message mappings
            CreateMap<SendMessageDto, Message>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.Ignore())
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore());

            CreateMap<Message, MessageResponseDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.SenderAvatar, opt => opt.MapFrom(src => src.Sender.ProfileImage))
                .ForMember(dest => dest.IsMine, opt => opt.Ignore());

            // Conversation mappings
            CreateMap<Conversation, ConversationResponseDto>()
                .ForMember(dest => dest.Partner, opt => opt.Ignore())
                .ForMember(dest => dest.LastMessage, opt => opt.Ignore())
                .ForMember(dest => dest.UnreadCount, opt => opt.Ignore())
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore());

            // Subject mappings
            CreateMap<Subject, SubjectResponseDto>();

            // TutorSubject mappings
            CreateMap<TutorSubject, TutorSubjectDto>()
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject));

            // Certification mappings
            CreateMap<Certification, CertificationDto>();

            // Review mappings
            CreateMap<Review, ReviewSummaryDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.StudentAvatar, opt => opt.MapFrom(src => src.Student.User.ProfileImage));

            // Tutor mappings
            CreateMap<Tutor, TutorResponseDto>()
          .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))  // Lấy UserId từ Tutor
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))  // Lấy FullName từ User
          .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))  // Lấy ProfileImage từ User
          .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects));  // Lấy TutorSubjects


            CreateMap<Tutor, TutorSearchResultDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.User.District))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => ts.Subject.Name)));

            CreateMap<Tutor, TutorAdminDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.BookingsAsTutor.Count))
                .ForMember(dest => dest.TotalEarnings, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => ts.Subject.Name)));

            // Booking mappings
            CreateMap<Booking, BookingScheduleDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name));

            // Update mappings
            CreateMap<TutorUpdateRequestDto, Tutor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<TutorCreateDto, Tutor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
