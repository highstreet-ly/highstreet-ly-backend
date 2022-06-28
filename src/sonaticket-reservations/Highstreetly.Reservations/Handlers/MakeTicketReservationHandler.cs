using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class MakeTicketReservationHandler : IConsumer<IMakeTicketReservation>
    {
        private readonly IEventSourcedRepository<TicketsAvailability> _repository;
        private readonly ILogger<MakeTicketReservationHandler> _logger;
        private ReservationDbContext _reservationDbContext;

        public MakeTicketReservationHandler(
            IEventSourcedRepository<TicketsAvailability> repository,
            ILogger<MakeTicketReservationHandler> logger,
            ReservationDbContext reservationDbContext)
        {
            _repository = repository;
            _logger = logger;
            _reservationDbContext = reservationDbContext;
        }

        public async Task Consume(
            ConsumeContext<IMakeTicketReservation> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                _logger.LogInformation($"running {command.Message.GetType().Name}");
                var availability = _repository.Get(command.Message.EventInstanceId);

                _logger.LogInformation($"Availability version: {availability.Version}");

                availability.MakeReservation(command.Message.ReservationId, command.Message.Tickets, command.Message.IsStockManaged);
                await _repository.Save(availability, command.Message.CorrelationId.ToString());
            }
        }
    }
}