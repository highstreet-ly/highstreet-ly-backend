using Highstreetly.Infrastructure;

namespace Highstreetly.Management.Handlers
{
    public class OrderRegistrantAssignedHandlerDefinition : HandlerDefinitionBase<OrderRegistrantAssignedHandler>
    {
        public OrderRegistrantAssignedHandlerDefinition() :base($"management-handlers-{nameof(OrderRegistrantAssignedHandler)}-handler")
        {
        }
    }
}