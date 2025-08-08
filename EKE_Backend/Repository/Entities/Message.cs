using Repository.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Message : BaseEntity
    {
        public long ConversationId { get; set; }
        public long SenderId { get; set; }

        public MessageType MessageType { get; set; } = MessageType.Text;
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Navigation Properties
        public Conversation Conversation { get; set; } = null!;
        public User Sender { get; set; } = null!;

    }
}
