using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _service;

        public ShipmentsController(IShipmentService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var shipment = await _service.CreateShipmentAsync(request);
            return CreatedAtAction(nameof(Details), new { id = shipment.Id }, new ShipmentResponse
            {
                Id = shipment.Id,
                UserId = shipment.CreatedBy ?? 0,
                ProductName = shipment.ShipmentName ?? string.Empty,
                Quantity = shipment.Items?.FirstOrDefault()?.Quantity ?? 0,
                InvoiceValue = shipment.Items?.FirstOrDefault()?.TotalValue ?? 0,
                HsCode = shipment.Items?.FirstOrDefault()?.HsCode,
                Status = shipment.Status ?? "draft",
                CreatedAt = shipment.CreatedAt,
                UpdatedAt = shipment.UpdatedAt,
                TotalValue = shipment.TotalValue,
                TotalWeight = shipment.TotalWeight,
                Currency = shipment.Currency,
                TrackingNumber = shipment.TrackingNumber,
                EstimatedDelivery = shipment.EstimatedDelivery,
                ShipperId = shipment.ShipperId,
                ShipperName = shipment.Parties?.FirstOrDefault(p => p.PartyType == PartyType.shipper)?.CompanyName
            });
        }

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetByUser([FromRoute] long userId)
        {
            var list = await _service.GetShipmentsByUserAsync(userId);
            var result = list.Select(s => new ShipmentResponse
            {
                Id = s.Id,
                UserId = s.CreatedBy ?? 0,
                ProductName = s.ShipmentName ?? string.Empty,
                Quantity = s.Items?.FirstOrDefault()?.Quantity ?? 0,
                InvoiceValue = s.Items?.FirstOrDefault()?.TotalValue ?? 0,
                HsCode = s.Items?.FirstOrDefault()?.HsCode,
                Status = s.Status ?? "draft",
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
            return Ok(result);
        }

        [HttpGet("details/{id:long}")]
        public async Task<IActionResult> Details([FromRoute] long id)
        {
            var s = await _service.GetShipmentByIdAsync(id);
            if (s == null) return NotFound();

            var resp = new ShipmentResponse
            {
                Id = s.Id,
                UserId = s.CreatedBy ?? 0,
                ProductName = s.ShipmentName ?? string.Empty,
                Quantity = s.Items?.FirstOrDefault()?.Quantity ?? 0,
                InvoiceValue = s.Items?.FirstOrDefault()?.TotalValue ?? 0,
                HsCode = s.Items?.FirstOrDefault()?.HsCode,
                Status = s.Status ?? "draft",
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };

            return Ok(resp);
        }

        [HttpPut("update/{id:long}")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateShipmentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var s = await _service.UpdateShipmentAsync(id, request);
            if (s == null) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:long}/submit")]
        public async Task<IActionResult> Submit([FromRoute] long id, [FromBody] ChangeStatusRequest req)
        {
            var ok = await _service.ChangeStatusAsync(id, "submitted", null, req?.Notes);
            if (!ok) return BadRequest(new { error = "invalid_transition" });
            return Ok();
        }

        [HttpPost("{id:long}/approve")]
        public async Task<IActionResult> Approve([FromRoute] long id, [FromBody] ChangeStatusRequest req)
        {
            var ok = await _service.ChangeStatusAsync(id, "approved", null, req?.Notes);
            if (!ok) return BadRequest(new { error = "invalid_transition" });
            return Ok();
        }

        [HttpPost("{id:long}/reject")]
        public async Task<IActionResult> Reject([FromRoute] long id, [FromBody] ChangeStatusRequest req)
        {
            var ok = await _service.ChangeStatusAsync(id, "rejected", null, req?.Notes);
            if (!ok) return BadRequest(new { error = "invalid_transition" });
            return Ok();
        }

        [HttpPost("{id:long}/hold")]
        public async Task<IActionResult> Hold([FromRoute] long id, [FromBody] ChangeStatusRequest req)
        {
            var ok = await _service.ChangeStatusAsync(id, "on_hold", null, req?.Notes);
            if (!ok) return BadRequest(new { error = "invalid_transition" });
            return Ok();
        }

        [HttpPost("{id:long}/reopen")]
        public async Task<IActionResult> Reopen([FromRoute] long id, [FromBody] ChangeStatusRequest req)
        {
            var ok = await _service.ChangeStatusAsync(id, "reopen", null, req?.Notes);
            if (!ok) return BadRequest(new { error = "invalid_transition" });
            return Ok();
        }
    }
}
