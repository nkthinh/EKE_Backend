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
}