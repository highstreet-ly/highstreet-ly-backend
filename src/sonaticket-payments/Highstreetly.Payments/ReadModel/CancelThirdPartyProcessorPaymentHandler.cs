using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class CancelThirdPartyProcessorPaymentHandler : IConsumer<ICancelThirdPartyProcessorPayment>
    {
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly ILogger<CompleteThirdPartyProcessorPaymentHandler> _logger;

        public CancelThirdPartyProcessorPaymentHandler(
            PaymentsDbContext paymentsDbContext,
            ILogger<CompleteThirdPartyProcessorPaymentHandler> logger)
        {
            _paymentsDbContext = paymentsDbContext;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ICancelThirdPartyProcessorPayment> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                var model = await _paymentsDbContext.ThirdPartyPayments.FirstAsync(x =>
                    x.Id == context.Message.PaymentId);
                model.State = PaymentStates.Rejected;
                model.Description = context.Message.Reason;
                model.OutcomeCode = context.Message.Code;
                await _paymentsDbContext.SaveChangesAsync();
            }
        }
    }
}
