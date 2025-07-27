using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class SendMessageDto
    {
        [Required]
        public long ConversationId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Tin nhắn không được vượt quá 1000 ký tự")]
        public string Content { get; set; } = string.Empty;

        public MessageType MessageType { get; set; } = MessageType.Text;

        public string? FileUrl { get; set; }
    }

    public class MarkAsReadDto
    {
        [Required]
        public long ConversationId { get; set; }

        public long? LastMessageId { get; set; }
    }

    public class OnlineStatusDto
    {
        public bool IsOnline { get; set; }
    }
}
