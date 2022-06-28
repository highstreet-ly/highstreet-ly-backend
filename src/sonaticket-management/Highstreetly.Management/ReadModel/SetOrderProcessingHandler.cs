using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.ReadModel
{
    public class SetOrderProcessingHandler : IConsumer<ISetOrderProcessing>
    {
        private readonly ILogger<SetOrderProcessingHandler> _log;


        public SetOrderProcessingHandler(ILogger<SetOrderProcessingHandler> log)
        {
            _log = log;
        }

        public Task Consume(ConsumeContext<ISetOrderProcessing> command)
        {
            return Task.CompletedTask;
        }
    }
}