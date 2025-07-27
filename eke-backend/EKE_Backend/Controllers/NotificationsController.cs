using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Notifications;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Create a notification (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationResponseDto>> CreateNotification([FromBody] CreateNotificationRequestDto request)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(request);
                return Ok(notification);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, new { message = "An error occurred while creating the notification" });
            }
        }

        /// <summary>
        /// Create bulk notifications (Admin only)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> CreateBulkNotification([FromBody] BulkNotificationRequestDto request)
        {
            try
            {
                var notifications = await _notificationService.CreateBulkNotificationAsync(request);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk notifications");
                return StatusCode(500, new { message = "An error occurred while creating bulk notifications" });
            }
        }

        /// <summary>
        /// Get all notifications for current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetMyNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        /// <summary>
        /// Get unread notifications for current user
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUnreadNotificationsByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving unread notifications" });
            }
        }

        /// <summary>
        /// Get notification by ID
        /// </summary>
        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotificationById(long notificationId)
        {
            try
            {
                var notification = await _notificationService.GetNotificationByIdAsync(notificationId);

                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found" });
                }

                var userId = GetCurrentUserId();
                if (notification.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid("You can only access your own notifications");
                }

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
                return StatusCode(500, new { message = "An error occurred while retrieving the notification" });
            }
        }

        /// <summary>
        /// Get notifications by type for current user
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetNotificationsByType(NotificationType type)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsByUserIdAndTypeAsync(userId, type);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type {Type} for current user", type);
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        /// <summary>
        /// Get notification summary for current user
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<NotificationSummaryDto>> GetNotificationSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _notificationService.GetNotificationSummaryByUserIdAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification summary for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving notification summary" });
            }
        }

        /// <summary>
        /// Get unread count for current user
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _notificationService.GetUnreadCountByUserIdAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving unread count" });
            }
        }

        /// <summary>
        /// Get recent notifications for current user
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetRecentNotifications([FromQuery] int days = 30)
        {
            try
            {
                if (days <= 0 || days > 365)
                {
                    return BadRequest(new { message = "Days must be between 1 and 365" });
                }

                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetRecentNotificationsByUserIdAsync(userId, days);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent notifications for current user");
                return StatusCode(500, new { message = "An error occurred while retrieving recent notifications" });
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{notificationId}/read")]
        public async Task<ActionResult> MarkNotificationAsRead(long notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, userId);

                if (!result)
                {
                    return NotFound(new { message = "Notification not found or access denied" });
                }

                return Ok(new { message = "Notification marked as read successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
                return StatusCode(500, new { message = "An error occurred while marking notification as read" });
            }
        }

        /// <summary>
        /// Mark all notifications as read for current user
        /// </summary>
        [HttpPut("mark-all-read")]
        public async Task<ActionResult> MarkAllNotificationsAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

                if (!result)
                {
                    return Ok(new { message = "No unread notifications found" });
                }

                return Ok(new { message = "All notifications marked as read successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for current user");
                return StatusCode(500, new { message = "An error occurred while marking notifications as read" });
            }
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> DeleteNotification(long notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.DeleteNotificationAsync(notificationId, userId);

                if (!result)
                {
                    return NotFound(new { message = "Notification not found or access denied" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", notificationId);
                return StatusCode(500, new { message = "An error occurred while deleting the notification" });
            }
        }

        /// <summary>
        /// Get all notifications by type (Admin only)
        /// </summary>
        [HttpGet("admin/type/{type}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetAllNotificationsByType(NotificationType type)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByTypeAsync(type);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all notifications by type {Type}", type);
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        /// <summary>
        /// Get notifications for a specific user (Admin only)
        /// </summary>
        [HttpGet("admin/user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetNotificationsByUserId(long userId)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving notifications" });
            }
        }

        /// <summary>
        /// Get notification summary for a specific user (Admin only)
        /// </summary>
        [HttpGet("admin/user/{userId}/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationSummaryDto>> GetNotificationSummaryByUserId(long userId)
        {
            try
            {
                var summary = await _notificationService.GetNotificationSummaryByUserIdAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification summary for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving notification summary" });
            }
        }

        /// <summary>
        /// Delete old notifications (Admin only)
        /// </summary>
        [HttpDelete("admin/cleanup")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteOldNotifications([FromQuery] int daysOld = 90)
        {
            try
            {
                if (daysOld <= 0)
                {
                    return BadRequest(new { message = "Days old must be greater than 0" });
                }

                var result = await _notificationService.DeleteOldNotificationsAsync(daysOld);

                if (!result)
                {
                    return Ok(new { message = "No old notifications found to delete" });
                }

                return Ok(new { message = $"Successfully deleted notifications older than {daysOld} days" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old notifications");
                return StatusCode(500, new { message = "An error occurred while deleting old notifications" });
            }
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }
    }
}
