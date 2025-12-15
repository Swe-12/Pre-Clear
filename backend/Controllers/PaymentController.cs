using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public class CreateIntentRequest
        {
            public long UserId { get; set; }
            public decimal Amount { get; set; }
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreateIntent([FromBody] CreateIntentRequest req)
        {
            if (req == null || req.Amount <= 0) return BadRequest("Invalid request");

            var payment = await _paymentService.CreatePaymentIntentAsync(req.UserId, req.Amount);
            return Ok(new { id = payment.Id, status = payment.Status, amount = payment.Amount });
        }

        public class ConfirmRequest
        {
            public long PaymentId { get; set; }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequest req)
        {
            if (req == null || req.PaymentId <= 0) return BadRequest("Invalid request");

            var p = await _paymentService.ConfirmPaymentAsync(req.PaymentId);
            if (p == null) return NotFound();
            return Ok(new { id = p.Id, status = p.Status });
        }
    }
}
