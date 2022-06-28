using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Configuration;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Stripe;
using Highstreetly.Management.Handlers;
using Highstreetly.Management.Resources;
using JustEat.HttpClientInterception;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Highstreetly.Management.Tests.Handlers
{
    public class LinkEventOrganiserAccountToStripeHandlerTests
    {
        public HttpClientInterceptorOptions Interceptor { get; set; }

        [SetUp]
        public void Setup()
        {
            Interceptor = new HttpClientInterceptorOptions()
                .ThrowsOnMissingRegistration();
        }

        [Test]
        public async Task Consume_LinkEventOrganiserAccountToStripe_HappyPath()
        {
            var id = Guid.NewGuid();

            new HttpRequestInterceptionBuilder()
                .ForHost("connect.stripe.com")
                .ForPath("/oauth/token")
                .ForMethod(HttpMethod.Post)
                .ForScheme("https")
                .WithJsonContent(new StripeResponse
                {
                    access_token = "123.123.123",
                    stripe_user_id = "1",
                    stripe_publishable_key = "xxxxx"
                })
                .RegisterWith(Interceptor);


            var data = new List<EventOrganiser>
            {
                new()
                {
                    Id = id,
                    Name = "test"
                }
            }.AsQueryable();

            var mockSet = data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ManagementDbContext>("test");
            mockContext.Setup(m => m.EventOrganisers).Returns(mockSet.Object);

            var provider = new ServiceCollection()
                .AddSingleton(_ => mockContext.Object)
                .AddLogging()
                .AddHttpClient()
                .AddSingleton<IHttpMessageHandlerBuilderFilter, HttpClientInterceptionFilter>(
                    (_) => new HttpClientInterceptionFilter(Interceptor))
                .AddScoped(_ => new StripeConfiguration
                {
                    ApiKey = "xxxxx"
                })
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<LinkEventOrganiserAccountToStripeHandler>();
                    cfg.AddConsumerTestHarness<LinkEventOrganiserAccountToStripeHandler>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var client = provider.GetRequiredService<IBus>();

                await client.Publish<ILinkEventOrganiserAccountToStripe>(new
                {

                    Code = default(string),
                    EventOrganiserId = id,
                    Scope = default(string),
                    Id = default(Guid),
                    Delay = default(TimeSpan),
                    TypeInfo = default(string),
                    CorrelationId = default(Guid)
                });

                Assert.That(await harness.Consumed.Any<ILinkEventOrganiserAccountToStripe>());

                var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<LinkEventOrganiserAccountToStripeHandler>>();

                Assert.That(await harness.Published.Any<IEventOrganiserAccountLinkedToStripe>());

                Assert.That(await consumerHarness.Consumed.Any<ILinkEventOrganiserAccountToStripe>());

                var eventOrganiser = mockContext.Object.EventOrganisers.First();
                eventOrganiser.StripePublishableKey.Should().NotBeEmpty();
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}
