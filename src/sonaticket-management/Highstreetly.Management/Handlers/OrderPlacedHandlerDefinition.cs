using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class OrderPlacedHandlerDefinition : HandlerDefinitionBase<OrderPlacedHandler>
    {
        public OrderPlacedHandlerDefinition() :base($"management-handlers-{nameof(OrderPlacedHandler)}-handler")
        {
        }
    }
}