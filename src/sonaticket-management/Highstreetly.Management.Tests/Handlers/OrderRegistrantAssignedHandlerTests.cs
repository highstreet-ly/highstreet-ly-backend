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
    public class OrderRegistrantAssignedHandlerTests
    {
        [Test]
        public async Task Consume_OrderRegistrantAssigned_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

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
                    cfg.AddConsumer<OrderRegistrantAssignedHandler>();
                    cfg.AddConsumerTestHarness<OrderRegistrantAssignedHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<IOrderRegistrantAssigned>(new
                {
                    OwnerName = ownerEmail,
                    Email = ownerEmail,
                    UserId = ownerId,
                    Phone = "11111111",
                    DeliveryLine1 = "123",
                    DeliveryPostcode = "456",
                    SourceId = orderId,
                });

                (await harness.Consumed.Any<IOrderRegistrantAssigned>()).Should().BeTrue();

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<OrderRegistrantAssignedHandler>>();

                (await consumerHarness.Consumed.Any<IOrderRegistrantAssigned>()).Should().BeTrue();

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