using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class ReviewResponseDto
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public string TutorName { get; set; } = string.Empty;
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ReviewStatisticsDto
    {
        public long TutorId { get; set; }
        public string TutorName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }
}
