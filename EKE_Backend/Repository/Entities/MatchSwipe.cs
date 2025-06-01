using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class MatchSwipe
    {
        [Key]
        public int SwipeId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public StudentProfile Student { get; set; }

        [ForeignKey("Tutor")]
        public int TutorId { get; set; }
        public User Tutor { get; set; }

        public bool IsLiked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
