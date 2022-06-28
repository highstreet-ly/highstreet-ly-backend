using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class AvailableTicketsChangedHandlerDefinition : HandlerDefinitionBase<AvailableTicketsChangedHandler>
    {
        public AvailableTicketsChangedHandlerDefinition() :base($"management-read-model-{nameof(AvailableTicketsChangedHandler)}-handler")
        {
        }
    }
}