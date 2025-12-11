namespace PreClear.Api.Models
{
    public enum Payer { Shipper, Consignee, ThirdParty }
    public enum PaymentStatus { pending, paid, failed, refunded }

    public class Payment
    {
        public long Id { get; set; }
        public long? ShipmentId { get; set; }
        public Payer? Payer { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? PaymentMethod { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
