using PreClear.Api.Models;
using System.Threading.Tasks;

namespace PreClear.Api.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> GenerateInvoiceAsync(long shipmentId);
        Task<Invoice> GetInvoiceAsync(long invoiceId);
    }
}
