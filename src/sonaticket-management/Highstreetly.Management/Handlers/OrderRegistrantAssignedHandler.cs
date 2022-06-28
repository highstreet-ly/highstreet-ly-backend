using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers
{
    public class OrderRegistrantAssignedHandler
        : OrderEventHandlerBase<OrderRegistrantAssignedHandler>, IConsumer<IOrderRegistrantAssigned>
    {
        public OrderRegistrantAssignedHandler(
            ILogger<OrderRegistrantAssignedHandler> logger,
            ManagementDbContext managementDbContext)
            : base(logger, managementDbContext) { }

        public async Task Consume(
            ConsumeContext<IOrderRegistrantAssigned> @event)
        {
            using (Logger.BeginScope(@event))
            {
                Logger.LogInformation(
                    $"Processing IOrderRegistrantAssigned for {@event.Message.SourceId} with email {@event.Message.Email}");

                await ProcessOrder(order => order.Id == @event.Message.SourceId, order =>
                {
                    order.RegistrantUserId = @event.Message.UserId;
                    order.OwnerEmail = @event.Message.Email;
                    order.OwnerName = @event.Message.OwnerName;
                    return Task.CompletedTask;
                });
            }

        }
    }
}