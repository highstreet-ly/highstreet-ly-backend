using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class OrderConfirmedHandlerDefinition : HandlerDefinitionBase<OrderConfirmedHandler>
    {
        public OrderConfirmedHandlerDefinition() :base($"management-handlers-{nameof(OrderConfirmedHandler)}-handler")
        {
        }
        
    }
}