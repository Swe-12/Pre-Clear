using PreClear.Api.Models;
using System.Threading.Tasks;

namespace PreClear.Api.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice> GetAsync(long id);
    }
}
