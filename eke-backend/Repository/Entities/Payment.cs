//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Repository.Entities
//{
//    [Table("Payments")]
//    public class Payment : BaseEntity
//    {
//        [Required]
//        public long BookingId { get; set; }

//        [Required]
//        public long PayerId { get; set; } // StudentId who pays

//        [Required]
//        public long ReceiverId { get; set; } // TutorId who receives

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal Amount { get; set; }

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal PlatformFee { get; set; } // Commission for platform

//        [Required]
//        [Column(TypeName = "decimal(12,2)")]
//        public decimal NetAmount { get; set; } // Amount after platform fee

//        [Required]
//        public PaymentMethod PaymentMethod { get; set; }

//        [Required]
//        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

//        [MaxLength(100)]
//        public string? TransactionId { get; set; } // External payment gateway transaction ID

//        [MaxLength(50)]
//        public string? PaymentGateway { get; set; } // VNPay, Momo, etc.

//        [MaxLength(500)]
//        public string? Description { get; set; }

//        public DateTime? ProcessedAt { get; set; }
//        public DateTime? CompletedAt { get; set; }
//        public DateTime? FailedAt { get; set; }

//        [MaxLength(1000)]
//        public string? FailureReason { get; set; }

//        [MaxLength(2000)]
//        public string? GatewayResponse { get; set; } // JSON response from payment gateway

//        // Navigation properties
//        [ForeignKey("BookingId")]
//        public virtual Booking Booking { get; set; } = null!;

//        [ForeignKey("PayerId")]
//        public virtual User Payer { get; set; } = null!;

//        [ForeignKey("ReceiverId")]
//        public virtual User Receiver { get; set; } = null!;

//        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
//    }
//}
