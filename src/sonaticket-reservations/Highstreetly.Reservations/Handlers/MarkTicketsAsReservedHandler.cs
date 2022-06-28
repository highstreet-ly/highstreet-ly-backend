using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class MarkTicketsAsReservedHandler : IConsumer<IMarkTicketsAsReserved>
    {
        private readonly ILogger<MarkTicketsAsReservedHandler> _logger;
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly IPricingService _pricingService;

        public MarkTicketsAsReservedHandler(
            IPricingService pricingService,
            IEventSourcedRepository<Order> repository,
            ILogger<MarkTicketsAsReservedHandler> logger)
        {
            _pricingService = pricingService;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IMarkTicketsAsReserved> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                try
                {
                    var order = _repository.Get(command.Message.OrderId);

                    if (command.Message.Tickets.Any())
                    {
                        // var totals =
                        //     await _pricingService.CalculateTotal(order.ConferenceId, order.Id, command.Message.Tickets);

                        order.MarkAsReserved(null, command.Message.Expiration, command.Message.Tickets, order.ConferenceId);
                        await _repository.Save(order, command.Message.CorrelationId.ToString());
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Couldn't run IMarkTicketsAsReserved  {command.Message.OrderId}", ex);
                    throw;
                }
            }
        }
    }
}