using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IChatMessageService
    {
        Task<ShipmentMessage> SendAsync(long shipmentId, string message, string sender = "user", long? senderId = null);
        Task<List<ShipmentMessage>> GetHistoryAsync(long shipmentId);
    }
}
