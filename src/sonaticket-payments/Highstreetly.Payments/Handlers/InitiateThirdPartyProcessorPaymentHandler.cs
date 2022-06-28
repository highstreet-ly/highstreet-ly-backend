using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Payments.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Handlers
{
    public class InitiateThirdPartyProcessorPaymentHandler :
        IConsumer<IInitiateThirdPartyProcessorPayment>
    {
        private readonly Func<IDataContext<ThirdPartyProcessorPayment>> _contextFactory;
        private readonly ILogger<InitiateThirdPartyProcessorPaymentHandler> _logger;

        public InitiateThirdPartyProcessorPaymentHandler(
            Func<IDataContext<ThirdPartyProcessorPayment>> contextFactory,
            ILogger<InitiateThirdPartyProcessorPaymentHandler> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<IInitiateThirdPartyProcessorPayment> message)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = message.CorrelationId, ["SourceId"] = message.Message.Id }))
            {
                var repository = _contextFactory();
                var command = message.Message;

                var items = command.Items.Select(t => new ThirdPartyProcessorPaymentItem(t.Description, t.Amount))
                    .ToList();
                var payment = new ThirdPartyProcessorPayment(
                    command.PaymentId,
                    command.PaymentSourceId,
                    command.Description,
                    command.TotalAmount,
                    command.PaymentIntentId,
                    items);

                repository.Save(payment);
                return Task.CompletedTask;
            }
        }
    }
}
