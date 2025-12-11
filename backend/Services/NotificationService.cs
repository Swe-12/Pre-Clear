using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository repo, ILogger<NotificationService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<Notification>> GetAsync(long userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive", nameof(userId));

            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<Notification> MarkReadAsync(long notificationId)
        {
            if (notificationId <= 0)
                throw new ArgumentException("NotificationId must be positive", nameof(notificationId));

            var notification = await _repo.FindAsync(notificationId);
            if (notification == null)
                throw new InvalidOperationException($"Notification {notificationId} not found");

            notification.IsRead = true;
            await _repo.UpdateAsync(notification);
            _logger.LogInformation($"Notification {notificationId} marked as read");

            return notification;
        }
    }
}
