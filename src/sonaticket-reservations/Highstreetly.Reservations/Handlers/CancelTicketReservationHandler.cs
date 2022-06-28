using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class CancelTicketReservationHandler : IConsumer<ICancelTicketReservation>
    {
        private readonly IEventSourcedRepository<TicketsAvailability> _repository;
        private readonly ILogger<CancelTicketReservationHandler> _logger;

        public CancelTicketReservationHandler(
            IEventSourcedRepository<TicketsAvailability> repository,
            ILogger<CancelTicketReservationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ICancelTicketReservation> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                _logger.LogInformation($"running {command.Message.GetType().Name}");
                var availability = _repository.Get(command.Message.EventInstanceId);
                availability.Id = command.Message.EventInstanceId;
                availability.CancelReservation(command.Message.ReservationId);
                await _repository.Save(availability, command.Message.CorrelationId.ToString());
            }
        }
    }
}