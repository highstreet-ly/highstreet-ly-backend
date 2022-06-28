using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class OrderExpiredHandlerDefinition : HandlerDefinitionBase<OrderExpiredHandler>
    {
        public OrderExpiredHandlerDefinition() :base($"management-handlers-{nameof(OrderExpiredHandler)}-handler")
        {
        }
    }
}