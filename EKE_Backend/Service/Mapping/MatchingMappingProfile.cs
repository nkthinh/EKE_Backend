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
    public class MatchingMappingProfile : Profile
    {
        public MatchingMappingProfile()
        {
            // Tutor to TutorCardDto
            CreateMap<Tutor, TutorCardDto>()
                .ForMember(dest => dest.TutorId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.User.District))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.User.DateOfBirth)))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.TutorSubjects.Select(ts => ts.Subject)));

            // Match to MatchResponseDto
            CreateMap<Match, MatchResponseDto>()
                .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student.User))
                .ForMember(dest => dest.Tutor, opt => opt.MapFrom(src => src.Tutor.User))
                .ForMember(dest => dest.Conversation, opt => opt.MapFrom(src => src.Conversations.FirstOrDefault()));
        }

        private static int CalculateAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return 0;

            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
