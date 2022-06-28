using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPaymentConfirmedHandlerDefinition : HandlerDefinitionBase<OrderPaymentConfirmedHandler>
    {
        public OrderPaymentConfirmedHandlerDefinition() : base($"reservations-read-model-{nameof(OrderPaymentConfirmedHandler)}-handler")
        {
        }
        
    }
}