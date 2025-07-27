using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationRequestDto request)
        {
            try
            {
                // Verify user exists
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                var notification = new Notification
                {
                    UserId = request.UserId,
                    Type = request.Type,
                    Title = request.Title,
                    Content = request.Content,
                    Data = request.Data,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.CompleteAsync();

                // Get the notification with navigation properties loaded
                var createdNotification = await _unitOfWork.Notifications.GetByIdAsync(notification.Id);
                return MapToResponseDto(createdNotification!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", request.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> CreateBulkNotificationAsync(BulkNotificationRequestDto request)
        {
            try
            {
                var notifications = new List<Notification>();
                var results = new List<NotificationResponseDto>();

                // Begin transaction for bulk operation
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    foreach (var userId in request.UserIds)
                    {
                        // Verify user exists
                        var user = await _unitOfWork.Users.GetByIdAsync(userId);
                        if (user == null)
                        {
                            _logger.LogWarning("User {UserId} not found, skipping notification", userId);
                            continue;
                        }

                        var notification = new Notification
                        {
                            UserId = userId,
                            Type = request.Type,
                            Title = request.Title,
                            Content = request.Content,
                            Data = request.Data,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.Notifications.AddAsync(notification);
                        notifications.Add(notification);
                    }

                    if (notifications.Any())
                    {
                        await _unitOfWork.CompleteAsync();
                        await _unitOfWork.CommitTransactionAsync();

                        // Load notifications with navigation properties
                        foreach (var notification in notifications)
                        {
                            var loadedNotification = await _unitOfWork.Notifications.GetByIdAsync(notification.Id);
                            if (loadedNotification != null)
                            {
                                results.Add(MapToResponseDto(loadedNotification));
                            }
                        }
                    }
                    else
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk notifications");
                throw;
            }
        }

        public async Task<NotificationResponseDto?> GetNotificationByIdAsync(long notificationId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
                if (notification == null)
                {
                    return null;
                }

                return MapToResponseDto(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserIdAsync(long userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserIdAsync(userId);
                return notifications.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsByUserIdAsync(long userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsByUserIdAsync(userId);
                return notifications.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByTypeAsync(NotificationType type)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetNotificationsByTypeAsync(type);
                return notifications.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications by type {Type}", type);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserIdAndTypeAsync(long userId, NotificationType type)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserIdAndTypeAsync(userId, type);
                return notifications.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user {UserId} and type {Type}", userId, type);
                throw;
            }
        }

        public async Task<NotificationSummaryDto> GetNotificationSummaryByUserIdAsync(long userId)
        {
            try
            {
                var allNotifications = await _unitOfWork.Notifications.GetNotificationsByUserIdAsync(userId);
                var notificationsList = allNotifications.ToList();

                var unreadCount = await _unitOfWork.Notifications.GetUnreadCountByUserIdAsync(userId);

                return new NotificationSummaryDto
                {
                    UserId = userId,
                    TotalNotifications = notificationsList.Count,
                    UnreadCount = unreadCount,
                    ReadCount = notificationsList.Count - unreadCount,
                    LastNotificationDate = notificationsList.Any() ? notificationsList.Max(n => n.CreatedAt) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification summary for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(long notificationId, long userId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
                if (notification == null || notification.UserId != userId)
                {
                    return false;
                }

                var result = await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
                if (result)
                {
                    await _unitOfWork.CompleteAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
                throw;
            }
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(long userId)
        {
            try
            {
                var result = await _unitOfWork.Notifications.MarkAllAsReadByUserIdAsync(userId);
                if (result)
                {
                    await _unitOfWork.CompleteAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteNotificationAsync(long notificationId, long userId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
                if (notification == null || notification.UserId != userId)
                {
                    return false;
                }

                // Use Remove method from BaseRepository that accepts entity
                _unitOfWork.Notifications.Remove(notification);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}", notificationId, userId);
                throw;
            }
        }

        public async Task<bool> DeleteOldNotificationsAsync(int daysOld = 90)
        {
            try
            {
                var result = await _unitOfWork.Notifications.DeleteOldNotificationsAsync(daysOld);
                if (result)
                {
                    await _unitOfWork.CompleteAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old notifications older than {DaysOld} days", daysOld);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetRecentNotificationsByUserIdAsync(long userId, int days = 30)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetRecentNotificationsByUserIdAsync(userId, days);
                return notifications.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent notifications for user {UserId} within {Days} days", userId, days);
                throw;
            }
        }

        public async Task<int> GetUnreadCountByUserIdAsync(long userId)
        {
            try
            {
                return await _unitOfWork.Notifications.GetUnreadCountByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
                throw;
            }
        }

        private NotificationResponseDto MapToResponseDto(Notification notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                UserName = notification.User?.FullName ?? "Unknown",
                Type = notification.Type,
                TypeName = notification.Type.ToString(),
                Title = notification.Title,
                Content = notification.Content,
                Data = notification.Data,
                IsRead = notification.IsRead,
                ReadAt = notification.ReadAt,
                CreatedAt = notification.CreatedAt,
                UpdatedAt = notification.UpdatedAt
            };
        }
    }
}