using PreClear.Api.Interfaces;
using PreClear.Api.Models;
using System;
using System.Threading.Tasks;

namespace PreClear.Api.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IShipmentRepository _shipmentRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IShipmentRepository shipmentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<Invoice> GenerateInvoiceAsync(long shipmentId)
        {
            // Verify shipment exists
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
            if (shipment == null)
                throw new InvalidOperationException($"Shipment with ID {shipmentId} not found");

            // Generate invoice
            var invoice = new Invoice
            {
                ShipmentId = shipmentId,
                TotalAmount = shipment.TotalValue ?? 0, // Use shipment value as invoice amount
                PdfUrl = $"/invoices/invoice_{shipmentId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf",
                CreatedAt = DateTime.UtcNow
            };

            return await _invoiceRepository.CreateAsync(invoice);
        }

        public async Task<Invoice> GetInvoiceAsync(long invoiceId)
        {
            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            if (invoice == null)
                throw new InvalidOperationException($"Invoice with ID {invoiceId} not found");

            return invoice;
        }
    }
}
