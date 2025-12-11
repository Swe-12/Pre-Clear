using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly IExceptionRepository _repo;
        private readonly ILogger<ExceptionService> _logger;

        public ExceptionService(IExceptionRepository repo, ILogger<ExceptionService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ShipmentException> CreateAsync(long shipmentId, string code, string message, string severity = "warning", long? createdBy = null)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("ShipmentId must be positive", nameof(shipmentId));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required", nameof(code));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required", nameof(message));

            var exception = new ShipmentException
            {
                ShipmentId = shipmentId,
                Code = code,
                Message = message,
                Severity = severity,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                Resolved = false
            };

            await _repo.AddAsync(exception);
            _logger.LogInformation($"Exception created: {code} for shipment {shipmentId}");

            return exception;
        }

        public async Task<List<ShipmentException>> GetOpenByShipmentAsync(long shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("ShipmentId must be positive", nameof(shipmentId));

            return await _repo.GetOpenByShipmentAsync(shipmentId);
        }

        public async Task<ShipmentException> ResolveAsync(long exceptionId, long? resolvedBy = null)
        {
            if (exceptionId <= 0)
                throw new ArgumentException("ExceptionId must be positive", nameof(exceptionId));

            var exception = await _repo.FindAsync(exceptionId);
            if (exception == null)
                throw new InvalidOperationException($"Exception {exceptionId} not found");

            exception.Resolved = true;
            exception.ResolvedBy = resolvedBy;
            exception.ResolvedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(exception);
            _logger.LogInformation($"Exception {exceptionId} resolved");

            return exception;
        }
    }
}
