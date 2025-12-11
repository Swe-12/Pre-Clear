using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> CreateAsync(AuditLog auditLog);
        Task<List<AuditLog>> GetByShipmentAsync(long shipmentId);
        Task<List<AuditLog>> GetByUserAsync(long userId);
    }
}
