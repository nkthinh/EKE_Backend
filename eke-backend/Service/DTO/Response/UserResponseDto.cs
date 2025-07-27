using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class UserResponseDto
    {
        public long Id { get; set; }
        public long UserId => Id; // For backward compatibility
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? GenderText => Gender switch
        {
            Repository.Enums.Gender.Male => "Nam",
            Repository.Enums.Gender.Female => "Nữ",
            Repository.Enums.Gender.Other => "Khác",
            _ => "Không xác định"
        };
        public UserRole Role { get; set; }
        public string RoleText => Role switch
        {
            UserRole.Student => "Học sinh",
            UserRole.Tutor => "Gia sư",
            UserRole.Admin => "Quản trị viên",
            _ => "Không xác định"
        };
        public string? ProfileImage { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Role-specific information
        public StudentProfileDto? StudentProfile { get; set; }
        public TutorProfileDto? TutorProfile { get; set; }

        // Computed properties
        public int? Age => DateOfBirth.HasValue
            ? DateTime.Now.Year - DateOfBirth.Value.Year - (DateTime.Now.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
            : null;

        public string FullAddress => string.Join(", ", new[] { Address, District, City }.Where(x => !string.IsNullOrEmpty(x)));
    }
}
