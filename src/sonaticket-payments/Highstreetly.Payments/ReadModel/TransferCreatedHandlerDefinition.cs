using Highstreetly.Infrastructure;

namespace Highstreetly.Payments.ReadModel
{
    public class TransferCreatedHandlerDefinition : HandlerDefinitionBase<TransferCreatedHandler>
    {
        public TransferCreatedHandlerDefinition() : base($"payments-read-model-{nameof(TransferCreatedHandler)}")
        {
        }
    }
}