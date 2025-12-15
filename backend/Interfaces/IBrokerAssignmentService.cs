using PreClear.Api.Models;
using System.Threading.Tasks;

namespace PreClear.Api.Interfaces
{
    public interface IBrokerAssignmentService
    {
        Task<Shipment> AssignBrokerAsync(long shipmentId, long brokerId);
        Task<List<Shipment>> GetShipmentsByBrokerAsync(long brokerId);
    }
}
