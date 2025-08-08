using Repository.Enums;
using System;

namespace Service.DTO.Response
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

        // Thêm các trường liên quan đến Student, Tutor và Conversation
        public UserBasicInfoDto Student { get; set; } // DTO cho học sinh
        public UserBasicInfoDto Tutor { get; set; } // DTO cho gia sư
        public ConversationResponseDto Conversation { get; set; } // DTO cho cuộc trò chuyện
    }

}