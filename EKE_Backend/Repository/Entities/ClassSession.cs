using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class ClassSession
    {
        [Key]
        public int SessionId { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public ClassRequest Class { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [StringLength(30)]
        public string Status { get; set; }

        [StringLength(500)]
        public string? MeetLink { get; set; }
    }
}
