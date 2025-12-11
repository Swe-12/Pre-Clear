using PreClear.Api.Interfaces;
using PreClear.Api.Models;
using System;
using System.Threading.Tasks;

namespace PreClear.Api.Services
{
    public class BrokerAssignmentService : IBrokerAssignmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ILogger<BrokerAssignmentService> _logger;

        public BrokerAssignmentService(IShipmentRepository shipmentRepository, ILogger<BrokerAssignmentService> logger)
        {
            _shipmentRepository = shipmentRepository;
            _logger = logger;
        }

        public async Task<Shipment> AssignBrokerAsync(long shipmentId, long brokerId)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
            if (shipment == null)
                throw new InvalidOperationException($"Shipment with ID {shipmentId} not found");

            shipment.AssignedBrokerId = brokerId;
            await _shipmentRepository.UpdateAsync(shipment);

            _logger.LogInformation($"Assigned broker {brokerId} to shipment {shipmentId}");
            return shipment;
        }

        public async Task<List<Shipment>> GetShipmentsByBrokerAsync(long brokerId)
        {
            return await _shipmentRepository.GetByBrokerAsync(brokerId);
        }
    }
}
