using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _ai;
        private readonly ILogger<AiController> _logger;

        public AiController(IAiService ai, ILogger<AiController> logger)
        {
            _ai = ai;
            _logger = logger;
        }

        public class AnalyzeRequest
        {
            public string Description { get; set; } = string.Empty;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] AnalyzeRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Description))
                return BadRequest(new { error = "description_required" });

            try
            {
                var result = await _ai.AnalyzeAsync(req.Description);
                return Ok(result);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Bad request in AI analyze");
                return BadRequest(new { error = "invalid_input", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during AI analysis");
                return StatusCode(500, new { error = "internal_error", detail = "An unexpected error occurred while analyzing the description." });
            }
        }

        [HttpPost("extract-text")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ExtractText([FromForm] ExtractTextRequest request)
        {
            if (request?.File == null)
                return BadRequest(new { error = "file_required" });

            try
            {
                var result = await _ai.ExtractTextAsync(request.File);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during extract-text");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpPost("validate-invoice")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ValidateInvoice([FromForm] ValidateInvoiceRequest request)
        {
            if (request?.File == null)
                return BadRequest(new { error = "file_required" });

            try
            {
                var result = await _ai.ValidateInvoiceAsync(request.File);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during validate-invoice");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpPost("validate-packing-list")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ValidatePackingList([FromForm] ValidatePackingListRequest request)
        {
            if (request?.File == null)
                return BadRequest(new { error = "file_required" });

            try
            {
                var result = await _ai.ValidatePackingListAsync(request.File);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during validate-packing-list");
                return StatusCode(500, new { error = "internal_error" });
            }
        }
    }
}

