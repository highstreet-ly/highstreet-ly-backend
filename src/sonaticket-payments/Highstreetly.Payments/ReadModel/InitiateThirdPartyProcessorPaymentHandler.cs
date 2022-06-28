using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Payments.Resources;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class InitiateThirdPartyProcessorPaymentHandler
        : IConsumer<IInitiateThirdPartyProcessorPayment>
    {
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly ILogger<InitiateThirdPartyProcessorPaymentHandler> _logger;

        public InitiateThirdPartyProcessorPaymentHandler(
            ILogger<InitiateThirdPartyProcessorPaymentHandler> logger,
            PaymentsDbContext paymentsDbContext)
        {
            _logger = logger;
            _paymentsDbContext = paymentsDbContext;
        }

        public async Task Consume(
            ConsumeContext<IInitiateThirdPartyProcessorPayment> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                _paymentsDbContext.ThirdPartyPayments.Add(new ThirdPartyPayment
                {
                    Id = context.Message.PaymentId,
                    Description = context.Message.Description,
                    PaymentSourceId = context.Message.PaymentSourceId,
                    State = PaymentStates.Initiated
                });

                await _paymentsDbContext.SaveChangesAsync();
            }
        }
    }
}