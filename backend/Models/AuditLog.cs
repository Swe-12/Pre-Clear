namespace PreClear.Api.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? ShipmentId { get; set; }
        public string Action { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
