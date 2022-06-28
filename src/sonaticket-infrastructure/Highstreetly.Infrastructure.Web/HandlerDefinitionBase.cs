using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace Highstreetly.Infrastructure
{
    public abstract class HandlerDefinitionBase<T> :
        ConsumerDefinition<T> where T : class, IConsumer
    {
        protected HandlerDefinitionBase(string endpoint)
        {
            // override the default endpoint name
            EndpointName = endpoint;

            // limit the number of messages consumed concurrently
            // this applies to the consumer only, not the endpoint
            ConcurrentMessageLimit = 1;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<T> consumerConfigurator)
        {
            // configure message retry with millisecond intervals
            endpointConfigurator.UseMessageRetry(r => r.Intervals(1000, 2000, 5000, 8000, 10000));

            // use the outbox to prevent duplicate events from being published
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}