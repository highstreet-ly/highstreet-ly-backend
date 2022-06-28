using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderPlacedHandlerDefinition : HandlerDefinitionBase<OrderPlacedHandler>
    {
        public OrderPlacedHandlerDefinition() :base($"management-read-model-{nameof(OrderPlacedHandler)}-handler")
        {
        }
    }
}