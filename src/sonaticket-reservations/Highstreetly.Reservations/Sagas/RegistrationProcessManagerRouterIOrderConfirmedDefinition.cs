using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIOrderConfirmedDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterIOrderConfirmed>
    {
        public RegistrationProcessManagerRouterIOrderConfirmedDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterIOrderConfirmed)}")
        {
        }
    }
}