using System;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Processors;
using Highstreetly.Reservations.Sagas;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OrderConfirmed = Highstreetly.Infrastructure.Events.OrderConfirmed;

namespace Highstreetly.Reservations.Tests.Sagas
{
    public class RegistrationProcessManagerRouterIOrderConfirmedFixture
    {
        private EventSourcingProcessManagerTestHelper<IOrderConfirmed> _sut;
        private Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        private StubProcessManagerDataContext<RegistrationProcessManager> _stubProcessManagerDataContext;

        [SetUp]
        public void SetUp()
        {
            _stubProcessManagerDataContext = new StubProcessManagerDataContext<RegistrationProcessManager>();

            _contextFactory = () => _stubProcessManagerDataContext;
            _sut = new EventSourcingProcessManagerTestHelper<IOrderConfirmed>();

            _sut.ServiceCollection
                .AddScoped<Func<IProcessManagerDataContext<RegistrationProcessManager>>>((sp) => _contextFactory)
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<RegistrationProcessManagerRouterIOrderConfirmed>();
                    cfg.AddConsumerTestHarness<RegistrationProcessManagerRouterIOrderConfirmed>();
                });

            _sut.Setup().GetAwaiter().GetResult();
        }

        [Test]
        public async Task when_order_confirmed_received_then_routes_and_saves()
        {
            var pm = new RegistrationProcessManager
            {
                State = RegistrationProcessManager.ProcessState.ReservationConfirmationReceived,
                OrderId = Guid.NewGuid(),
                ReservationId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                ReservationAutoExpiration = DateTime.UtcNow.AddMinutes(10)
            };

            _stubProcessManagerDataContext.Store.Add(pm);

            await _sut.When(new OrderConfirmed()
            {
                SourceId = pm.OrderId,
            });

            var consumed = await _sut.ThenConsumed<RegistrationProcessManagerRouterIOrderConfirmed>();
            consumed.Should().BeTrue();

            _stubProcessManagerDataContext.SavedProcesses.Count.Should().Be(1);
            _stubProcessManagerDataContext.DisposeCalled.Should().BeTrue();
        }
    }
}