using System.Collections.Generic;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class CompleteThirdPartyProcessorPaymentHandler
        : IConsumer<ICompleteThirdPartyProcessorPayment>
    {
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly ILogger<CompleteThirdPartyProcessorPaymentHandler> _logger;

        public CompleteThirdPartyProcessorPaymentHandler(

            ILogger<CompleteThirdPartyProcessorPaymentHandler> logger,
            PaymentsDbContext paymentsDbContext)
        {
            _logger = logger;
            _paymentsDbContext = paymentsDbContext;
        }

        public async Task Consume(
            ConsumeContext<ICompleteThirdPartyProcessorPayment> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
                // var model = await _paymentsDbContext.ThirdPartyPayments.FirstAsync(x => x.Id == context.Message.PaymentId);
                // model.State = PaymentStates.Completed;
                // model.Amount = context.Message.Amount;
                // model.TotalAmount = context.Message.Amount;
                // model.Last4 = context.Message.Last4;
                // model.ApplicationFeeAmount = context.Message.ApplicationFeeAmount;
                // model.Created = context.Message.Created;
                // model.Currency = context.Message.Currency;
                // await _paymentsDbContext.SaveChangesAsync();
            }
        }
    }
}