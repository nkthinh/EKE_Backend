using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Review : BaseEntity
    {
        public long TutorId { get; set; }
        public long StudentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public bool IsApproved { get; set; } = true;

        // Navigation Properties
        public Tutor Tutor { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
