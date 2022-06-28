using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Payments.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Highstreetly.Payments.Handlers
{
    public class IssueRefundHandler : IConsumer<IIssueRefund>
    {
        private readonly ILogger<IssueRefundHandler> _logger;
        private readonly Func<IDataContext<ThirdPartyProcessorPayment>> _contextFactory;
        private readonly PaymentsDbContext _paymentsDbContext;
        private readonly string _apiKey;

        public IssueRefundHandler(
            ILogger<IssueRefundHandler> logger,
            Func<IDataContext<ThirdPartyProcessorPayment>> contextFactory, PaymentsDbContext paymentsDbContext, IConfiguration config)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _paymentsDbContext = paymentsDbContext;
            _apiKey = config.GetSection("Stripe")["ApiKey"];
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task Consume(
            ConsumeContext<IIssueRefund> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.Id }))
            {
               
                
                var refund = _paymentsDbContext
                             .Refunds
                             .Include(x => x.Charge)
                             .First(x => x.Id == context.Message.RefundId);
                
                var repository = _contextFactory();
            
                var payment = repository.Find(refund.Charge.PaymentId);
            
                if (payment != null)
                {
                        payment.IssueRefund(refund.Amount, refund.Charge.ChargeId, refund.Id);
                        repository.Save(payment);

                        refund.Status = "Refunded";
                        await _paymentsDbContext.SaveChangesAsync();
                    
                }
                else
                {
                    _logger.LogError(
                        "Failed to locate the payment entity with id {0} for the completed third party payment.",
                        refund.Charge.PaymentId);
                }
            }
        }
    }
}