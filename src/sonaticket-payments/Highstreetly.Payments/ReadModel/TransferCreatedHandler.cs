using System.Threading.Tasks;
using Highstreetly.Infrastructure.CloudStorage;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.ReadModel
{
    public class TransferCreatedHandler : IConsumer<ITransferCreated>
    {
        private readonly ILogger<TransferCreatedHandler> _logger;
        private readonly IAzureStorage _azureStorage;

        public TransferCreatedHandler(ILogger<TransferCreatedHandler> logger, IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }
        
        public  Task Consume(ConsumeContext<ITransferCreated> context)
        {
            return Task.CompletedTask;
            //var stripeEvent = _azureStorage.ReadJsonPayloadFromAuzureBlob( context.Message.HsEventId);

            // var transfer = JsonConvert.DeserializeObject<PaymentIntent>(stripeEvent.Data);
            //     
            // _logger.LogInformation($"Succeeded fetching intent from payload: {transfer.Id}");
            //
            // _azureStorage.Store(transfer);

            //await  _azureStorage.SaveChangesAsync();
        }
    }
}