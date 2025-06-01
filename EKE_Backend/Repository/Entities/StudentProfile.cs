using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class StudentProfile
    {
        [Key]
        public int StudentId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [StringLength(50)]
        public string AcademicLevel { get; set; }

        public ICollection<ClassRequest> ClassRequests { get; set; }
    }


}
