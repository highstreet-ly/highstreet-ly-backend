using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class CommitTicketReservationHandler : IConsumer<ICommitTicketReservation>
    {
        private readonly IEventSourcedRepository<TicketsAvailability> _repository;
        private readonly ILogger<CommitTicketReservationHandler> _logger;

        public CommitTicketReservationHandler(
            IEventSourcedRepository<TicketsAvailability> repository,
            ILogger<CommitTicketReservationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ICommitTicketReservation> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                _logger.LogInformation($"running {command.Message.GetType().Name}");
                var availability = _repository.Get(command.Message.EventInstanceId);

                availability.CommitReservation(command.Message.ReservationId);
                await _repository.Save(availability, command.Message.CorrelationId.ToString());
            }
        }
    }
}