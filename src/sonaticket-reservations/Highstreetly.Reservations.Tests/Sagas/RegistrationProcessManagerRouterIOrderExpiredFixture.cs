namespace Highstreetly.Reservations.Tests.Sagas
{
    // TODO: I need to make another EventSourcingProcessManagerTestHelper to handle commands

    // public class RegistrationProcessManagerRouterIExpireRegistrationProcessFixture
    // {
    //     private EventSourcingProcessManagerTestHelper<IExpireRegistrationProcess> _sut;
    //     private Func<IProcessManagerDataContext<RegistrationProcessManager>> _contextFactory;
    //     private StubProcessManagerDataContext<RegistrationProcessManager> _stubProcessManagerDataContext;
    //
    //     [SetUp]
    //     public void SetUp()
    //     {
    //         _stubProcessManagerDataContext = new StubProcessManagerDataContext<RegistrationProcessManager>();
    //
    //         _contextFactory = () => _stubProcessManagerDataContext;
    //         _sut = new EventSourcingProcessManagerTestHelper<IExpireRegistrationProcess>();
    //
    //         _sut.ServiceCollection
    //             .AddScoped<Func<IProcessManagerDataContext<RegistrationProcessManager>>>((sp) => _contextFactory)
    //             .AddMassTransitInMemoryTestHarness(cfg =>
    //             {
    //                 cfg.AddConsumer<RegistrationProcessManagerRouterIExpireRegistrationProcess>();
    //                 cfg.AddConsumerTestHarness<RegistrationProcessManagerRouterIExpireRegistrationProcess>();
    //             });
    //
    //         _sut.Setup().GetAwaiter().GetResult();
    //     }
    //
    //     [Test]
    //     public async Task when_order_confirmed_received_then_routes_and_saves()
    //     {
    //         var pm = new RegistrationProcessManager
    //         {
    //             State = RegistrationProcessManager.ProcessState.ReservationConfirmationReceived,
    //             OrderId = Guid.NewGuid(),
    //             ReservationId = Guid.NewGuid(),
    //             EventInstanceId = Guid.NewGuid(),
    //             ReservationAutoExpiration = DateTime.UtcNow.AddMinutes(10)
    //         };
    //
    //         _stubProcessManagerDataContext.Store.Add(pm);
    //
    //         await _sut.When(new OrderConfirmed()
    //         {
    //             SourceId = pm.OrderId,
    //         });
    //
    //         var consumed = await _sut.ThenConsumed<RegistrationProcessManagerRouterIExpireRegistrationProcess>();
    //         consumed.Should().BeTrue();
    //
    //         _stubProcessManagerDataContext.SavedProcesses.Count.Should().Be(1);
    //         _stubProcessManagerDataContext.DisposeCalled.Should().BeTrue();
    //
    //     }
    // }
}