using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IShipmentService
    {
        Task<Shipment> CreateShipmentAsync(CreateShipmentRequest request);
        Task<IEnumerable<Shipment>> GetShipmentsByUserAsync(long userId);
        Task<Shipment?> GetShipmentByIdAsync(long id);
        Task<Shipment?> UpdateShipmentAsync(long id, UpdateShipmentRequest request);
        Task<bool> ChangeStatusAsync(long id, string newStatus, long? performedBy = null, string? notes = null);
    }
}
 
