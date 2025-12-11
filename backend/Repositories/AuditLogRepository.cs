using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PreclearDbContext _db;
        public AuditLogRepository(PreclearDbContext db) => _db = db;

        public async Task<AuditLog> CreateAsync(AuditLog auditLog)
        {
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();
            return auditLog;
        }

        public async Task<List<AuditLog>> GetByShipmentAsync(long shipmentId)
        {
            return await _db.AuditLogs
                .AsNoTracking()
                .Where(a => a.ShipmentId == shipmentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByUserAsync(long userId)
        {
            return await _db.AuditLogs
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
