using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _svc;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService svc, ILogger<NotificationsController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        public class MarkReadRequest
        {
            public long NotificationId { get; set; }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(long userId)
        {
            try
            {
                var notifications = await _svc.GetAsync(userId);
                return Ok(notifications);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for get notifications");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkRead([FromBody] MarkReadRequest req)
        {
            if (req == null || req.NotificationId <= 0)
                return BadRequest(new { error = "notification_id_required" });

            try
            {
                var result = await _svc.MarkReadAsync(req.NotificationId);
                return Ok(result);
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "Notification not found");
                return NotFound(new { error = "notification_not_found" });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for mark read");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { error = "internal_error" });
            }
        }
    }
}
