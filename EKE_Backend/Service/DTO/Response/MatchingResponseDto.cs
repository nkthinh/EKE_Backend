using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class TutorCardDto
    {
        public long TutorId { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public int ExperienceYears { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Introduction { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public List<SubjectResponseDto> Subjects { get; set; } = new();
        public string? City { get; set; }
        public string? District { get; set; }
        public int Age { get; set; }
    }

  
    public class UserBasicInfoDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;  // Optional, nếu có
        public bool IsOnline { get; set; }  // Trạng thái kết nối real-time
        public DateTime? LastSeen { get; set; }  // Thời gian người dùng lần cuối online
    }


    public class ConversationBasicDto
    {
        public long Id { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

    public class SwipeResultDto
    {
        public bool IsMatch { get; set; }
        public long? MatchId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
