using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketsReservedHandler : IConsumer<ITicketsReserved>
    {
        private readonly ILogger<TicketsReservedHandler> _logger;
        private readonly ITicketQuantityService _ticketQuantityService;
        private readonly ManagementDbContext _managementDbContext;

        public TicketsReservedHandler(
            ILogger<TicketsReservedHandler> logger,
            ITicketQuantityService ticketQuantityService,
            ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _ticketQuantityService = ticketQuantityService;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<ITicketsReserved> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation($"Running ConsumeContext<ITicketsReserved>");

                // if the store is stock managed we get AvailableTicketsChanged 
                // otherwise it's null - there's no need to set the change
                if (@event.Message.AvailableTicketsChanged != null)
                {
                    await _ticketQuantityService.UpdateAvailableQuantity(
                        @event.Message,
                        @event.Message.AvailableTicketsChanged);
                }
            }
        }
    }
}