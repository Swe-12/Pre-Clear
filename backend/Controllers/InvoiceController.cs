using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using System;
using System.Threading.Tasks;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost("generate/{shipmentId}")]
        public async Task<IActionResult> GenerateInvoice(long shipmentId)
        {
            if (shipmentId <= 0)
                return BadRequest(new { error = "invalid_shipment_id" });

            try
            {
                var invoice = await _invoiceService.GenerateInvoiceAsync(shipmentId);
                return Ok(new 
                { 
                    invoice_id = invoice.Id, 
                    shipment_id = invoice.ShipmentId, 
                    total_amount = invoice.TotalAmount,
                    pdf_url = invoice.PdfUrl,
                    created_at = invoice.CreatedAt
                });
            }
            catch (InvalidOperationException ioex)
            {
                _logger.LogWarning(ioex, "Shipment not found in generate invoice");
                return NotFound(new { error = "shipment_not_found", detail = ioex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpGet("pdf/{invoiceId}")]
        public async Task<IActionResult> GetInvoicePdf(long invoiceId)
        {
            if (invoiceId <= 0)
                return BadRequest(new { error = "invalid_invoice_id" });

            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(invoiceId);
                return Ok(new 
                { 
                    invoice_id = invoice.Id, 
                    shipment_id = invoice.ShipmentId,
                    total_amount = invoice.TotalAmount,
                    pdf_url = invoice.PdfUrl,
                    created_at = invoice.CreatedAt
                });
            }
            catch (InvalidOperationException ioex)
            {
                _logger.LogWarning(ioex, "Invoice not found");
                return NotFound(new { error = "invoice_not_found", detail = ioex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice");
                return StatusCode(500, new { error = "internal_error" });
            }
        }
    }
}
