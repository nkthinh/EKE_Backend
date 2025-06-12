using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class TutorSubject : BaseEntity
    {
        public long TutorId { get; set; }
        public long SubjectId { get; set; }
        public ProficiencyLevel ProficiencyLevel { get; set; }
        public int YearsExperience { get; set; } = 0;

        // Navigation Properties
        public Tutor Tutor { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}
