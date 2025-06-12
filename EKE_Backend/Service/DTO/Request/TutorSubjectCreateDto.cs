using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class TutorSubjectCreateDto
    {
        [Required]
        public long SubjectId { get; set; }

        [Required]
        public ProficiencyLevel ProficiencyLevel { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int YearsExperience { get; set; } = 0;
    }
}
