using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Reservations.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Handlers
{
    public class ConfirmOrderHandler : IConsumer<IConfirmOrder>
    {
        private readonly IEventSourcedRepository<Order> _repository;
        private readonly ILogger<ConfirmOrderHandler> _logger;

        public ConfirmOrderHandler(
            IEventSourcedRepository<Order> repository,
            ILogger<ConfirmOrderHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IConfirmOrder> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                try
                {
                    _logger.LogInformation("public Task Consume(ConsumeContext<ConfirmOrder> command)");
                    var order = _repository.Get(command.Message.OrderId);
                    order.Confirm(command.Message.Email);
                    await _repository.Save(order, command.Message.CorrelationId.ToString());
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Couldn't run IConfirmOrder  {command.Message.OrderId}", ex);
                    throw;
                }
            }
        }
    }
}