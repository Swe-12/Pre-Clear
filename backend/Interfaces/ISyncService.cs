using System.Threading.Tasks;

namespace PreClear.Api.Interfaces
{
    public interface ISyncService
    {
        /// <summary>
        /// Run a synchronization: simulate pulling external data and upsert shipments.
        /// Returns a result string or summary.
        /// </summary>
        Task<SyncResult> RunSyncAsync();
    }

    public class SyncResult
    {
        public int Imported { get; set; }
        public int Updated { get; set; }
        public string? Details { get; set; }
    }
}

