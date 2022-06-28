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
    public class RegistrationProcessManagerRouterITicketsReservedFixture
    {
        private EventSourcingProcessManagerTestHelper<ITicketsReserved> _sut;
        private Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
        private StubProcessManagerDataContext<RegistrationProcessManager> _stubProcessManagerDataContext;

        [SetUp]
        public void SetUp()
        {
            _stubProcessManagerDataContext = new StubProcessManagerDataContext<RegistrationProcessManager>();

            _contextFactory = () => _stubProcessManagerDataContext;
            _sut = new EventSourcingProcessManagerTestHelper<ITicketsReserved>();

            _sut.ServiceCollection
                .AddScoped<Func<IProcessManagerDataContext<RegistrationProcessManager>>>((sp) => _contextFactory)
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<RegistrationProcessManagerRouterITicketsReserved>();
                    cfg.AddConsumerTestHarness<RegistrationProcessManagerRouterITicketsReserved>();
                });

            _sut.Setup().GetAwaiter().GetResult();
        }

        [Test]
        public async Task when_reservation_accepted_then_routes_and_saves()
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

            await _sut.When(new TicketsReserved
            {
                SourceId = pm.OrderId,
                ReservationId = pm.ReservationId,
                ReservationDetails = new List<TicketQuantity>()
            });

            var consumed = await _sut.ThenConsumed<RegistrationProcessManagerRouterITicketsReserved>();
            consumed.Should().BeTrue();

            _stubProcessManagerDataContext.SavedProcesses.Count.Should().Be(1);
            _stubProcessManagerDataContext.DisposeCalled.Should().BeTrue();

        }
    }
}