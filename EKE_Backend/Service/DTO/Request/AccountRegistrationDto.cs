// Step 1: Account Registration DTO
using Service.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace Service.DTO.Request
{
    public class AccountRegistrationDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;
    }

    // Step 2: Role Selection DTO
    public class RoleSelectionDto
    {
        [Required(ErrorMessage = "Role là bắt buộc")]
        public string Role { get; set; } = string.Empty; // "Student" hoặc "Tutor"
    }

    // Step 3: Profile Completion DTO
    public class ProfileCompletionDto
    {
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        // Student-specific fields (chỉ điền khi role = Student)
        public string? SchoolName { get; set; }
        public string? GradeLevel { get; set; }
        public string? LearningGoals { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }

        // Tutor-specific fields (chỉ điền khi role = Tutor) 
        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Introduction { get; set; }
        public List<long>? SubjectIds { get; set; } = new List<long>();
    }

    // Response DTO for registration steps
    public class RegistrationStepResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserResponseDto User { get; set; } = null!;
        public string NextStep { get; set; } = string.Empty; // "SelectRole", "CompleteProfile", "Completed"
        public bool IsCompleted { get; set; }
    }
}