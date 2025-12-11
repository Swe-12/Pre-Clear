using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentIntentAsync(long userId, decimal amount);
        Task<Payment?> ConfirmPaymentAsync(long paymentId);
    }
}
