using System;
using System.ComponentModel.DataAnnotations;

namespace PreClear.Api.Models
{
    public class CreateShipmentRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal InvoiceValue { get; set; }

        public string? HsCode { get; set; }

        // extended
        public List<CreateShipmentItemDto>? Items { get; set; }
        public string? Currency { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? TotalWeight { get; set; }
        public long? ShipperId { get; set; }
        public string? ShipperName { get; set; }
        public List<DocumentDto>? Documents { get; set; }
    }

    public class CreateShipmentItemDto
    {
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public string? HsCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class DocumentDto
    {
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
    }

    

    public class UpdateShipmentRequest
    {
        public string? ProductName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? InvoiceValue { get; set; }
        public string? HsCode { get; set; }
    }

    public class ShipmentResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal InvoiceValue { get; set; }
        public string? HsCode { get; set; }
        public string Status { get; set; } = "draft";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? TotalWeight { get; set; }
        public string? Currency { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public long? ShipperId { get; set; }
        public string? ShipperName { get; set; }
    }

    public class ChangeStatusRequest
    {
        [Required]
        public string NewStatus { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
