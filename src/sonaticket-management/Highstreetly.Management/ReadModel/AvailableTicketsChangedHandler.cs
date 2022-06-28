using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class AvailableTicketsChangedHandler : IConsumer<IAvailableTicketsChanged>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly ILogger<AvailableTicketsChangedHandler> _logger;
        private readonly ITicketQuantityService _ticketQuantityService;

        public AvailableTicketsChangedHandler(
            ILogger<AvailableTicketsChangedHandler> logger,
            ITicketQuantityService ticketQuantityService, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _ticketQuantityService = ticketQuantityService;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<IAvailableTicketsChanged> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation($"Running ConsumeContext<IAvailableTicketsChanged>");
                
                await _ticketQuantityService.UpdateAvailableQuantity(@event.Message, @event.Message.Tickets);
            }
        }
    }
}