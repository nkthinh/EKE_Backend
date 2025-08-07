using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class TutorResponseDto
    {
        public long Id { get; set; }  // ID của tutor trong bảng Tutor
        public long UserId { get; set; }  // UserId của tutor từ bảng User
        public int ExperienceYears { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? ProfileImage { get; set; }  // Ảnh đại diện của tutor
        public string FullName { get; set; } // Tên đầy đủ của tutor
        public ICollection<TutorSubjectDto> Subjects { get; set; } = new List<TutorSubjectDto>(); // Các môn học mà tutor dạy

        // Bạn có thể thêm một số thuộc tính tùy chỉnh nếu cần (ví dụ: IsOnline)
        public bool IsOnline { get; set; }
    }

    public class TutorStatisticsDto
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalStudentsTaught { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal ThisMonthEarnings { get; set; }
        public Dictionary<string, int> BookingsBySubject { get; set; } = new();
        public Dictionary<string, decimal> EarningsByMonth { get; set; } = new();

        public double CompletionRate => TotalBookings > 0
            ? (double)CompletedBookings / TotalBookings * 100
            : 0;
    }

    public class MessageStatisticsDto
    {
        public int TotalMessages { get; set; }
        public int TotalConversations { get; set; }
        public int ActiveConversations { get; set; }
        public int MessagesToday { get; set; }
        public int MessagesThisWeek { get; set; }
        public int MessagesThisMonth { get; set; }
        public double AverageMessagesPerConversation { get; set; }
        public Dictionary<string, int> MessagesByType { get; set; } = new();
        public int TotalUsers { get; set; } // Tổng số người dùng
        public int UnreadMessages { get; set; } // Số tin nhắn chưa đọc
    }
    public class TutorAdminDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<string> Subjects { get; set; } = new();

        public string VerificationStatusText => VerificationStatus switch
        {
            VerificationStatus.Pending => "Đang chờ xác minh",
            VerificationStatus.Verified => "Đã xác minh",
            VerificationStatus.Rejected => "Bị từ chối",
            _ => "Không xác định"
        };
    }

    public class TutorOverviewStatisticsDto
    {
        public int TotalTutors { get; set; }
        public int VerifiedTutors { get; set; }
        public int PendingVerification { get; set; }
        public int RejectedTutors { get; set; }
        public int FeaturedTutors { get; set; }
        public int NewTutorsThisMonth { get; set; }
        public decimal AverageRating { get; set; }
        public decimal AverageHourlyRate { get; set; }
        public Dictionary<string, int> TutorsByCity { get; set; } = new();
        public Dictionary<string, int> TutorsBySubject { get; set; } = new();

        public double VerificationRate => TotalTutors > 0
            ? (double)VerifiedTutors / TotalTutors * 100
            : 0;
    }

    public class TutorRecommendationDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Introduction { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> MatchingSubjects { get; set; } = new();
        public double CompatibilityScore { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
    }
    public class TutorSearchResultDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
        public string? Introduction { get; set; }
        public int ExperienceYears { get; set; }
        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsFeatured { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public List<string> Subjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public int TotalStudentsTaught { get; set; }

        // Computed properties
        public string RatingDisplay => TotalReviews > 0
            ? $"{AverageRating:F1} ⭐ ({TotalReviews} đánh giá)"
            : "Chưa có đánh giá";

        public string HourlyRateText => HourlyRate.HasValue
            ? $"{HourlyRate:N0} VNĐ/giờ"
            : "Thỏa thuận";

        public string ExperienceText => ExperienceYears switch
        {
            0 => "Mới bắt đầu",
            var years when years <= 2 => $"{years} năm kinh nghiệm",
            var years when years <= 5 => $"{years} năm kinh nghiệm",
            var years => $"Hơn {years} năm kinh nghiệm"
        };
    }
}