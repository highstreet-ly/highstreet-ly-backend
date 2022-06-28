using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderRegistrantAssignedHandlerDefinition : HandlerDefinitionBase<OrderRegistrantAssignedHandler>
    {
        public OrderRegistrantAssignedHandlerDefinition() :base($"reservations-read-model-{nameof(OrderRegistrantAssignedHandler)}-handler")
        {
        }
    }
}