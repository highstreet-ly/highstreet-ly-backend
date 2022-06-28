using System.Threading.Tasks;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class PaymentCompletedHandler : IConsumer<IPaymentCompleted>
    { 
        private readonly ILogger<PaymentCompletedHandler> _logger;

        public PaymentCompletedHandler(ILogger<PaymentCompletedHandler> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IPaymentCompleted> context)
        {
            
                
            _logger.LogInformation($"Running IPaymentCompleted");
            return Task.CompletedTask;
        }
    }
}