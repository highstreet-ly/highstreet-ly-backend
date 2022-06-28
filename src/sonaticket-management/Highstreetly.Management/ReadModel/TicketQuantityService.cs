using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Management.Resources;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class TicketQuantityService : ITicketQuantityService
    {
        private readonly ILogger<TicketQuantityService> _logger;
        readonly ManagementDbContext  _ctx;

        public TicketQuantityService(ILogger<TicketQuantityService> logger, ManagementDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task UpdateAvailableQuantity(ISonaticketEvent @event, IEnumerable<TicketQuantity> seats)
        {
            _logger.LogInformation($"Running UpdateAvailableQuantity");

            try
            {
                var ticketTypes = _ctx
                    .TicketTypes
                    .Where(x => x.EventInstanceId == @event.SourceId)
                    .ToList<ITicketType>();

                // This check assumes events might be received more than once, but not out of order
                // it bombs out here if the version is not in sync - check this makes sense!
                var maxSeatsAvailabilityVersion = ticketTypes.Max(x => x.TicketsAvailabilityVersion);
                if (maxSeatsAvailabilityVersion >= @event.Version)
                {
                    _logger.LogInformation(
                        "Ignoring availability update message with version {1} for seat types with conference id {0}, last known version {2}.",
                        @event.SourceId,
                        @event.Version,
                        maxSeatsAvailabilityVersion);
                    return;
                }

                if (ticketTypes.Count > 0)
                {
                    _logger.LogInformation($"Calling into ProcessUpdateAvailableQuantity for TicketType");
                    await ProcessUpdateAvailableQuantity(@event, seats, ticketTypes);
                }
                else
                {
                    _logger.LogError(
                        $"Failed to locate Seat Types read model for updated seat availability, with conference id {@event.SourceId}.");
                }

                var ticketTypeConfigurations = _ctx
                    .TicketTypeConfigurations
                    .Where(x => x.EventInstanceId == @event.SourceId)
                    .ToList<ITicketType>();

                if (ticketTypeConfigurations.Count > 0)
                {
                    _logger.LogInformation($"Calling into ProcessUpdateAvailableQuantity for TicketTypeConfiguration");
                    await ProcessUpdateAvailableQuantity(@event, seats, ticketTypeConfigurations);
                }
                else
                {
                    _logger.LogError(
                        $"Failed to locate Seat Types read model for updated seat availability, with conference id {@event.SourceId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Couldn't run UpdateAvailableQuantity for {@event.SourceId}");
                throw;
            }
        }

        private async Task ProcessUpdateAvailableQuantity(
            ISonaticketEvent @event,
            IEnumerable<TicketQuantity> seats,
            List<ITicketType> ticketTypes)
        {
            _logger.LogInformation($"Running ProcessUpdateAvailableQuantity");

            foreach (var seat in seats)
            {
                var ticketType = ticketTypes.FirstOrDefault(x => x.Id == seat.TicketType);

                if (ticketType != null)
                {
                    _logger.LogInformation($"Running ProcessUpdateAvailableQuantity processing seat {ticketType.Name} - setting AvailableQuantity: {seat.Quantity}");

                    ticketType.AvailableQuantity ??= 0;

                    ticketType.AvailableQuantity += seat.Quantity;
                    ticketType.TicketsAvailabilityVersion = @event.Version;
                }
                else
                {
                    // TODO should reject the entire update?
                    _logger.LogError(
                        "Failed to locate Seat Type read model being updated with id {0}.", seat.TicketType);
                }
            }

            await _ctx.SaveChangesAsync();
        }
    }
}