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
        public string ProficiencyLevelText => ProficiencyLevel switch
        {
            ProficiencyLevel.Beginner => "Cơ bản",
            ProficiencyLevel.Intermediate => "Trung bình",
            ProficiencyLevel.Advanced => "Nâng cao",
            ProficiencyLevel.Expert => "Chuyên gia",
            _ => "Không xác định"
        };
        public int YearsExperience { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string? SubjectCode { get; set; }
        public string? SubjectCategory { get; set; }
        public string? SubjectIcon { get; set; }
    }
}
