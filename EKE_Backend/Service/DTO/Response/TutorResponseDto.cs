using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class TutorResponseDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public UserResponseDto User { get; set; } = null!;
        public ICollection<TutorSubjectDto> Subjects { get; set; } = new List<TutorSubjectDto>();
    }
}
