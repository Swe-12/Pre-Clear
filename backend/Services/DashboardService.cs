using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly PreclearDbContext _db;

        public DashboardService(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardSummary> GetSummaryAsync()
        {
            var total = await _db.Shipments.AsNoTracking().LongCountAsync();

            // Define approved and pending status sets (string comparisons)
            var approvedStatuses = new[] { "pre_cleared", "token_generated", "booked", "in_transit", "delivered" };
            var pendingStatuses = new[] { "draft", "pending_validation", "requires_resolution" };

            var approved = await _db.Shipments.AsNoTracking().Where(s => approvedStatuses.Contains(s.Status)).LongCountAsync();
            var pending = await _db.Shipments.AsNoTracking().Where(s => pendingStatuses.Contains(s.Status)).LongCountAsync();

            var exceptionsCount = await _db.ShipmentExceptions.AsNoTracking().LongCountAsync();

            return new DashboardSummary
            {
                TotalShipments = total,
                ApprovedShipments = approved,
                PendingShipments = pending,
                ExceptionsCount = exceptionsCount
            };
        }
    }
}
