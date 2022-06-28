using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterITicketsReservedDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterITicketsReserved>
    {
        public RegistrationProcessManagerRouterITicketsReservedDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterITicketsReserved)}")
        {
        }
    }
}