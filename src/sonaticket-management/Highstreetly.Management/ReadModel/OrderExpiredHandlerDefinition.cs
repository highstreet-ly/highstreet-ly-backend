using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderExpiredHandlerDefinition : HandlerDefinitionBase<OrderExpiredHandler>
    {
        public OrderExpiredHandlerDefinition() :base($"management-read-model-{nameof(OrderExpiredHandler)}-handler")
        {
        }
    }
}