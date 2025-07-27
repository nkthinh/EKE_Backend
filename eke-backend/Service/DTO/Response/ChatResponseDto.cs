using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    //public class UserBasicInfoDto
    //{
    //    public long Id { get; set; }
    //    public string FullName { get; set; } = string.Empty;
    //    public string? ProfileImage { get; set; }
    //    public bool IsOnline { get; set; }
    //    public DateTime? LastSeen { get; set; }
    //}
    public class ConversationResponseDto
    {
        public long Id { get; set; }
        public long MatchId { get; set; }
        public long? LastMessageId { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Partner info (the other person in conversation)
        public UserBasicInfoDto Partner { get; set; } = null!;
        public string? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
    }

    public class MessageResponseDto
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long SenderId { get; set; }
        public MessageType MessageType { get; set; }
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Sender info
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public bool IsMine { get; set; } // True if current user sent this message
    }

    public class ChatHistoryDto
    {
        public ConversationResponseDto Conversation { get; set; } = null!;
        public List<MessageResponseDto> Messages { get; set; } = new();
        public bool HasMoreMessages { get; set; }
        public int TotalCount { get; set; }
    }

  
}
