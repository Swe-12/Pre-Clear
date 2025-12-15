using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByIdAsync(long id);
        Task<Payment> UpdateAsync(Payment payment);
        Task<List<Payment>> GetByUserAsync(long userId);
    }
}
