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
    public class ExceptionRepository : IExceptionRepository
    {
        private readonly PreclearDbContext _db;

        public ExceptionRepository(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ShipmentException exception)
        {
            await _db.ShipmentExceptions.AddAsync(exception);
            await SaveAsync();
        }

        public async Task UpdateAsync(ShipmentException exception)
        {
            _db.ShipmentExceptions.Update(exception);
            await SaveAsync();
        }

        public async Task<ShipmentException> FindAsync(long exceptionId)
        {
            return await _db.ShipmentExceptions.FirstOrDefaultAsync(e => e.Id == exceptionId);
        }

        public async Task<List<ShipmentException>> GetByShipmentIdAsync(long shipmentId)
        {
            return await _db.ShipmentExceptions
                .Where(e => e.ShipmentId == shipmentId)
                .ToListAsync();
        }

        public async Task<List<ShipmentException>> GetOpenByShipmentAsync(long shipmentId)
        {
            return await _db.ShipmentExceptions
                .Where(e => e.ShipmentId == shipmentId && !e.Resolved)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
