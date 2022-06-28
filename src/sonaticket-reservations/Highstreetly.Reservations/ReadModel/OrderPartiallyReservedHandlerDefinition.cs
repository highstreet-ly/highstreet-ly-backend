using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderPartiallyReservedHandlerDefinition : HandlerDefinitionBase<OrderPartiallyReservedHandler>
    {
        public OrderPartiallyReservedHandlerDefinition() : base($"reservations-read-model-{nameof(OrderPartiallyReservedHandler)}-handler")
        {
        }
        
    }
}