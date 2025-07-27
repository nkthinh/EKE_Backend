using Microsoft.AspNetCore.Http;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    // TutorSearchRequestDto - For search functionality
    public class TutorSearchRequestDto
    {
        public string? Keyword { get; set; }
        public long? SubjectId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }
        public decimal? MinRating { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
        public bool? IsFeatured { get; set; }
        public string? SortBy { get; set; } = "AverageRating"; // AverageRating, HourlyRate, ExperienceYears, CreatedAt
        public string? SortOrder { get; set; } = "DESC"; // ASC, DESC
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // TutorUpdateRequestDto - For profile updates
    public class TutorUpdateRequestDto
    {
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int? ExperienceYears { get; set; }

        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }

        [Range(1950, 2030, ErrorMessage = "Năm tốt nghiệp không hợp lệ")]
        public int? GraduationYear { get; set; }

        public string? TeachingStyle { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá giờ phải >= 0")]
        public decimal? HourlyRate { get; set; }

        public string? Availability { get; set; }

        [StringLength(2000, ErrorMessage = "Giới thiệu không được vượt quá 2000 ký tự")]
        public string? Introduction { get; set; }
    }

    // Admin DTOs
    public class TutorVerificationRequestDto
    {
        [Required]
        public VerificationStatus VerificationStatus { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }

    public class UpdateVerificationStatusDto
    {
        [Required]
        public VerificationStatus VerificationStatus { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }

    // Complete MessageRequestDto (was empty)
    public class MessageRequestDto
    {
        [Required]
        public long ConversationId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Tin nhắn không được vượt quá 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        public MessageType MessageType { get; set; } = MessageType.Text;
        public string? FileUrl { get; set; }
    }
    public class TutorSearchDto
    {
        public string? Keyword { get; set; }
        public long? SubjectId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }
        public decimal? MinRating { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
        public bool? IsFeatured { get; set; }
        public string? SortBy { get; set; } = "AverageRating";
        public string? SortOrder { get; set; } = "DESC";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class TutorAvailabilityUpdateDto
    {
        [StringLength(2000, ErrorMessage = "Lịch rảnh không được vượt quá 2000 ký tự")]
        public string Availability { get; set; } = string.Empty;
    }

    public class TutorUpdateDto
    {
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int? ExperienceYears { get; set; }

        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }

        [Range(1950, 2030, ErrorMessage = "Năm tốt nghiệp không hợp lệ")]
        public int? GraduationYear { get; set; }

        public string? TeachingStyle { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá giờ phải >= 0")]
        public decimal? HourlyRate { get; set; }

        public string? Availability { get; set; }

        [StringLength(2000, ErrorMessage = "Giới thiệu không được vượt quá 2000 ký tự")]
        public string? Introduction { get; set; }
    }

    public class TutorVerificationDto
    {
        [Required]
        public VerificationStatus VerificationStatus { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }

    public class TutorVerificationStatusDto
    {
        [Required]
        public VerificationStatus VerificationStatus { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }

    public class TutorImageUploadDto
    {
        [Required]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
