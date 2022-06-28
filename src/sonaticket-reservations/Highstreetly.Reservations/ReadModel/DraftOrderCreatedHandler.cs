using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using System; 

namespace Highstreetly.Reservations.ReadModel
{
    public class DraftOrderCreatedHandler : IConsumer<IDraftOrderCreated>
    {
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly IPricingService _pricingService;
        private readonly ILogger<DraftOrderCreatedHandler> _logger;

        public DraftOrderCreatedHandler(
            IPricingService pricingService,
            IEventSourcedRepository<Order> repository,
            ILogger<DraftOrderCreatedHandler> logger)
        {
            _pricingService = pricingService;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IDraftOrderCreated> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.SourceId }))
            {
                _logger.LogInformation($"Handling {nameof(IDraftOrderCreated)}");

                var order = new Order(
                    command.Message.OrderId,
                    command.Message.EventInstanceId,
                    new List<OrderItem>(),
                    Guid.Empty,
                    null,
                    command.Message.CorrelationId,
                    command.Message.HumanReadableId,
                    command.Message.IsClickAndCollect,
                    command.Message.IsLocalDelivery,
                    command.Message.IsNationalDelivery,
                    command.Message.IsToTable,
                    command.Message.TableInfo);

                await _repository.Save(order, command.Message.CorrelationId.ToString());
            }
        }
    }
}
