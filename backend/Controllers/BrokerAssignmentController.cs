using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    public class BrokerAssignmentController : ControllerBase
    {
        private readonly IBrokerAssignmentService _brokerService;

        public BrokerAssignmentController(IBrokerAssignmentService brokerService)
        {
            _brokerService = brokerService;
        }

        [HttpPost("/api/shipments/{id}/assign-broker")]
        public async Task<IActionResult> AssignBroker([FromRoute] long id, [FromBody] AssignBrokerRequest req)
        {
            try
            {
                var updated = await _brokerService.AssignBrokerAsync(id, req.BrokerId);
                return Ok(new { shipment_id = updated.Id, assigned_broker_id = updated.AssignedBrokerId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("/api/brokers/{brokerId}/shipments")]
        public async Task<IActionResult> GetShipmentsByBroker([FromRoute] long brokerId)
        {
            var list = await _brokerService.GetShipmentsByBrokerAsync(brokerId);
            return Ok(list);
        }
    }

    public class AssignBrokerRequest
    {
        public long BrokerId { get; set; }
    }
}
