using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIOrderUpdatedDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterIOrderUpdated>
    {
        public RegistrationProcessManagerRouterIOrderUpdatedDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterIOrderUpdated)}")
        {
        }
    }
}