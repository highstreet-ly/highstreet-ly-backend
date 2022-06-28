using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Highstreetly.Payments.ReadModel
{
    public class RefundIssuedHandler : IConsumer<IRefundIssued>
    {
        private readonly ILogger<RefundIssuedHandler> _logger;
        private readonly string _apiKey;
        private readonly PaymentsDbContext _paymentsDbContext;

        public RefundIssuedHandler(
            ILogger<RefundIssuedHandler> logger,
            IConfiguration config,
            PaymentsDbContext paymentsDbContext)
        {
            _logger = logger;
            _paymentsDbContext = paymentsDbContext;
            _apiKey = config.GetSection("Stripe")["ApiKey"];
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task Consume(
            ConsumeContext<IRefundIssued> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                // var service = new RefundService();
                //
                // var payment = _paymentsDbContext.Payments.Single(x => x.Id == context.Message.SourceId);
                //
                // var charges = _paymentsDbContext.Charges.Where(x => x.PaymentId == payment.Id);
                //
                // foreach (var charge in charges)
                // {
                //     var options = new RefundCreateOptions
                //     {
                //         PaymentIntent = payment.PaymentIntentId,
                //         ReverseTransfer = true,
                //         Reason = "requested_by_customer",
                //         
                //     };
                //
                //     await service.CreateAsync(options, new RequestOptions
                //     {
                //         ApiKey = _apiKey
                //     });
                //
                //     charge.Refunded = true;
                // }
                //
                // await _paymentsDbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}