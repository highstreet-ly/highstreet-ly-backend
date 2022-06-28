using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class AssignRegistrantDetailsHandler : IConsumer<IAssignRegistrantDetails>
    {
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly ILogger<AssignRegistrantDetailsHandler> _logger;

        public AssignRegistrantDetailsHandler(
            IEventSourcedRepository<Order> repository,
            ILogger<AssignRegistrantDetailsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IAssignRegistrantDetails> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                var assignRegistrantDetails = command.Message;

                var order = _repository.Find(assignRegistrantDetails.OrderId);
                order.AssignRegistrant(assignRegistrantDetails.OwnerName, assignRegistrantDetails.Email,
                    assignRegistrantDetails.UserId, assignRegistrantDetails.Phone,
                    assignRegistrantDetails.DeliveryLine1, assignRegistrantDetails.DeliveryPostcode);
                await _repository.Save(order, assignRegistrantDetails.CorrelationId.ToString());
            }
        }
    }
}