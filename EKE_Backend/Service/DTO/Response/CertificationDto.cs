using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class CertificationDto
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Issuer { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }

        // Computed properties
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
        public string StatusText => IsVerified ? "Đã xác minh" : "Chưa xác minh";
        public string ValidityText => ExpiryDate.HasValue
            ? (IsExpired ? "Đã hết hạn" : $"Còn hiệu lực đến {ExpiryDate.Value:dd/MM/yyyy}")
            : "Không có thời hạn";
    }
}
