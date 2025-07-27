//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;

//namespace Repository.Entities
//{
//    [Table("Transactions")]
//    public class Transaction : BaseEntity
//    {
//        [Required]
//        public long UserId { get; set; }

//        public long? PaymentId { get; set; } // Nullable for non-payment transactions

//        [Required]
//        public TransactionType Type { get; set; }

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal Amount { get; set; }

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal BalanceBefore { get; set; }

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal BalanceAfter { get; set; }

//        [Required]
//        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

//        [Required]
//        [MaxLength(500)]
//        public string Description { get; set; } = string.Empty;

//        [MaxLength(100)]
//        public string? ReferenceId { get; set; } // External reference

//        [MaxLength(2000)]
//        public string? Metadata { get; set; } // JSON for additional data

//        // Navigation properties
//        [ForeignKey("UserId")]
//        public virtual User User { get; set; } = null!;

//        [ForeignKey("PaymentId")]
//        public virtual Payment? Payment { get; set; }
//    }
//}
