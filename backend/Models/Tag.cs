using System;

namespace PreClear.Api.Models
{
    public class Tag
    {
        public long Id { get; set; }
        public long ShipmentId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
