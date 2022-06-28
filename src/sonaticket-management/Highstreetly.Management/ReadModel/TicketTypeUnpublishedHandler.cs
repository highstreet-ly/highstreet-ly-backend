using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketTypeUnpublishedHandler : IConsumer<ITicketTypeUnpublished>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly IBusClient _ticketReservationClient;
        private readonly ILogger<TicketTypeUnpublishedHandler> _logger;

        public TicketTypeUnpublishedHandler(IBusClient ticketReservationClient, ILogger<TicketTypeUnpublishedHandler> logger, ManagementDbContext managementDbContext)
        {
            _ticketReservationClient = ticketReservationClient;
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<ITicketTypeUnpublished> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation($"Running ConsumeContext<ITicketTypeUnpublished>");

                try
                {
                    //TODO: shouldn't we just delete the tickettype?

                    _logger.LogInformation($"Un-publishing ticket type {@event.Message.SourceId}");

                    var product = _managementDbContext.TicketTypes.FirstOrDefault(x => x.Id == @event.Message.SourceId);
                    product.IsPublished = false;
                    await _managementDbContext.SaveChangesAsync();

                    // await _ticketReservationClient.Send<IRemoveTicketTypes>(
                    //     new RemoveTicketTypes
                    //     {
                    //         EventInstanceId = @event.Message.EventInstanceId,
                    //         TicketType = @event.Message.SourceId,
                    //         Quantity = product.Quantity ?? 0,
                    //         CorrelationId = @event.Message.CorrelationId
                    //     });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Couldn't run ITicketTypeUnpublished for {@event.Message.Name}", ex);
                    throw;
                }
            }
        }
    }
}