using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentRejectedHandler : IConsumer<IPaymentRejected>
    {
        private readonly ILogger<PaymentRejectedHandler> _logger;

        public PaymentRejectedHandler(ILogger<PaymentRejectedHandler> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IPaymentRejected> context)
        {
            _logger.LogInformation("IPaymentRejected");
            
            return Task.CompletedTask;
        }
    }
}