using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PreClear.Api.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly PreclearDbContext _db;

        public InvoiceRepository(PreclearDbContext db)
        {
            _db = db;
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> GetAsync(long id)
        {
            return await _db.Invoices.FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
