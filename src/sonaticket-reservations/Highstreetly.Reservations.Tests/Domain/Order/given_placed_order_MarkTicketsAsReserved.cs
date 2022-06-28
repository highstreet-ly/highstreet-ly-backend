using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts.Requests;
using Highstreetly.Permissions.Contracts.Requests;
using Highstreetly.Reservations.Handlers;
using Highstreetly.Reservations.Resources;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using OrderTicketDetails = Highstreetly.Infrastructure.MessageDtos.OrderTicketDetails;

namespace Highstreetly.Reservations.Tests.Domain.Order
{
    public class given_placed_order_MarkTicketsAsReserved
    {
        private const string OwnerEmail = "test@test.com";
        private static readonly Guid OrderId = Guid.NewGuid();
        private static readonly Guid EventInstanceId = Guid.NewGuid();
        private static readonly Guid OwnerId = Guid.NewGuid();
        private static readonly Guid TicketTypeId = Guid.NewGuid();

        private static readonly OrderTotal OrderTotal = new()
        {
            Total = 33,
            Lines = new List<TicketOrderLine>
            {
                new()
            }
        };

        private Mock<IJsonApiClient<EventInstance, Guid>> _eventInstanceClient;
        private Mock<ReservationDbContext> _mockContext;
        private Mock<IPricingService> _pricingService;
        private EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IMarkTicketsAsReserved> _sut;
        private Mock<IJsonApiClient<User, Guid>> _userClient;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _sut = new EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IMarkTicketsAsReserved>();
            _mockContext = new Mock<ReservationDbContext>("test");
            var orderData = new List<DraftOrder>
            {
                new()
                {
                    Id = OrderId,
                    OwnerEmail = OwnerEmail,
                    OwnerId = OwnerId,
                    EventInstanceId = EventInstanceId
                }
            }.AsQueryable();

            var mockOrderSet = orderData.AsQueryable()
                .BuildMockDbSet();
            var mockPricedOrderSet = new List<PricedOrder>().AsQueryable()
                .BuildMockDbSet();


            _mockContext.Setup(m => m.DraftOrders)
                .Returns(mockOrderSet.Object);
            _mockContext.Setup(m => m.PricedOrders)
                .Returns(mockPricedOrderSet.Object);

            _userClient = new Mock<IJsonApiClient<User, Guid>>();

            _userClient
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<QueryBuilder>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new User
                {
                    Email = OwnerEmail,
                    Id = OwnerId
                });

            _eventInstanceClient = new Mock<IJsonApiClient<EventInstance, Guid>>();
            _eventInstanceClient
                .Setup(x =>
                    x.GetAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<QueryBuilder>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(new EventInstance
                {
                    Id = OwnerId,
                    Name = "test",
                    Slug = "test slug".GenerateSlug(),
                    MainImageId = "1",
                    ShortLocation = "test location"
                });

            _pricingService = new Mock<IPricingService>();

            _pricingService.Setup(x => x.CalculateTotal(EventInstanceId, OrderId, It.IsAny<List<TicketQuantity>>()))
                .ReturnsAsync(OrderTotal);

            _sut.ServiceCollection
                .AddSingleton(_ => _mockContext.Object)
                .AddSingleton(_ => _userClient.Object)
                .AddSingleton(_ => _eventInstanceClient.Object)
                .AddSingleton(_ => _pricingService.Object)
                .AddLogging()
                .AddHttpClient()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<MarkTicketsAsReservedHandler>();
                    cfg.AddConsumerTestHarness<MarkTicketsAsReservedHandler>();
                });

            await _sut.Setup();
            _sut.Given(new Tuple<Type, ISonaticketEvent>(typeof(IOrderPlaced), new OrderPlaced
            {
                SourceId = OrderId,
                EventInstanceId = EventInstanceId,
                Tickets = new List<TicketQuantity>
                {
                    new(Guid.NewGuid(), 1, new OrderTicketDetails())
                }
            }));
        }

        [Test]
        public async Task when_marking_a_subset_of_items_as_reserved_then_order_is_partially_reserved()
        {
            var expiration = DateTime.UtcNow.AddMinutes(15);
            await _sut.When(new MarkTicketsAsReserved
            {
                OrderId = OrderId,
                Expiration = expiration,
                Tickets = new List<TicketQuantity> { new(TicketTypeId, 3, new OrderTicketDetails()) }
            });

            var consumed = await _sut.ThenConsumed<MarkTicketsAsReservedHandler>();
            consumed.Should()
                .BeTrue();

            var @event = _sut.ThenHasOne<OrderPartiallyReserved>();

            @event.SourceId.Should()
                .Be(OrderId);
            @event.Tickets.Count()
                .Should()
                .Be(1);
            @event.Tickets.ElementAt(0)
                .Quantity.Should()
                .Be(3);
            @event.ReservationExpiration.Should()
                .Be(expiration);
        }

        [Test]
        public async Task when_marking_a_subset_of_items_as_reserved_then_totals_are_calculated()
        {
            var expiration = DateTime.UtcNow.AddMinutes(15);
            await _sut.When(new MarkTicketsAsReserved
            {
                OrderId = OrderId,
                Expiration = expiration,
                Tickets = new List<TicketQuantity> { new(TicketTypeId, 3, new OrderTicketDetails()) }
            });

            var consumed = await _sut.ThenConsumed<MarkTicketsAsReservedHandler>();
            consumed.Should()
                .BeTrue();

            var @event = _sut.ThenHasOne<OrderTotalsCalculated>();
            @event.SourceId.Should()
                .Be(OrderId);
            @event.Total.Should()
                .Be(33);
            @event.Lines.Count()
                .Should()
                .Be(1);
            @event.Lines.Single()
                .Should()
                .Be(OrderTotal.Lines.Single());

            _pricingService.Verify(s => s.CalculateTotal(EventInstanceId, OrderId, It.Is<List<TicketQuantity>>(x => x
                .Single()
                .TicketType == TicketTypeId && x.Single()
                .Quantity == 3)));
        }

        // [Test]
        // public async Task when_marking_all_items_as_reserved_then_order_is_reserved()
        // {
        //     var expiration = DateTime.UtcNow.AddMinutes(15);
        //     _sut.When(new MarkTicketsAsReserved { OrderId = OrderId, Expiration = expiration, Tickets = new List<TicketQuantity> { new TicketQuantity(TicketTypeId, 5, new OrderTicketDetails()) } });
        //
        //     var @event = _sut.ThenHasOne<OrderReservationCompleted>();
        //     Assert.Equal(OrderId, @event.SourceId);
        //     Assert.Equal(1, @event.Tickets.Count());
        //     Assert.Equal(5, @event.Tickets.ElementAt(0).Quantity);
        //     Assert.Equal(expiration, @event.ReservationExpiration);
        // }
        //
        // [Test]
        // public async Task when_marking_all_as_reserved_then_totals_are_not_recalculated()
        // {
        //     var expiration = DateTime.UtcNow.AddMinutes(15);
        //     _sut.When(new MarkTicketsAsReserved { OrderId = OrderId, Expiration = expiration, Tickets = new List<TicketQuantity> { new TicketQuantity(TicketTypeId, 5, new OrderTicketDetails()) } });
        //
        //     Assert.Equal(0, _sut.Events.OfType<OrderTotalsCalculated>().Count());
        // }
        //
        // [Test]
        // public async Task when_expiring_order_then_notifies()
        // {
        //     _sut.When(new RejectOrder { OrderId = OrderId });
        //
        //     var @event = _sut.ThenHasSingle<OrderExpired>();
        //     Assert.Equal(OrderId, @event.SourceId);
        // }
        //
        // [Test]
        // public async Task when_assigning_registrant_information_then_raises_integration_event()
        // {
        //     _sut.When(new AssignRegistrantDetails { OrderId = OrderId, FirstName = "foo", LastName = "bar", Email = "foo@bar.com" });
        //
        //     var @event = _sut.ThenHasSingle<OrderRegistrantAssigned>();
        //     Assert.Equal(OrderId, @event.SourceId);
        //     Assert.Equal("foo", @event.FirstName);
        //     Assert.Equal("bar", @event.LastName);
        //     Assert.Equal("foo@bar.com", @event.Email);
        // }
    }
}