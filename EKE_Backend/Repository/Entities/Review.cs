using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public ClassRequest Class { get; set; }

        [ForeignKey("Reviewer")]
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }

        [Range(1, 5)]
        public float Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
