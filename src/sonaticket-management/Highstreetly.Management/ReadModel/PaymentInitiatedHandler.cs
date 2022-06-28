using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class PaymentInitiatedHandler : IConsumer<IPaymentInitiated>
    {
        private readonly ILogger<PaymentInitiatedHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public PaymentInitiatedHandler(
            ManagementDbContext managementDbContext,
            ILogger<PaymentInitiatedHandler> logger)
        {
            _managementDbContext = managementDbContext;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<IPaymentInitiated> context)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.CorrelationId, ["SourceId"] = context.Message.SourceId }))
            {
                var order = _managementDbContext.Orders.FirstOrDefault(x => x.Id == @context.Message.PaymentSourceId);
                order.PaymentId = context.Message.SourceId;

                await _managementDbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}