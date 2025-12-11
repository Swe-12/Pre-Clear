using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogService(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public async Task LogActionAsync(long? userId, long? shipmentId, string action, string? description)
        {
            var audit = new AuditLog
            {
                UserId = userId,
                ShipmentId = shipmentId,
                Action = action,
                Description = description,
                CreatedAt = System.DateTime.UtcNow
            };

            await _repo.CreateAsync(audit);
        }

        public async Task<List<AuditLog>> GetShipmentAuditAsync(long shipmentId)
        {
            return await _repo.GetByShipmentAsync(shipmentId);
        }

        public async Task<List<AuditLog>> GetUserAuditAsync(long userId)
        {
            return await _repo.GetByUserAsync(userId);
        }
    }
}
