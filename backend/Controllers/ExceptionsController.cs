using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/exceptions")]
    public class ExceptionsController : ControllerBase
    {
        private readonly IExceptionService _svc;
        private readonly ILogger<ExceptionsController> _logger;

        public ExceptionsController(IExceptionService svc, ILogger<ExceptionsController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        public class CreateExceptionRequest
        {
            public long ShipmentId { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Severity { get; set; } = "warning";
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateExceptionRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Code) || string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { error = "code_and_message_required" });

            try
            {
                var result = await _svc.CreateAsync(req.ShipmentId, req.Code, req.Message, req.Severity);
                return Ok(result);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for create exception");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exception");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpPut("resolve/{id}")]
        public async Task<IActionResult> Resolve(long id)
        {
            try
            {
                var result = await _svc.ResolveAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "Exception not found");
                return NotFound(new { error = "exception_not_found" });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for resolve exception");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving exception");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpGet("open/{shipmentId}")]
        public async Task<IActionResult> GetOpen(long shipmentId)
        {
            try
            {
                var result = await _svc.GetOpenByShipmentAsync(shipmentId);
                return Ok(result);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for get open exceptions");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting open exceptions");
                return StatusCode(500, new { error = "internal_error" });
            }
        }
    }
}
