using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class TeachingSubject
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tutor")]
        public int TutorId { get; set; }
        public TutorProfile Tutor { get; set; }

        [StringLength(100)]
        public string SubjectName { get; set; }

        [StringLength(50)]
        public string GradeLevel { get; set; }

        public int PricePerSession { get; set; }
    }
}
