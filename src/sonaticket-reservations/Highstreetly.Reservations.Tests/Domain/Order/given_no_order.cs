using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class given_no_order
    {
        private EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IAddTicketsToBasket> _sut;
        private static readonly Guid OrderId = Guid.NewGuid();
        private static readonly Guid EventInstanceId = Guid.NewGuid();
        private static readonly Guid OwnerId = Guid.NewGuid();
        const string OwnerEmail = "test@test.com";

        private Mock<ReservationDbContext> _mockContext;
        private Mock<IJsonApiClient<User, Guid>> _userClient;
        private Mock<IJsonApiClient<EventInstance, Guid>> _eventInstanceClient;
        private Mock<IPricingService> _pricingService;

        [SetUp]
        public void SetUp()
        {
            this._sut = new EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IAddTicketsToBasket>();
            _mockContext = new Mock<ReservationDbContext>("test");
            var orderData = new List<DraftOrder>
            {
                new()
                {
                    Id = OrderId,
                    OwnerEmail = OwnerEmail,
                    OwnerId =  OwnerId,
                    EventInstanceId = EventInstanceId
                }
            }.AsQueryable();

            var mockOrderSet = orderData.AsQueryable().BuildMockDbSet();
            var mockPricedOrderSet = new List<PricedOrder>().AsQueryable().BuildMockDbSet();

            _mockContext.Setup(m => m.DraftOrders).Returns(mockOrderSet.Object);
            _mockContext.Setup(m => m.PricedOrders).Returns(mockPricedOrderSet.Object);

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
                .ReturnsAsync(new EventInstance()
                {
                    Id = OwnerId,
                    Name = "test",
                    Slug = "test slug".GenerateSlug(),
                    MainImageId = "1",
                    ShortLocation = "test location"
                });

            _pricingService = new Mock<IPricingService>();

            _sut.ServiceCollection
                .AddSingleton(_ => _mockContext.Object)
                .AddSingleton(_ => _userClient.Object)
                .AddSingleton(_ => _eventInstanceClient.Object)
                .AddSingleton(_ => _pricingService.Object)
                .AddLogging()
                .AddHttpClient()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<AddTicketsToBasketHandler>();
                    cfg.AddConsumerTestHarness<AddTicketsToBasketHandler>();
                });

            _sut.Setup().GetAwaiter().GetResult();
        }

        [Test]
        public async Task when_creating_order_then_is_placed_with_specified_id()
        {
            await _sut.When(new AddTicketsToBasket()
            {
                EventInstanceId = EventInstanceId,
                OrderId = OrderId,
                Tickets = new List<TicketQuantity>
                {
                    new(Guid.NewGuid(), 1, new OrderTicketDetails())
                }
            });

            var consumed = await _sut.ThenConsumed<AddTicketsToBasketHandler>();
            consumed.Should().BeTrue();

            _sut.ThenHasSingle<OrderPlaced>().SourceId.Should().Be(OrderId);
        }

        [Test]
        public async Task when_placing_order_then_defines_expected_expiration_time_in_15_minutes()
        {
            await _sut.When(new AddTicketsToBasket()
            {
                EventInstanceId = EventInstanceId,
                OrderId = OrderId,
                Tickets = new List<TicketQuantity>
                {
                    new(Guid.NewGuid(), 1, new OrderTicketDetails())
                }
            });

            var consumed = await _sut.ThenConsumed<AddTicketsToBasketHandler>();
            consumed.Should().BeTrue();

            var evt = _sut.ThenHasSingle<OrderPlaced>();
            var expire = evt.ReservationAutoExpiration.Subtract(DateTime.UtcNow);
            expire.Minutes.Should().Be(59);
        }

        // [Fact]
        // public void when_creating_order_then_calculates_totals()
        // {
        //     this.sut.When(new RegisterToConference { ConferenceId = ConferenceId, OrderId = OrderId, Tickets = new[] { new SeatQuantity(SeatTypeId, 5) } });
        //
        //     var totals = sut.ThenHasOne<OrderTotalsCalculated>();
        //     Assert.Equal(OrderTotal.Total, totals.Total);
        //     Assert.Equal(OrderTotal.Lines.Count, totals.Lines.Length);
        //     Assert.Equal(OrderTotal.Lines.First().LineTotal, totals.Lines[0].LineTotal);
        // }
    }
}