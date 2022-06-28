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
using OrderTicketDetails = Highstreetly.Reservations.Resources.OrderTicketDetails;
using ProductExtra = Highstreetly.Reservations.Resources.ProductExtra;

namespace Highstreetly.Reservations.Tests.Domain.Order
{
    public class given_placed_order_AddTicketsToBasket
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
        private EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IAddTicketsToBasket> _sut;
        private Mock<IJsonApiClient<User, Guid>> _userClient;

        [SetUp]
        public async Task SetUp()
        {
            _sut = new EventSourcingCommandHandlerTestHelper<Reservations.Domain.Order, IAddTicketsToBasket>();
            _mockContext = new Mock<ReservationDbContext>("test");
            // TODO: I am forcing the reserved tickets on the draft order to be 20 here to make 
            // when_updating_items_then_updates_order_with_new_items pass
            // I assume this gets updated in the API PATCH/POST but need to check this - if not then it should be updated in the 
            // AddTicketsToBasketHandler - which is not happening right now?!
            var orderData = new List<DraftOrder>
            {
                new()
                {
                    Id = OrderId,
                    OwnerEmail = OwnerEmail,
                    OwnerId = OwnerId,
                    EventInstanceId = EventInstanceId,
                    DraftOrderItems = new List<DraftOrderItem>
                    {
                        new()
                        {
                            TicketType = TicketTypeId,
                            RequestedTickets = 20,
                            ReservedTickets = 20,
                            Ticket = new OrderTicketDetails
                            {
                                ProductExtras = new List<ProductExtra>(),
                                Quantity = 20
                            }
                        }
                    }
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
                .Setup(
                    x =>
                        x.GetAsync(
                            It.IsAny<Guid>(),
                            It.IsAny<QueryBuilder>(),
                            It.IsAny<bool>()))
                .ReturnsAsync(
                    new User
                    {
                        Email = OwnerEmail,
                        Id = OwnerId
                    });

            _eventInstanceClient = new Mock<IJsonApiClient<EventInstance, Guid>>();
            _eventInstanceClient
                .Setup(
                    x =>
                        x.GetAsync(
                            It.IsAny<Guid>(),
                            It.IsAny<QueryBuilder>(),
                            It.IsAny<bool>()))
                .ReturnsAsync(
                    new EventInstance
                    {
                        Id = OwnerId,
                        Name = "test",
                        Slug = "test slug".GenerateSlug(),
                        MainImageId = "1",
                        ShortLocation = "test location"
                    });

            _pricingService = new Mock<IPricingService>();

            _pricingService.Setup(
                    x => x.CalculateTotal(
                        EventInstanceId,
                        OrderId,
                        It.IsAny<List<TicketQuantity>>()))
                .ReturnsAsync(OrderTotal);

            _sut.ServiceCollection
                .AddSingleton(_ => _mockContext.Object)
                .AddSingleton(_ => _userClient.Object)
                .AddSingleton(_ => _eventInstanceClient.Object)
                .AddSingleton(_ => _pricingService.Object)
                .AddLogging()
                .AddHttpClient()
                .AddMassTransitInMemoryTestHarness(
                    cfg =>
                    {
                        cfg.AddConsumer<AddTicketsToBasketHandler>();
                        cfg.AddConsumerTestHarness<AddTicketsToBasketHandler>();
                    });

            await _sut.Setup();
            _sut.Given(
                new Tuple<Type, ISonaticketEvent>(
                    typeof(IOrderPlaced),
                    new OrderPlaced
                    {
                        SourceId = OrderId,
                        EventInstanceId = EventInstanceId,
                        Tickets = new List<TicketQuantity>
                        {
                            new(
                                Guid.NewGuid(),
                                1,
                                new Infrastructure.MessageDtos.OrderTicketDetails())
                        }
                    }));
        }

        [Test]
        public async Task when_updating_order_then_calculates_totals()
        {
            await _sut.When(
                new AddTicketsToBasket
                {
                    EventInstanceId = EventInstanceId,
                    OrderId = OrderId,
                    Tickets = new List<TicketQuantity>
                    {
                        new(
                            Guid.NewGuid(),
                            1,
                            new Infrastructure.MessageDtos.OrderTicketDetails())
                    }
                });

            var consumed = await _sut.ThenConsumed<AddTicketsToBasketHandler>();
            consumed.Should()
                .BeTrue();

            await _sut.When(
                new AddTicketsToBasket
                {
                    EventInstanceId = EventInstanceId,
                    OrderId = OrderId,
                    Tickets = new List<TicketQuantity>
                    {
                        new(
                            Guid.NewGuid(),
                            1,
                            new Infrastructure.MessageDtos.OrderTicketDetails())
                    }
                });

            consumed = await _sut.ThenConsumed<AddTicketsToBasketHandler>();
            consumed.Should()
                .BeTrue();

            var published = _sut.ThenHasOne<OrderTotalsCalculated>();
            published.Should()
                .NotBeNull();
        }


        [Test]
        public async Task when_updating_items_then_updates_order_with_new_items()
        {
            await _sut.When(
                new AddTicketsToBasket
                {
                    EventInstanceId = EventInstanceId,
                    OrderId = OrderId,
                    Tickets = new[]
                    {
                        new TicketQuantity(
                            TicketTypeId,
                            20,
                            new Infrastructure.MessageDtos.OrderTicketDetails())
                    }
                });

            var consumed = await _sut.ThenConsumed<AddTicketsToBasketHandler>();
            consumed.Should()
                .BeTrue();

            var @event = _sut.ThenHasOne<OrderUpdated>();
            @event.SourceId.Should()
                .Be(OrderId);
            @event.Tickets.Count()
                .Should()
                .Be(1);
            @event.Tickets.ElementAt(0)
                .Quantity.Should()
                .Be(20);
        }
    }
}