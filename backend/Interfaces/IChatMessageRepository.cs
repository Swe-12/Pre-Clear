using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IChatMessageRepository
    {
        Task AddAsync(ShipmentMessage message);
        Task<List<ShipmentMessage>> GetByShipmentIdAsync(long shipmentId);
        Task SaveAsync();
    }
}
