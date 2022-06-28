using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPlacedHandlerHandlerDefinition : HandlerDefinitionBase<OrderPlacedHandler>
    {
        public OrderPlacedHandlerHandlerDefinition() :base($"reservations-read-model-{nameof(OrderPlacedHandler)}-handler")
        {
        }
    }
}