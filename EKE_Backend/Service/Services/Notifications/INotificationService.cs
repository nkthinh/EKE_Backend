using Repository.Enums;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Notifications
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationRequestDto request);
        Task<IEnumerable<NotificationResponseDto>> CreateBulkNotificationAsync(BulkNotificationRequestDto request);
        Task<NotificationResponseDto?> GetNotificationByIdAsync(long notificationId);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserIdAsync(long userId);
        Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsByUserIdAsync(long userId);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsByTypeAsync(NotificationType type);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserIdAndTypeAsync(long userId, NotificationType type);
        Task<NotificationSummaryDto> GetNotificationSummaryByUserIdAsync(long userId);
        Task<bool> MarkNotificationAsReadAsync(long notificationId, long userId);
        Task<bool> MarkAllNotificationsAsReadAsync(long userId);
        Task<bool> DeleteNotificationAsync(long notificationId, long userId);
        Task<bool> DeleteOldNotificationsAsync(int daysOld = 90);
        Task<IEnumerable<NotificationResponseDto>> GetRecentNotificationsByUserIdAsync(long userId, int days = 30);
        Task<int> GetUnreadCountByUserIdAsync(long userId);
    }

}
