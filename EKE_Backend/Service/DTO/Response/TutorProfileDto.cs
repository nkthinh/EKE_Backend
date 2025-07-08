using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class TutorProfileDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public int? GraduationYear { get; set; }
        public string? TeachingStyle { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Availability { get; set; }
        public string? Introduction { get; set; }
        public int TotalStudentsTaught { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsFeatured { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string VerificationStatusText => VerificationStatus switch
        {
            VerificationStatus.Pending => "Đang chờ xác minh",
            VerificationStatus.Verified => "Đã xác minh",
            VerificationStatus.Rejected => "Bị từ chối",
            _ => "Không xác định"
        };
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Related data
        public ICollection<TutorSubjectDto> Subjects { get; set; } = new List<TutorSubjectDto>();
        public ICollection<CertificationDto> Certifications { get; set; } = new List<CertificationDto>();
        public ICollection<ReviewSummaryDto> RecentReviews { get; set; } = new List<ReviewSummaryDto>();

        // Computed properties
        public string ExperienceText => ExperienceYears switch
        {
            0 => "Mới bắt đầu",
            var years when years <= 2 => $"{years} năm kinh nghiệm",
            var years when years <= 5 => $"{years} năm kinh nghiệm",
            var years => $"Hơn {years} năm kinh nghiệm"
        };

        public string RatingDisplay => TotalReviews > 0
            ? $"{AverageRating:F1} ⭐ ({TotalReviews} đánh giá)"
            : "Chưa có đánh giá";

        public string HourlyRateText => HourlyRate.HasValue
            ? $"{HourlyRate:N0} VNĐ/giờ"
            : "Thỏa thuận";
    }

}
