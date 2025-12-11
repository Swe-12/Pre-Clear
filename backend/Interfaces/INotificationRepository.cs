using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(long userId);
        Task<Notification> FindAsync(long notificationId);
        Task UpdateAsync(Notification notification);
        Task SaveAsync();
    }
}
