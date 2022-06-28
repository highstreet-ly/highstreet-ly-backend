using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
    public class OrderTotalsCalculatedHandlerTests
    {
        [Test]
        public async Task Consume_OrderTotalsCalculated_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();
            const int expectedTotal = 9;

            const string ownerEmail = "test@test.com";
            const string notificationEmail = "event@owner.com";

            var orderData = new List<Order>
            {
                new()
                {
                    Id = orderId,
                    OwnerEmail = ownerEmail,
                    EventInstanceId = eiId
                }
            }.AsQueryable();

            var eiData = new List<EventInstance>
            {
                new()
                {
                    Id = eiId,
                    NotificationEmail = notificationEmail
                }
            }.AsQueryable();

            var mockOrderSet = orderData.AsQueryable().BuildMockDbSet();
            var mockEiSet = eiData.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ManagementDbContext>("test");
            mockContext.Setup(m => m.Orders).Returns(mockOrderSet.Object);
            mockContext.Setup(m => m.EventInstances).Returns(mockEiSet.Object);

            var provider = new ServiceCollection()
                .AddSingleton(_ => mockContext.Object)
                .AddLogging()
                .AddHttpClient()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<OrderTotalsCalculatedHandler>();
                    cfg.AddConsumerTestHarness<OrderTotalsCalculatedHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<IOrderTotalsCalculated>(new
                {
                    Total = expectedTotal,
                    SourceId = orderId,
                });

                (await harness.Consumed.Any<IOrderTotalsCalculated>()).Should().BeTrue();

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<OrderTotalsCalculatedHandler>>();

                (await consumerHarness.Consumed.Any<IOrderTotalsCalculated>()).Should().BeTrue();

                var order = mockContext.Object.Orders.First();

                order.Status.Should().Be(OrderStatus.Pending);
                order.TotalAmount.Should().Be(expectedTotal);
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}