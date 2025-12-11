using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly PreclearDbContext _db;
        public TagRepository(PreclearDbContext db) => _db = db;

        public async Task<Tag> CreateAsync(Tag tag)
        {
            tag.CreatedAt = System.DateTime.UtcNow;
            _db.Set<Tag>().Add(tag);
            await _db.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(long tagId)
        {
            var t = await _db.Set<Tag>().FindAsync(tagId);
            if (t == null) return;
            _db.Set<Tag>().Remove(t);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetByShipmentAsync(long shipmentId)
        {
            return await _db.Set<Tag>()
                .AsNoTracking()
                .Where(t => t.ShipmentId == shipmentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
