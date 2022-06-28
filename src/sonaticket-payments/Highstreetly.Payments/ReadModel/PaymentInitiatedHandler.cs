using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentInitiatedHandler : IConsumer<IPaymentInitiated>
    { 
        private readonly ILogger<PaymentInitiatedHandler> _logger;

        public PaymentInitiatedHandler(ILogger<PaymentInitiatedHandler> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IPaymentInitiated> context)
        {
           
            _logger.LogInformation($"IPaymentInitiated");
            
            return Task.CompletedTask;
        }
    }
}