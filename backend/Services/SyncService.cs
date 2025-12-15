using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class SyncService : ISyncService
    {
        private readonly PreclearDbContext _db;

        public SyncService(PreclearDbContext db)
        {
            _db = db;
        }

        // Simulate pulling data from an external API and upsert shipments
        public async Task<SyncResult> RunSyncAsync()
        {
            // Simulated external payload: create 3 shipments, update 1 if exists
            var now = DateTime.UtcNow;
            var simulated = new[]
            {
                new { ReferenceId = $"SYNC-{now:yyyyMMddHHmmss}-A", ProductName = "Widget A", Origin = "CN", Destination = "US", Value = 120.00m },
                new { ReferenceId = $"SYNC-{now:yyyyMMddHHmmss}-B", ProductName = "Widget B", Origin = "IN", Destination = "UK", Value = 220.50m },
                new { ReferenceId = $"SYNC-{now:yyyyMMddHHmmss}-C", ProductName = "Widget C", Origin = "DE", Destination = "US", Value = 44.99m }
            };

            int imported = 0, updated = 0;

            foreach (var item in simulated)
            {
                // Try to find by ReferenceId
                var existing = await _db.Shipments.FirstOrDefaultAsync(s => s.ReferenceId == item.ReferenceId);
                if (existing == null)
                {
                    var s = new Shipment
                    {
                        ReferenceId = item.ReferenceId,
                        ShipmentName = item.ProductName,
                        Status = "draft",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        TotalValue = item.Value,
                        Currency = "USD"
                    };
                    _db.Shipments.Add(s);
                    imported++;
                }
                else
                {
                    existing.ShipmentName = item.ProductName;
                    existing.TotalValue = item.Value;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _db.Shipments.Update(existing);
                    updated++;
                }
            }

            await _db.SaveChangesAsync();

            // Record SyncLog
            var log = new SyncLog
            {
                RunAt = DateTime.UtcNow,
                ImportedCount = imported,
                UpdatedCount = updated,
                Details = $"Imported: {imported}, Updated: {updated}"
            };
            _db.SyncLogs.Add(log);
            await _db.SaveChangesAsync();

            return new SyncResult { Imported = imported, Updated = updated, Details = log.Details };
        }
    }
}

