namespace PreClear.Api.Models
{
    public class Invoice
    {
        public long Id { get; set; }
        public long ShipmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PdfUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
