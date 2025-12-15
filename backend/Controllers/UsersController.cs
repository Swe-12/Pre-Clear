using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _svc;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService svc, ILogger<UsersController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        public class ChangeRoleRequest
        {
            public string Role { get; set; } = string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int? skip, [FromQuery] int? take)
        {
            try
            {
                var users = await _svc.ListAsync(skip, take);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var user = await _svc.GetAsync(id);
                return Ok(user);
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "User not found");
                return NotFound(new { error = "user_not_found" });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(long id, [FromBody] ChangeRoleRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Role))
                return BadRequest(new { error = "role_required" });

            try
            {
                var user = await _svc.ChangeRoleAsync(id, req.Role);
                return Ok(user);
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "User not found");
                return NotFound(new { error = "user_not_found" });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return Ok(new { message = "user_deleted" });
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "User not found");
                return NotFound(new { error = "user_not_found" });
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, new { error = "internal_error" });
            }
        }
    }
}
