using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class ReviewSummaryDto
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public long StudentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }

        // Student info (if not anonymous)
        public string StudentName { get; set; } = string.Empty;
        public string? StudentAvatar { get; set; }

        // Computed properties
        public string RatingStars => new string('⭐', Rating);
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                return timeSpan.TotalDays > 30
                    ? CreatedAt.ToString("dd/MM/yyyy")
                    : timeSpan.TotalDays >= 1
                        ? $"{(int)timeSpan.TotalDays} ngày trước"
                        : timeSpan.TotalHours >= 1
                            ? $"{(int)timeSpan.TotalHours} giờ trước"
                            : "Vừa xong";
            }
        }
    }
}
