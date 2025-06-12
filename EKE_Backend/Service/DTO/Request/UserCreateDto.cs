using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public UserRole Role { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; }

        // Role-specific profiles
        public StudentCreateDto? StudentProfile { get; set; }
        public TutorCreateDto? TutorProfile { get; set; }
    }
}
