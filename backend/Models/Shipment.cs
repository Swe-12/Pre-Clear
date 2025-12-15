namespace PreClear.Api.Models
{
    public enum ShipmentMode { Air, Sea, Ground }
    public enum ShipmentType { Domestic, International }
    public enum ShipmentStatus { draft, pending_validation, requires_resolution, pre_cleared, token_generated, booked, in_transit, delivered, cancelled }

    public class Shipment
    {
        public long Id { get; set; }
        public string ReferenceId { get; set; } = null!;
        public string? ShipmentName { get; set; }
        public ShipmentMode Mode { get; set; } = ShipmentMode.Ground;
        public ShipmentType ShipmentType { get; set; } = ShipmentType.International;
        public string Carrier { get; set; } = "UPS";
        public string Status { get; set; } = "draft";
        public string? PreclearToken { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Optional summary fields
        public string? BrokerNotes { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? TotalWeight { get; set; }
        public string? Currency { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public long? ShipperId { get; set; }
        public long? ConsigneeId { get; set; }
        public DateTime? TokenGeneratedAt { get; set; }
        // Broker assignment
        public long? AssignedBrokerId { get; set; }

        // Navigation properties
        public virtual ICollection<ShipmentItem>? Items { get; set; }
        public virtual ICollection<ShipmentDocument>? Documents { get; set; }
        public virtual ICollection<ShipmentParty>? Parties { get; set; }
    }
}
