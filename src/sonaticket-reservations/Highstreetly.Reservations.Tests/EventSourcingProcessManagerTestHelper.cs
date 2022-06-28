using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Processors;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Highstreetly.Reservations.Tests
{
    public class EventSourcingProcessManagerTestHelper<TEvent>
        where TEvent : class, ISonaticketEvent
    {
        private ServiceProvider _serviceProvider;
        public ServiceCollection ServiceCollection = new();
        private IBus _client;
      
        private Guid _expectedCorrelationId;

        public async Task Setup()
        {
            _serviceProvider = ServiceCollection.BuildServiceProvider(true);
            var harness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();
            await harness.Start();

            _client = _serviceProvider.GetRequiredService<IBus>();
        }

        public async Task When(ISonaticketEvent @event)
        {
            _expectedCorrelationId = @event.CorrelationId;
            await _client.Publish<TEvent>(@event);
            _expectedCorrelationId = default;
        }

        public async Task<bool> ThenConsumed<TEventHandler>() where TEventHandler : class, IConsumer<TEvent>
        {
            var harness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();
            var harnessConsumed = await harness.Consumed.Any<TEvent>();
            var consumerHarness = _serviceProvider.GetRequiredService<IConsumerTestHarness<TEventHandler>>();
            var consumerHarnessConsumed = await consumerHarness.Consumed.Any<TEvent>();
            harnessConsumed.Should().Be(true);
            consumerHarnessConsumed.Should().Be(true);
            return harnessConsumed && consumerHarnessConsumed;
        }
    }

    public class StubProcessManagerDataContext<T> : IProcessManagerDataContext<T> where T : class, IProcessManager
    {
        public readonly List<T> SavedProcesses = new List<T>();

        public readonly List<T> Store = new List<T>();

        public bool DisposeCalled { get; set; }

        public T Find(Guid id)
        {
            return this.Store.SingleOrDefault(x => x.Id == id);
        }

        public void Save(T processManager)
        {
            this.SavedProcesses.Add(processManager);
        }

        public T Find(Expression<Func<T, bool>> predicate, bool includeCompleted = false)
        {
            return this.Store.AsQueryable().Where(x => includeCompleted || !x.Completed).SingleOrDefault(predicate);
        }

        public void Dispose()
        {
            this.DisposeCalled = true;
        }
    }
}