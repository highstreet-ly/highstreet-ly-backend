using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class OrderRegistrantAssignedHandlerDefinition : HandlerDefinitionBase<OrderRegistrantAssignedHandler>
    {
        public OrderRegistrantAssignedHandlerDefinition() :base($"management-read-model-{nameof(OrderRegistrantAssignedHandler)}-handler")
        {
        }
    }
}