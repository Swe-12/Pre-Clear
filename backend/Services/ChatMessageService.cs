using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _repo;
        private readonly ILogger<ChatMessageService> _logger;

        private static readonly string[] ValidSenders = { "user", "bot" };

        public ChatMessageService(IChatMessageRepository repo, ILogger<ChatMessageService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ShipmentMessage> SendAsync(long shipmentId, string message, string sender = "user", long? senderId = null)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("ShipmentId must be positive", nameof(shipmentId));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required", nameof(message));

            if (Array.IndexOf(ValidSenders, sender?.ToLowerInvariant() ?? "user") < 0)
                throw new ArgumentException($"Invalid sender. Allowed: {string.Join(", ", ValidSenders)}", nameof(sender));

            var chatMessage = new ShipmentMessage
            {
                ShipmentId = shipmentId,
                SenderId = senderId,
                Sender = (sender ?? "user").ToLowerInvariant(),
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(chatMessage);
            _logger.LogInformation($"Chat message sent for shipment {shipmentId} by {sender}");

            return chatMessage;
        }

        public async Task<List<ShipmentMessage>> GetHistoryAsync(long shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("ShipmentId must be positive", nameof(shipmentId));

            return await _repo.GetByShipmentIdAsync(shipmentId);
        }
    }
}
