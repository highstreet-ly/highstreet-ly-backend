using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketsReservationCancelledHandler : IConsumer<ITicketsReservationCancelled>
    {
        private readonly ILogger<TicketsReservationCancelledHandler> _logger;
        private readonly ITicketQuantityService _ticketQuantityService;
        private readonly ManagementDbContext _managementDbContext;

        public TicketsReservationCancelledHandler(
            ILogger<TicketsReservationCancelledHandler> logger,
            ITicketQuantityService ticketQuantityService,
            ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _ticketQuantityService = ticketQuantityService;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<ITicketsReservationCancelled> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation($"Running ConsumeContext<ITicketsReservationCancelled>");

                await _ticketQuantityService.UpdateAvailableQuantity(@event.Message,
                    @event.Message.AvailableTicketsChanged);
            }
        }
    }
}