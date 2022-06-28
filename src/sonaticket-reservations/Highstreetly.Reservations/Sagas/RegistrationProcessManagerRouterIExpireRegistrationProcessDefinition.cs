using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIExpireRegistrationProcessDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterIExpireRegistrationProcess>
    {
        public RegistrationProcessManagerRouterIExpireRegistrationProcessDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterIExpireRegistrationProcess)}")
        {
        }
    }
}