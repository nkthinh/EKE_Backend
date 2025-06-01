using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class TutorProfile
    {
        [Key]
        public int TutorId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [StringLength(100)]
        public string School { get; set; }

        [StringLength(100)]
        public string Major { get; set; }

        [StringLength(100)]
        public string Certificate { get; set; }

        [StringLength(20)]
        public string TeachingStyle { get; set; }

        [MaxLength(1000)]
        public string SelfIntroduction { get; set; }

        public ICollection<TeachingSubject> TeachingSubjects { get; set; }
    }

}
