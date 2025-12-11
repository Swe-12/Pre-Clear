using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMsgService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, IChatMessageService chatMsgService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _chatMsgService = chatMsgService;
            _logger = logger;
        }

        [HttpGet("shipments/{shipmentId}/messages")]
        public async Task<IActionResult> GetMessages(long shipmentId)
        {
            var messages = await _chatService.GetMessagesForShipmentAsync(shipmentId);
            var dto = messages.Select(m => new ShipmentMessageDto
            {
                Id = m.Id,
                ShipmentId = m.ShipmentId,
                SenderId = m.SenderId,
                Message = m.Message,
                CreatedAt = m.CreatedAt
            });
            return Ok(dto);
        }

        [HttpPost("shipments/{shipmentId}/messages")]
        public async Task<IActionResult> SendMessage(long shipmentId, [FromBody] SendMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "message is required" });
            }

            var msg = await _chatService.SendMessageAsync(shipmentId, request.SenderId, request.Message);

            var dto = new ShipmentMessageDto
            {
                Id = msg.Id,
                ShipmentId = msg.ShipmentId,
                SenderId = msg.SenderId,
                Message = msg.Message,
                CreatedAt = msg.CreatedAt
            };

            _logger.LogInformation("User {Sender} sent message {MessageId} on shipment {Shipment}", request.SenderId, msg.Id, shipmentId);
            return CreatedAtAction(nameof(GetMessages), new { shipmentId = shipmentId }, dto);
        }

        // New shipment-specific chat endpoints with sender role support
        [HttpPost("send/{shipmentId}")]
        public async Task<IActionResult> Send(long shipmentId, [FromBody] SendMessageRequestV2 req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { error = "message_required" });

            try
            {
                var msg = await _chatMsgService.SendAsync(shipmentId, req.Message, req.Sender ?? "user", req.SenderId);
                return Ok(msg);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for send message");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat message");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        [HttpGet("{shipmentId}")]
        public async Task<IActionResult> GetHistory(long shipmentId)
        {
            try
            {
                var history = await _chatMsgService.GetHistoryAsync(shipmentId);
                return Ok(history);
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Invalid argument for get history");
                return BadRequest(new { error = "invalid_argument", detail = aex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chat history");
                return StatusCode(500, new { error = "internal_error" });
            }
        }

        public class SendMessageRequest
        {
            public long? SenderId { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public class SendMessageRequestV2
        {
            public string Message { get; set; } = string.Empty;
            public string Sender { get; set; } = "user"; // "user" or "bot"
            public long? SenderId { get; set; }
        }

        public class ShipmentMessageDto
        {
            public long Id { get; set; }
            public long ShipmentId { get; set; }
            public long? SenderId { get; set; }
            public string Message { get; set; } = string.Empty;
            public System.DateTime CreatedAt { get; set; }
        }
    }
}
