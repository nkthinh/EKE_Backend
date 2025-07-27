using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class TutorSignUpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Tutor-specific fields
        [Required(ErrorMessage = "Trình độ học vấn là bắt buộc")]
        public string EducationLevel { get; set; } = string.Empty;

        public string? University { get; set; }
        public string? Major { get; set; }
        public int ExperienceYears { get; set; } = 0;
        public decimal? HourlyRate { get; set; }
        public string? Introduction { get; set; }

        // Subjects they can teach
        public List<long> SubjectIds { get; set; } = new List<long>();
    }
}