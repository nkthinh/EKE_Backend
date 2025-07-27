using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class TutorCreateDto
    {
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int ExperienceYears { get; set; } = 0;

        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }

        [Range(1950, 2030, ErrorMessage = "Năm tốt nghiệp không hợp lệ")]
        public int? GraduationYear { get; set; }

        public string? TeachingStyle { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá giờ phải >= 0")]
        public decimal? HourlyRate { get; set; }

        public string? Availability { get; set; }
        public string? Introduction { get; set; }

        // Subjects and certifications can be added later
        public ICollection<TutorSubjectCreateDto> Subjects { get; set; } = new List<TutorSubjectCreateDto>();
    }
}
