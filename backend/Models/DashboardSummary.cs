namespace PreClear.Api.Models
{
    public class DashboardSummary
    {
        public long TotalShipments { get; set; }
        public long ApprovedShipments { get; set; }
        public long PendingShipments { get; set; }
        public long ExceptionsCount { get; set; }
    }
}
