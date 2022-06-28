using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StripeConfiguration = Highstreetly.Infrastructure.Configuration.StripeConfiguration;
using StripeResponse = Highstreetly.Infrastructure.Stripe.StripeResponse;

namespace Highstreetly.Management.Handlers
{
    public class LinkEventOrganiserAccountToStripeHandler :
        IConsumer<ILinkEventOrganiserAccountToStripe>
    {
        private readonly ManagementDbContext _managementDbContext;
        private readonly ILogger<LinkEventOrganiserAccountToStripeHandler> _logger;
        private readonly StripeConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public LinkEventOrganiserAccountToStripeHandler(
            ILoggerFactory loggerFactory, 
            ManagementDbContext managementDbContext,
            StripeConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _managementDbContext = managementDbContext;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<LinkEventOrganiserAccountToStripeHandler>();
        }

        public async Task Consume(
            ConsumeContext<ILinkEventOrganiserAccountToStripe> @event)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {["CorrelationId"] = @event.CorrelationId, ["SourceId"] = @event.Message.Id}))
            {
                try
                {

                    var client = _httpClientFactory.CreateClient("stripe-oauth");

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var payload = new
                    {
                        client_secret = _configuration.ApiKey,
                        code = @event.Message.Code,
                        grant_type = "authorization_code"
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                        "application/json");
                    var result = await client.PostAsync("https://connect.stripe.com/oauth/token", content);

                    var reason =
                        JsonConvert.DeserializeObject<StripeResponse>(await result.Content.ReadAsStringAsync());

                    if (string.IsNullOrEmpty(reason?.error))
                    {
                        var organiser =
                            _managementDbContext.EventOrganisers.First(x => x.Id == @event.Message.EventOrganiserId);
                        organiser.StripeAccessToken = reason?.access_token;
                        organiser.StripeAccountId = reason?.stripe_user_id;
                        organiser.StripePublishableKey = reason?.stripe_publishable_key;

                        await _managementDbContext.SaveChangesAsync();

                        await @event.Publish<IEventOrganiserAccountLinkedToStripe>(new
                        {
                            SourceId = @event.Message.EventOrganiserId,
                            @event.Message.CorrelationId
                        });
                    }
                    else
                    {
                        _logger.LogError($"{reason?.error_description}");
                        throw new InvalidOperationException(reason?.error_description);

                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e}");
                    throw;
                }
            }
        }
    }
}
