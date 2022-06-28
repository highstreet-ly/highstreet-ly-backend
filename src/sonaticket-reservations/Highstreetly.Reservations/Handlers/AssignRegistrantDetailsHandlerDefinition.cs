using Highstreetly.Infrastructure;

namespace Highstreetly.Reservations.Handlers
{
    public class AssignRegistrantDetailsHandlerDefinition : HandlerDefinitionBase<AssignRegistrantDetailsHandler>
    {
        public AssignRegistrantDetailsHandlerDefinition() : base($"reservations-handlers-{nameof(AssignRegistrantDetailsHandler)}-handler")
        {
        }
    }
}