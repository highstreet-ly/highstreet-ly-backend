using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.EventSourcing;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Highstreetly.Reservations.Tests
{
    public class EventSourcingCommandHandlerTestHelper<TEventSourced, TCommand>
        where TEventSourced : IEventSourced
        where TCommand : class, ICommand
    {
        private readonly RepositoryStub _repository;
        private string _expectedCorrelationId;
        private ServiceProvider _serviceProvider;
        public ServiceCollection ServiceCollection = new();
        private IBus _client;

        public EventSourcingCommandHandlerTestHelper()
        {
            Events = new List<ISonaticketEvent>();
            _repository =
                new RepositoryStub((eventSourced, correlationId) =>
                {
                    _expectedCorrelationId?.Should().Be(correlationId);

                    Events.AddRange(eventSourced.Events.Select(x => x.Value));
                });
        }

        public List<ISonaticketEvent> Events { get; }

        public IEventSourcedRepository<TEventSourced> Repository => _repository;

        public async Task Setup()
        {
            ServiceCollection.AddSingleton(Repository);
            _serviceProvider = ServiceCollection.BuildServiceProvider(true);
            var harness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();
            await harness.Start();

            _client = _serviceProvider.GetRequiredService<IBus>();
        }

        public void Given(params Tuple<Type, ISonaticketEvent>[] history)
        {
            _repository.History.AddRange(history);
        }

        public async Task When(TCommand command)
        {
            _expectedCorrelationId = command.Id.ToString();
            await _client.Publish(command);
            _expectedCorrelationId = null;
        }

        public bool ThenContains<TEvent>() where TEvent : ISonaticketEvent
        {
            return Events.Any(x => x.GetType() == typeof(TEvent));
        }

        public TEvent ThenHasSingle<TEvent>() where TEvent : ISonaticketEvent
        {
            Events.Count.Should().Be(1);
            var @event = Events.Single();
            Assert.IsAssignableFrom<TEvent>(@event);
            return (TEvent)@event;
        }

        public async Task<bool> ThenConsumed<TCommandHandler>() where TCommandHandler : class, IConsumer<TCommand>
        {
            var harness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();
            var harnessConsumed = await harness.Consumed.Any<TCommand>();
            var consumerHarness = _serviceProvider.GetRequiredService<IConsumerTestHarness<TCommandHandler>>();
            var consumerHarnessConsumed = await consumerHarness.Consumed.Any<TCommand>();
            harnessConsumed.Should().Be(true);
            consumerHarnessConsumed.Should().Be(true);
            return harnessConsumed && consumerHarnessConsumed;
        }


        public TEvent ThenHasOne<TEvent>() where TEvent : ISonaticketEvent
        {
            Events.OfType<TEvent>().Count().Should().Be(1);
            var @event = Events.OfType<TEvent>().Single();
            return @event;
        }

        public async Task Done()
        {
            var harness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();
            await harness.Stop();

            await _serviceProvider.DisposeAsync();
        }

        private class RepositoryStub : IEventSourcedRepository<TEventSourced>
        {
            public readonly List<Tuple<Type, ISonaticketEvent>> History = new();
            private readonly Action<TEventSourced, string> _onSave;
            private readonly Func<Guid, IEnumerable<ISonaticketEvent>, TEventSourced> _entityFactory;

            internal RepositoryStub(Action<TEventSourced, string> onSave)
            {
                _onSave = onSave;
                var constructor = typeof(TEventSourced).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<ISonaticketEvent>) });
                if (constructor == null)
                {
                    throw new InvalidCastException(
                        "Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<ISonaticketEvent>)");
                }
                _entityFactory = (id, events) => (TEventSourced)constructor.Invoke(new object[] { id, events });
            }

            public TEventSourced Find(Guid id, int? version = null)
            {
                var all = History.Where(x => x.Item2.SourceId == id).Select(x => x.Item2).ToList();
                if (all.Count > 0)
                {
                    return _entityFactory.Invoke(id, all);
                }

                return default;
            }

            TEventSourced IEventSourcedRepository<TEventSourced>.Get(Guid id)
            {
                var entity = ((IEventSourcedRepository<TEventSourced>)this).Find(id);
                if (Equals(entity, default(TEventSourced)))
                {
                    throw new EntityNotFoundException(id, "Test");
                }

                return entity;
            }

            public Task Save(TEventSourced eventSourced, string correlationId)
            {
                _onSave(eventSourced, correlationId);

                return Task.CompletedTask;
            }
        }
    }
}
