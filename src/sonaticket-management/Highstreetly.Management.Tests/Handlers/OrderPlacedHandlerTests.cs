using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Management.Contracts;
using Highstreetly.Management.Handlers;
using Highstreetly.Management.Resources;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Highstreetly.Management.Tests.Handlers
{
    public class OrderPlacedHandlerTests
    {
        [Test]
        public async Task Consume_OrderPlaced_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            const string ownerEmail = "test@test.com";

            var orderData = new List<Order>
            {
                new()
                {
                    Id = orderId,
                    OwnerEmail = ownerEmail,
                    EventInstanceId = eiId
                }
            }.AsQueryable();

            var mockOrderSet = orderData.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ManagementDbContext>("test");
            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);

            var provider = new ServiceCollection()
                .AddSingleton(_ => mockContext.Object)

                .AddLogging()
                .AddHttpClient()

                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<OrderPlacedHandler>();
                    cfg.AddConsumerTestHarness<OrderPlacedHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<IOrderPlaced>(new
                {
                    SourceId = orderId,
                    EventInstanceId = eiId,
                    // is this needed?
                    Tickets = new { },
                    OwnerId = ownerId,
                    HumanReadableId = RandomIdGenerator.GetBase36(5),
                    IsClickAndCollect = true,
                    IsLocalDelivery = false,
                    IsNationalDelivery = false
                });

                (await harness.Consumed.Any<IOrderPlaced>()).Should().BeTrue();

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<OrderPlacedHandler>>();

                (await consumerHarness.Consumed.Any<IOrderPlaced>()).Should().BeTrue();

                var order = mockContext.Object.Orders.First();

                order.Status.Should().Be(OrderStatus.Pending);
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