using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using AutoMapper;
using Service.DTO;

namespace Service.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // Ánh xạ từ UserCreateDto sang User
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Không ánh xạ Id vì đây là giá trị tự động sinh
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Không ánh xạ PasswordHash vì nó cần được băm sau

            // Ánh xạ từ UserUpdateDto sang User
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Không ánh xạ Id
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Không ánh xạ PasswordHash

            // Ánh xạ từ Tutor sang TutorSearchResultDto
            // Ánh xạ từ Tutor sang TutorSearchResultDto
            CreateMap<Tutor, TutorSearchResultDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)) // Lấy UserId từ User
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName)) // Lấy FullName từ User
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email)) // Lấy Email từ User
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone)) // Lấy Phone từ User
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage)) // Lấy ProfileImage từ User
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City)) // Lấy City từ User
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.User.District)) // Lấy District từ User
                .ForMember(dest => dest.Introduction, opt => opt.MapFrom(src => src.Introduction)) // Giới thiệu từ Tutor
                .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.ExperienceYears)) // Kinh nghiệm từ Tutor
                .ForMember(dest => dest.EducationLevel, opt => opt.MapFrom(src => src.EducationLevel)) // Trình độ học vấn từ Tutor
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University)) // Đại học từ Tutor
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major)) // Chuyên ngành từ Tutor
                .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.HourlyRate)) // Giờ học từ Tutor
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating)) // Đánh giá trung bình từ Tutor
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.TotalReviews)) // Số đánh giá từ Tutor
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.IsFeatured)) // Tính năng đặc biệt từ Tutor
                .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => src.VerificationStatus)) // Trạng thái xác minh từ Tutor
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => ts.Subject.Name).ToList())) // Môn học từ TutorSubjects
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt)) // Ngày tạo từ Tutor
                .ForMember(dest => dest.TotalStudentsTaught, opt => opt.MapFrom(src => src.TotalStudentsTaught)); // Tổng số học viên từ Tutor
            // Ánh xạ từ User sang UserResponseDto
            CreateMap<User, UserResponseDto>();
        }
    }
}
