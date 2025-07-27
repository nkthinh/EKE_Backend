using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Notifications
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(long userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(long userId);
        Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(NotificationType type);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAndTypeAsync(long userId, NotificationType type);
        Task<int> GetUnreadCountByUserIdAsync(long userId);
        Task<bool> MarkAsReadAsync(long notificationId);
        Task<bool> MarkAllAsReadByUserIdAsync(long userId);
        Task<IEnumerable<Notification>> GetRecentNotificationsByUserIdAsync(long userId, int days = 30);
        Task<bool> DeleteOldNotificationsAsync(int daysOld = 90);
    }
}
