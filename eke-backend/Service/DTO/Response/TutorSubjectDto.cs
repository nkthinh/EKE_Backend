using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class TutorSubjectDto
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public long SubjectId { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public int YearsExperience { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public SubjectResponseDto Subject { get; set; } = null!;
    }
 
}
