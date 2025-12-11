using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditLogService _auditService;

        public AuditController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("{shipmentId}")]
        public async Task<IActionResult> GetByShipment(long shipmentId)
        {
            var logs = await _auditService.GetShipmentAuditAsync(shipmentId);
            return Ok(logs);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(long userId)
        {
            var logs = await _auditService.GetUserAuditAsync(userId);
            return Ok(logs);
        }
    }
}
