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
    public class ShipmentService : IShipmentService
    {
        private readonly PreclearDbContext _db;

        private static readonly Dictionary<string, string[]> _allowedTransitions = new()
        {
            { "draft", new[] { "submitted" } },
            { "submitted", new[] { "under_review", "on_hold" } },
            { "under_review", new[] { "approved", "rejected", "on_hold" } },
            { "approved", new[] { "completed" } },
            { "rejected", new[] { "reopen" } },
            { "on_hold", new[] { "reopen" } },
            { "reopen", new[] { "under_review" } }
        };

        public ShipmentService(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task<Shipment> CreateShipmentAsync(CreateShipmentRequest request)
        {
            var now = DateTime.UtcNow;
            var shipment = new Shipment
            {
                ReferenceId = $"SHP-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                ShipmentName = request.ProductName,
                Status = "draft",
                TotalValue = request.TotalValue,
                TotalWeight = request.TotalWeight,
                Currency = request.Currency,
                CreatedAt = now,
                UpdatedAt = now,
                PreclearToken = null,
                CreatedBy = request.UserId
            };

            // Store product-level summary on shipment_name and also create ShipmentItem if needed
            await _db.Shipments.AddAsync(shipment);
            await _db.SaveChangesAsync();

            // persist provided items
            if (request.Items != null && request.Items.Count > 0)
            {
                foreach (var it in request.Items)
                {
                    var item = new ShipmentItem
                    {
                        ShipmentId = shipment.Id,
                        ProductName = it.ProductName,
                        Description = it.Description,
                        HsCode = it.HsCode,
                        Quantity = it.Quantity,
                        UnitPrice = it.UnitPrice ?? 0,
                        TotalValue = (it.UnitPrice ?? 0) * it.Quantity,
                        Weight = it.Weight
                    };
                    await _db.ShipmentItems.AddAsync(item);
                }
                await _db.SaveChangesAsync();
            }

            // create a shipper party for quick reference
            if (!string.IsNullOrWhiteSpace(request.ShipperName) || request.ShipperId.HasValue)
            {
                var party = new ShipmentParty
                {
                    ShipmentId = shipment.Id,
                    PartyType = PartyType.shipper,
                    CompanyName = request.ShipperName ?? string.Empty,
                    ContactName = string.Empty
                };
                await _db.ShipmentParties.AddAsync(party);
                await _db.SaveChangesAsync();
                if (request.ShipperId.HasValue) shipment.ShipperId = request.ShipperId;
            }

            // persist provided document placeholders
            if (request.Documents != null && request.Documents.Count > 0)
            {
                foreach (var doc in request.Documents)
                {
                    var d = new ShipmentDocument
                    {
                        ShipmentId = shipment.Id,
                        DocumentType = DocumentType.Other,
                        FileUrl = null,
                        Version = 1
                    };
                    await _db.ShipmentDocuments.AddAsync(d);
                }
                await _db.SaveChangesAsync();
            }

            return shipment;
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByUserAsync(long userId)
        {
            return await _db.Shipments
                .AsNoTracking()
                .Where(s => s.CreatedBy == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Shipment?> GetShipmentByIdAsync(long id)
        {
            return await _db.Shipments
                .Include(s => s.Items)
                .Include(s => s.Documents)
                .Include(s => s.Parties)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Shipment?> UpdateShipmentAsync(long id, UpdateShipmentRequest request)
        {
            var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == id);
            if (shipment == null) return null;

            var updated = false;
            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                shipment.ShipmentName = request.ProductName;
                updated = true;
            }
            if (request.Quantity.HasValue)
            {
                // update first item as quick mapping
                var item = await _db.ShipmentItems.FirstOrDefaultAsync(i => i.ShipmentId == shipment.Id);
                if (item != null) item.Quantity = request.Quantity.Value;
                updated = true;
            }
            if (request.InvoiceValue.HasValue)
            {
                var item = await _db.ShipmentItems.FirstOrDefaultAsync(i => i.ShipmentId == shipment.Id);
                if (item != null)
                {
                    item.UnitPrice = request.InvoiceValue.Value;
                    item.TotalValue = request.InvoiceValue.Value;
                }
                updated = true;
            }
            if (!string.IsNullOrWhiteSpace(request.HsCode))
            {
                var item = await _db.ShipmentItems.FirstOrDefaultAsync(i => i.ShipmentId == shipment.Id);
                if (item != null) item.HsCode = request.HsCode;
                updated = true;
            }

            if (updated)
            {
                shipment.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return shipment;
        }

        public async Task<bool> ChangeStatusAsync(long id, string newStatus, long? performedBy = null, string? notes = null)
        {
            var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == id);
            if (shipment == null) return false;

            var current = shipment.Status ?? "draft";
            var normalizedNew = newStatus?.ToLowerInvariant() ?? string.Empty;

            // allow same-state no-op
            if (current == normalizedNew) return true;

            // special handling for some commands like submit -> submitted
            // validate allowed transitions
            if (!_allowedTransitions.TryGetValue(current, out var allowed))
            {
                return false;
            }

            if (!allowed.Contains(normalizedNew))
            {
                return false;
            }

            shipment.Status = normalizedNew;
            shipment.UpdatedAt = DateTime.UtcNow;

            // store notes into BrokerNotes if present
            if (!string.IsNullOrWhiteSpace(notes)) shipment.BrokerNotes = notes;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
