using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Processors;
using Highstreetly.Reservations.Sagas;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Highstreetly.Reservations.Tests.Sagas
{
    public class RegistrationProcessManagerRouterIOrderPlacedFixture
    {
        private EventSourcingProcessManagerTestHelper<IOrderPlaced> _sut;
        private Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        private StubProcessManagerDataContext<RegistrationProcessManager> _stubProcessManagerDataContext;

        [SetUp]
        public void SetUp()
        {
            _stubProcessManagerDataContext = new StubProcessManagerDataContext<RegistrationProcessManager>();

            _contextFactory = () => _stubProcessManagerDataContext;
            _sut = new EventSourcingProcessManagerTestHelper<IOrderPlaced>();

            _sut.ServiceCollection
                .AddScoped<Func<IProcessManagerDataContext<RegistrationProcessManager>>>((sp) => _contextFactory)
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<RegistrationProcessManagerRouterIOrderPlaced>();
                    cfg.AddConsumerTestHarness<RegistrationProcessManagerRouterIOrderPlaced>();
                });

            _sut.Setup().GetAwaiter().GetResult();
        }

        [Test]
        public async Task when_order_placed_then_routes_and_saves()
        {
            await _sut.When(new OrderPlaced
            {
                SourceId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                Tickets = new List<TicketQuantity>()
            });

            var consumed = await _sut.ThenConsumed<RegistrationProcessManagerRouterIOrderPlaced>();
            consumed.Should().BeTrue();

            _stubProcessManagerDataContext.SavedProcesses.Count.Should().Be(1);
            _stubProcessManagerDataContext.DisposeCalled.Should().BeTrue();
        }

        [Test]
        public async Task when_order_placed_it_is_reprocessed_then_routes_and_saves()
        {
            var pm = new RegistrationProcessManager
            {
                State = RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation,
                OrderId = Guid.NewGuid(),
                ReservationId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                ReservationAutoExpiration = DateTime.UtcNow.AddMinutes(10)
            };

            _stubProcessManagerDataContext.Store.Add(pm);
            await _sut.When(new OrderPlaced
            {
                SourceId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                Tickets = new List<TicketQuantity>()
            });

            var consumed = await _sut.ThenConsumed<RegistrationProcessManagerRouterIOrderPlaced>();
            consumed.Should().BeTrue();

            _stubProcessManagerDataContext.SavedProcesses.Count.Should().Be(1);
            _stubProcessManagerDataContext.DisposeCalled.Should().BeTrue();
        }
    }
}
