using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class CertificationCreateDto
    {
        [Required(ErrorMessage = "Tên chứng chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên chứng chỉ không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Tên tổ chức cấp không được vượt quá 200 ký tự")]
        public string? Issuer { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public IFormFile? CertificateFile { get; set; }
    }
    public class CertificationUpdateDto
    {
        [Required(ErrorMessage = "Tên chứng chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên chứng chỉ không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Tên tổ chức cấp không được vượt quá 200 ký tự")]
        public string? Issuer { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public IFormFile? CertificateFile { get; set; }
    }
}