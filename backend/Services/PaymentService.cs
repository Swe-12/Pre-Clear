using System;
using System.Threading.Tasks;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;

        public PaymentService(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<Payment> CreatePaymentIntentAsync(long userId, decimal amount)
        {
            var p = new Payment
            {
                UserId = userId,
                Amount = amount,
                Status = "requires_confirmation",
                CreatedAt = DateTime.UtcNow
            };

            return await _repo.CreateAsync(p);
        }

        public async Task<Payment?> ConfirmPaymentAsync(long paymentId)
        {
            var existing = await _repo.GetByIdAsync(paymentId);
            if (existing == null) return null;

            existing.Status = "succeeded";
            existing = await _repo.UpdateAsync(existing);
            return existing;
        }
    }
}
