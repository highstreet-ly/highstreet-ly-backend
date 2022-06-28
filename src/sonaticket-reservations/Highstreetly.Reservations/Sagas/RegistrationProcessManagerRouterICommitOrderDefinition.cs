using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterICommitOrderDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterICommitOrder>
    {
        public RegistrationProcessManagerRouterICommitOrderDefinition() : base($"reservations-{nameof(RegistrationProcessManagerRouterICommitOrder)}")
        {
        }
    }
}