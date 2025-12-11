using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActionAsync(long? userId, long? shipmentId, string action, string? description);
        Task<List<AuditLog>> GetShipmentAuditAsync(long shipmentId);
        Task<List<AuditLog>> GetUserAuditAsync(long userId);
    }
}
