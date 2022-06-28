using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class RejectOrderHandler : IConsumer<IRejectOrder>
    {
        private readonly ILogger<RejectOrderHandler> _logger;
        private readonly IEventSourcedRepository<Order> _repository;

        public RejectOrderHandler(
            ILogger<RejectOrderHandler> logger,
            IEventSourcedRepository<Order> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task Consume(
            ConsumeContext<IRejectOrder> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                try
                {
                    var order = _repository.Find(command.Message.OrderId);
                    // Explicitly idempotent. 
                    if (order != null)
                    {
                        order.Expire();
                        await _repository.Save(order, command.Message.CorrelationId.ToString());
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Couldn't run IRejectOrder  {command.Message.OrderId}", ex);
                    throw;
                }

            }
        }
    }
}