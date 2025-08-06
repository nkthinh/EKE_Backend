using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public long UserId { get; set; }
        public string OrderCode { get; set; }
        public string PaymentUrl { get; set; }
        public string QRCodeUrl { get; set; }
        public string PayOSTransactionId { get; set; }
        public string PayOSResponse { get; set; }
        public string Status { get; set; } // pending, success, failed, etc.
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public User User { get; set; }
    }
}
