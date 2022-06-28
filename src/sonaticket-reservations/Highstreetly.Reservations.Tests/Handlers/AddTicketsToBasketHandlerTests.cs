using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.EventSourcing;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Reservations.Domain;
using Highstreetly.Reservations.Handlers;
using Highstreetly.Reservations.Resources;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Highstreetly.Reservations.Tests
{
    public class AddTicketsToBasketHandlerTests
    {

        [Test]
        public async Task Consume_AddTicketsToBasket_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();

            const string ownerEmail = "test@test.com";

            var orderData = new List<DraftOrder>
            {
                new()
                {
                    Id = orderId,
                    OwnerEmail = ownerEmail,
                    EventInstanceId = eiId
                }
            }.AsQueryable();

            var mockOrderSet = orderData.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ReservationDbContext>("test");
            mockContext.Setup(m => m.DraftOrders).Returns(mockOrderSet.Object);

            var repo = new Mock<IEventSourcedRepository<Order>>();
            var pricingService = new Mock<IPricingService>();

            var provider = new ServiceCollection()
                .AddSingleton(_ => mockContext.Object)
                .AddSingleton(_ => repo.Object)
                .AddSingleton(_ => pricingService.Object)
                .AddLogging()
                .AddHttpClient()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<AddTicketsToBasketHandler>();
                    cfg.AddConsumerTestHarness<AddTicketsToBasketHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<IAddTicketsToBasket>(new
                {

                    OrderId = default(Guid),
                    EventInstanceId = default(Guid),
                    Slug = default(string),
                    EventInstanceName = default(string),
                    Tickets = new[] { default(TicketQuantity) },
                    OrderVersion = default(int),
                    OwnerId = default(Guid?),
                    OwnerEmail = default(string),
                    HumanReadableId = default(string),
                    Id = default(Guid),
                    Delay = default(TimeSpan),
                    TypeInfo = default(string),
                    CorrelationId = default(Guid)
                });

                (await harness.Consumed.Any<IAddTicketsToBasket>()).Should().BeTrue();

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<AddTicketsToBasketHandler>>();

                (await consumerHarness.Consumed.Any<IAddTicketsToBasket>()).Should().BeTrue();

                var order = mockContext.Object.DraftOrders.First();

                order.OwnerEmail.Should().Be(ownerEmail);
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}