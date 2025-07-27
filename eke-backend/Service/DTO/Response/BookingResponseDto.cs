using Repository.Enums;
using System;

namespace Application.DTOs
{
    public class BookingResponseDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public long SubjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public LocationType LocationType { get; set; }
        public string? LocationAddress { get; set; }
        public BookingStatus Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public string? StudentName { get; set; }
        public string? TutorName { get; set; }
        public string? SubjectName { get; set; }
        public decimal? TutorHourlyRate { get; set; }
        public double DurationHours { get; set; }
    }
    public class BookingScheduleDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public long SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; }
        public LocationType LocationType { get; set; }
        public string? LocationAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ReviewResponseDto
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? StudentAvatar { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayName => IsAnonymous ? "Ẩn danh" : StudentName;
    }

   
}