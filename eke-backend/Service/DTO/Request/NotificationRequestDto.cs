using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class CreateNotificationRequestDto
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string? Content { get; set; }

        public string? Data { get; set; } // JSON data for additional info
    }
    public class MarkNotificationAsReadRequestDto
    {
        [Required]
        public long NotificationId { get; set; }
    }

    public class BulkNotificationRequestDto
    {
        [Required]
        public List<long> UserIds { get; set; } = new();

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string? Content { get; set; }

        public string? Data { get; set; }
    }
}
