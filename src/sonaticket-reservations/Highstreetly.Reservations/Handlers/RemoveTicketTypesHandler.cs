using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class RemoveTicketTypesHandler : IConsumer<IRemoveTicketTypes>
    {
        private readonly IEventSourcedRepository<TicketsAvailability> _repository;
        private readonly ILogger<RemoveTicketTypesHandler> _logger;

        public RemoveTicketTypesHandler(
            IEventSourcedRepository<TicketsAvailability> repository,
            ILoggerFactory logFactory)
        {
            _repository = repository;
            _logger = logFactory.CreateLogger<RemoveTicketTypesHandler>();
        }

        public async Task Consume(
            ConsumeContext<IRemoveTicketTypes> command)
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

                _logger.LogInformation(
                    $"{command.Message.GetType().Name} found availability which says it's ID is {availability.Id}");

                availability.RemoveTickets(command.Message.TicketType, command.Message.Quantity);
                await _repository.Save(availability, command.Message.CorrelationId.ToString());
            }
        }
    }
}
