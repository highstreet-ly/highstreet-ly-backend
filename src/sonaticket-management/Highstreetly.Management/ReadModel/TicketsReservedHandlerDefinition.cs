using Highstreetly.Infrastructure;

namespace Highstreetly.Management.ReadModel
{
    public class TicketsReservedHandlerDefinition : HandlerDefinitionBase<TicketsReservedHandler>
    {
        public TicketsReservedHandlerDefinition() :base($"management-read-model-{nameof(TicketsReservedHandler)}-handler")
        {
        }
    }
}