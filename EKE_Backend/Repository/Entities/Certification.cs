using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Certification : BaseEntity
    {
        public long TutorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Issuer { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }
        public bool IsVerified { get; set; } = false;

        // Navigation Properties
        public Tutor Tutor { get; set; } = null!;
    }
}
