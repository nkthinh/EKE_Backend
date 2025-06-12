using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Student : BaseEntity
    {
        public long UserId { get; set; }
        public string? SchoolName { get; set; }
        public string? GradeLevel { get; set; }
        public string? LearningGoals { get; set; }
        public string? PreferredSchedule { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public LearningStyle? LearningStyle { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public ICollection<SwipeAction> SwipeActions { get; set; } = new List<SwipeAction>();
        public ICollection<Match> MatchesAsStudent { get; set; } = new List<Match>();
        public ICollection<Booking> BookingsAsStudent { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
