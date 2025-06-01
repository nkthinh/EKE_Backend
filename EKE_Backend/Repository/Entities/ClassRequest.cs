using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class ClassRequest
    {
        [Key]
        public int ClassId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public StudentProfile Student { get; set; }

        [ForeignKey("Tutor")]
        public int? TutorId { get; set; }
        public TutorProfile Tutor { get; set; }

        [Required, StringLength(100)]
        public string Subject { get; set; }

        [StringLength(50)]
        public string GradeLevel { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(20)]
        public string Mode { get; set; }

        [StringLength(30)]
        public string Status { get; set; }

        public int FeePerSession { get; set; }
        public int SessionCount { get; set; }

        public ICollection<ClassSession> Sessions { get; set; }
    }

}
