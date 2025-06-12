using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Booking : BaseEntity
    {
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public long SubjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public LocationType LocationType { get; set; }
        public string? LocationAddress { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;
        public Tutor Tutor { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}
