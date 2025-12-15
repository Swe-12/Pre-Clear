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
    public class UserRepository : IUserRepository
    {
        private readonly PreclearDbContext _db;

        public UserRepository(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> ListAsync(int? skip = null, int? take = null)
        {
            var query = _db.Users.AsQueryable();

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        public async Task<User> FindAsync(long userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await SaveAsync();
        }

        public async Task DeleteAsync(long userId)
        {
            var user = await FindAsync(userId);
            if (user != null)
            {
                _db.Users.Remove(user);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
