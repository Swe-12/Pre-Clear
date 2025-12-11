using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IExceptionRepository
    {
        Task AddAsync(ShipmentException exception);
        Task UpdateAsync(ShipmentException exception);
        Task<ShipmentException> FindAsync(long exceptionId);
        Task<List<ShipmentException>> GetByShipmentIdAsync(long shipmentId);
        Task<List<ShipmentException>> GetOpenByShipmentAsync(long shipmentId);
        Task SaveAsync();
    }
}
