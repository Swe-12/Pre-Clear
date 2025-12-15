using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly PreclearDbContext _db;

        public ChatMessageRepository(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ShipmentMessage message)
        {
            message.CreatedAt = DateTime.UtcNow;
            await _db.ShipmentMessages.AddAsync(message);
            await SaveAsync();
        }

        public async Task<List<ShipmentMessage>> GetByShipmentIdAsync(long shipmentId)
        {
            return await _db.ShipmentMessages
                .Where(m => m.ShipmentId == shipmentId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
