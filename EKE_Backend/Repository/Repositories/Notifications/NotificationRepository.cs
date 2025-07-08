using Microsoft.EntityFrameworkCore;
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
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByTypeAsync(NotificationType type)
        {
            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAndTypeAsync(long userId, NotificationType type)
        {
            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.UserId == userId && n.Type == type)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountByUserIdAsync(long userId)
        {
            return await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkAsReadAsync(long notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        public async Task<bool> MarkAllAsReadByUserIdAsync(long userId)
        {
            var notifications = await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
                return false;

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            return true;
        }

        public async Task<IEnumerable<Notification>> GetRecentNotificationsByUserIdAsync(long userId, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return await _dbSet
                .Include(n => n.User)
                .Where(n => n.UserId == userId && n.CreatedAt >= cutoffDate)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteOldNotificationsAsync(int daysOld = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

            var oldNotifications = await _dbSet
                .Where(n => n.CreatedAt < cutoffDate)
                .ToListAsync();

            if (!oldNotifications.Any())
                return false;

            _dbSet.RemoveRange(oldNotifications);
            return true;
        }

        // Override GetByIdAsync to include navigation properties
        public override async Task<Notification?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}