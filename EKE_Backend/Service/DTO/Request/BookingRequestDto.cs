using Repository.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BookingRequestDto
    {
        [Required]
        public long StudentId { get; set; }

        [Required]
        public long TutorId { get; set; }

        [Required]
        public long SubjectId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public LocationType LocationType { get; set; }

        public string? LocationAddress { get; set; }

        public BookingStatus? Status { get; set; }

        public decimal? TotalAmount { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}