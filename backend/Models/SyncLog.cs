using System;

namespace PreClear.Api.Models
{
    public class SyncLog
    {
        public long Id { get; set; }
        public DateTime RunAt { get; set; }
        public int ImportedCount { get; set; }
        public int UpdatedCount { get; set; }
        public string? Details { get; set; }
    }
}
