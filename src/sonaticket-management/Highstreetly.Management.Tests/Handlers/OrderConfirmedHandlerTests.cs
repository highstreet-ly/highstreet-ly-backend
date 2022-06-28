using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.JsonApiClient;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder;
using Highstreetly.Management.Contracts;
using Highstreetly.Management.Handlers;
using Highstreetly.Management.Resources;
using Highstreetly.Reservations.Contracts.Requests;
using JsonApiSerializer.JsonApi;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Highstreetly.Management.Tests.Handlers
{
    public class OrderConfirmedHandlerTests
    {
        [Test]
        public async Task Consume_OrderConfirmed_HappyPath()
        {
            var orderId = Guid.NewGuid();
            var eiId = Guid.NewGuid();

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

            var orderClient = new Mock<IJsonApiClient<PricedOrder, Guid>>();

            orderClient.Setup(x => x.GetListAsync(It.IsAny<QueryBuilder>(), false))
                .ReturnsAsync(new List<PricedOrder>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        PricedOrderLines = new List<PricedOrderLine>()
                    }
                });

            var mockEmailSender = new Mock<IEmailSender>();

            mockEmailSender.Setup(x => x.SendEmailAsync(ownerEmail, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>(), It.IsAny<byte[]>()));
            mockEmailSender.Setup(x => x.SendEmailAsync(notificationEmail, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>(), It.IsAny<byte[]>()));

            var provider = new ServiceCollection()
                .AddSingleton(_ => mockContext.Object)
                .AddSingleton(_ => orderClient.Object)
                .AddScoped(_ => mockEmailSender.Object)
                .AddLogging()
                .AddHttpClient()
                .AddScoped(_ => new EmailTemplateOptions
                {
                    OrderInTheBag = "asd",
                    OrderInTheBagOperator = "asd3"
                })
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<OrderConfirmedHandler>();
                    cfg.AddConsumerTestHarness<OrderConfirmedHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<IOrderConfirmed>(new
                {

                    Email = ownerEmail,
                    SourceId = orderId,
                    Delay = default(TimeSpan),
                    Version = default(int),
                    CorrelationId = default(Guid)
                });

                (await harness.Consumed.Any<IOrderConfirmed>()).Should().BeTrue();

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<OrderConfirmedHandler>>();

                (await consumerHarness.Consumed.Any<IOrderConfirmed>()).Should().BeTrue();

                // confirm that SendOrderConfirmedAsync was called with correct params
                // confirm that SendOrderConfirmedOperatorAsync was called with correct params
                mockEmailSender.VerifyAll();

                var order = mockContext.Object.Orders.First();

                order.Status.Should().Be(OrderStatus.Paid);
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