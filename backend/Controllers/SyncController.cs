using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/sync")]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run()
        {
            var result = await _syncService.RunSyncAsync();
            return Ok(new { imported = result.Imported, updated = result.Updated, details = result.Details });
        }
    }
}

