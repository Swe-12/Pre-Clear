using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PreclearDbContext _db;
        public PaymentRepository(PreclearDbContext db) => _db = db;

        public async Task<Payment> CreateAsync(Payment payment)
        {
            payment.CreatedAt = System.DateTime.UtcNow;
            _db.Set<Payment>().Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByIdAsync(long id)
        {
            return await _db.Set<Payment>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _db.Set<Payment>().Update(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> GetByUserAsync(long userId)
        {
            return await _db.Set<Payment>().AsNoTracking().Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
    }
}
