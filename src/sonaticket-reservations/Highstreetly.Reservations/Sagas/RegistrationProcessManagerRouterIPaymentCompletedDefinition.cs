using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManagerRouterIPaymentCompletedDefinition : HandlerDefinitionBase<RegistrationProcessManagerRouterIPaymentCompleted>
    {
        public RegistrationProcessManagerRouterIPaymentCompletedDefinition() :base($"reservations-{nameof(RegistrationProcessManagerRouterIPaymentCompleted)}")
        {
        }
    }
}