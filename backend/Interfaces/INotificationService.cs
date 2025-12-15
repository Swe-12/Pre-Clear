using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAsync(long userId);
        Task<Notification> MarkReadAsync(long notificationId);
    }
}
