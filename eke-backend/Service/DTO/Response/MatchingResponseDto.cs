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

    public class MatchResponseDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public MatchStatus Status { get; set; }
        public DateTime MatchedAt { get; set; }
        public DateTime LastActivity { get; set; }

        // Navigation data
        public UserBasicInfoDto Student { get; set; } = null!;
        public UserBasicInfoDto Tutor { get; set; } = null!;
        public ConversationBasicDto? Conversation { get; set; }
    }

    public class UserBasicInfoDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
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
