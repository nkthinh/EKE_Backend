using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class NotificationResponseDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class NotificationSummaryDto
    {
        public long UserId { get; set; }
        public int TotalNotifications { get; set; }
        public int UnreadCount { get; set; }
        public int ReadCount { get; set; }
        public DateTime? LastNotificationDate { get; set; }
    }
}
