using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PreclearDbContext _db;

        public NotificationRepository(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task<List<Notification>> GetByUserIdAsync(long userId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> FindAsync(long notificationId)
        {
            return await _db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _db.Notifications.Update(notification);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
