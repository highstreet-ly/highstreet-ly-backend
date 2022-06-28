using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.ReadModel
{
    public class OrderReservationCompletedHandlerDefinition : HandlerDefinitionBase<OrderReservationCompletedHandler>
    {
        public OrderReservationCompletedHandlerDefinition() :base($"reservations-read-model-{nameof(OrderReservationCompletedHandler)}-handler")
        {
        }
    }
}