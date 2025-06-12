using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Notification : BaseEntity
    {
        public long UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? Data { get; set; } // JSON data for additional info
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
