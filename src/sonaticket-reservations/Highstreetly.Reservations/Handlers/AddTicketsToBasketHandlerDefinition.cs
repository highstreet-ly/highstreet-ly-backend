using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class AddTicketsToBasketHandlerDefinition : HandlerDefinitionBase<AddTicketsToBasketHandler>
    {
        public AddTicketsToBasketHandlerDefinition() :base($"reservations-handlers-{nameof(AddTicketsToBasketHandler)}-handler")
        {
        }
    }
}