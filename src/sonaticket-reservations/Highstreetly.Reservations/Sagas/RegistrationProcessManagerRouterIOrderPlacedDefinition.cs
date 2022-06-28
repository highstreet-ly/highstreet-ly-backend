using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIOrderPlacedDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterIOrderPlaced>
    {
        public RegistrationProcessManagerRouterIOrderPlacedDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterIOrderPlaced)}")
        {
        }
    }
}