using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Payments.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Handlers
{
    public class CancelThirdPartyProcessorPaymentHandler
        : IConsumer<ICancelThirdPartyProcessorPayment>
    {
        private readonly Func<IDataContext<ThirdPartyProcessorPayment>> _contextFactory;
        private readonly ILogger<CancelThirdPartyProcessorPaymentHandler> _logger;

        public CancelThirdPartyProcessorPaymentHandler(
            Func<IDataContext<ThirdPartyProcessorPayment>> contextFactory,
            ILogger<CancelThirdPartyProcessorPaymentHandler> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<ICancelThirdPartyProcessorPayment> command)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = command.CorrelationId, ["SourceId"] = command.Message.Id }))
            {
                var repository = _contextFactory();

                var payment = repository.Find(command.Message.PaymentId);

                if (payment != null)
                {
                    payment.Cancel(command.Message.Reason);
                    repository.Save(payment);
                }
                else
                {
                    _logger.LogError(
                        "Failed to locate the payment entity with id {0} for the cancelled third party payment.",
                        command.Message.PaymentId);
                }

                return Task.CompletedTask;
            }
        }
    }
}