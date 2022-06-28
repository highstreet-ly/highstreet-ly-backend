using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class AddTicketTypesHandler : IConsumer<IAddTicketTypes>
    {
        private readonly IEventSourcedRepository<TicketsAvailability> _repository;
        private readonly ILogger<AddTicketTypesHandler> _logger;

        public AddTicketTypesHandler(
            IEventSourcedRepository<TicketsAvailability> repository,
            ILogger<AddTicketTypesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IAddTicketTypes> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                _logger.LogInformation($"running {command.Message.GetType().Name} " +
                                       $"for ticket {command.Message.TicketType} " +
                                       $"with quantity {command.Message.Quantity}");

                var availability = _repository.Find(command.Message.EventInstanceId);
                if (availability == null || availability.Version == -1)
                {
                    availability =
                        new TicketsAvailability(command.Message.EventInstanceId, command.Message.CorrelationId);
                }

                availability.Id = command.Message.EventInstanceId;

                availability.AddTickets(command.Message.TicketType, command.Message.Quantity,
                    command.Message.TicketDetails);

                await _repository.Save(availability, Guid.NewGuid()
                    .ToString());
            }
        }
    }
}