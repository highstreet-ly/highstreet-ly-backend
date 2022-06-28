using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class OrderRegistrantAssignedHandler : IConsumer<IOrderRegistrantAssigned>
    {
        private readonly ILogger<OrderRegistrantAssignedHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public OrderRegistrantAssignedHandler(
            ILogger<OrderRegistrantAssignedHandler> logger,
            ManagementDbContext managementDbContext)

        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(
            ConsumeContext<IOrderRegistrantAssigned> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.SourceId}))
            {
                _logger.LogInformation(
                    $"Processing IOrderRegistrantAssigned for {@event.Message.SourceId} with email {@event.Message.Email}");

                var order = _managementDbContext.Orders.FirstOrDefault(x => x.Id == @event.Message.SourceId);

                order.OwnerEmail = @event.Message.Email;
                order.OwnerId = @event.Message.UserId;
                order.OwnerPhone = @event.Message.Phone;
                order.OwnerName = @event.Message.OwnerName;
                order.DeliveryLine1 = @event.Message.DeliveryLine1;
                order.DeliveryPostcode = @event.Message.DeliveryPostcode;

                await _managementDbContext.SaveChangesAsync(@event.CancellationToken);
            }

        }
    }
}