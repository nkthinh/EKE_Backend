using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Tutor : BaseEntity
    {
        public long UserId { get; set; }
        public int ExperienceYears { get; set; } = 0;
        public string? EducationLevel { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public int? GraduationYear { get; set; }
        public string? TeachingStyle { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Availability { get; set; }
        public string? Introduction { get; set; }
        public int TotalStudentsTaught { get; set; } = 0;
        public decimal AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

        // Navigation Properties
        public User User { get; set; } = null!;
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();
        public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
        public ICollection<Match> MatchesAsTutor { get; set; } = new List<Match>();
        public ICollection<Booking> BookingsAsTutor { get; set; } = new List<Booking>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    }
}
