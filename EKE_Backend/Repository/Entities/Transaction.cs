using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public ClassRequest Class { get; set; }

        public int Amount { get; set; }

        [StringLength(30)]
        public string Type { get; set; }

        [StringLength(30)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
