using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderConfirmedHandlerDefinition : HandlerDefinitionBase<OrderConfirmedHandler>
    {
        public OrderConfirmedHandlerDefinition() : base($"reservations-read-model-{nameof(OrderConfirmedHandler)}-handler")
        {
        }
    }
}