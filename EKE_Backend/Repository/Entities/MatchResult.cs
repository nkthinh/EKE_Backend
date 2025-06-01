using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class MatchResult
    {
        [Key]
        public int MatchId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public StudentProfile Student { get; set; }

        [ForeignKey("Tutor")]
        public int TutorId { get; set; }
        public User Tutor { get; set; }

        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
