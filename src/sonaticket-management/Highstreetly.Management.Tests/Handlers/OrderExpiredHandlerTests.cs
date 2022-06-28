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

    public class OrderExpiredHandlerTests
    {
        [Test]
        public async Task Consume_OrderExpired_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();

            var orderData = new List<Order>
            {
                new()
                {
                    Id = orderId,
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
                    cfg.AddConsumer<OrderExpiredHandler>();
                    cfg.AddConsumerTestHarness<OrderExpiredHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();
                
                await client.Publish<IOrderExpired>(new
                {
                    SourceId = orderId,
                    Delay = default(TimeSpan),
                    Version = default(int),
                    CorrelationId = default(Guid)
                });
                
                (await harness.Consumed.Any<IOrderExpired>()).Should().BeTrue();
                
                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<OrderExpiredHandler>>();
                
                (await consumerHarness.Consumed.Any<IOrderExpired>()).Should().BeTrue();
              
                var order = mockContext.Object.Orders.First();
                
                order.Status.Should().Be(OrderStatus.Expired);
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}
