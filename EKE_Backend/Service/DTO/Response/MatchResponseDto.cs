using Repository.Enums;
using System;

namespace Application.DTOs
{
    public class MatchResponseDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public MatchStatus Status { get; set; }
        public DateTime MatchedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public string? StudentName { get; set; }
        public string? TutorName { get; set; }
        public int ConversationCount { get; set; }
    }
}