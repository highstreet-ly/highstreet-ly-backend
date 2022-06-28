using System.Threading.Tasks;
using ChargeBee.Api;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Infrastructure.Email;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    /// <summary>
    /// TODO: this seems to be firing on each retry attempt
    /// it needs to only fire when  the retries have been exhausted
    /// </summary>
    public class CreateUserSubscriptionFaultConsumer :
        IConsumer<Fault<ICreateUserSubscription>>
    {
        private readonly ILogger<CreateUserSubscriptionFaultConsumer> _logger;
        private readonly IEmailSender _emailSender;

        public CreateUserSubscriptionFaultConsumer(
            IEmailSender emailSender,
            IConfiguration config,
            ILogger<CreateUserSubscriptionFaultConsumer> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
            ApiConfig.Configure(config["ChargeBeeSite"], config["ChargeBeeKey"]);
        }

        public async Task Consume(ConsumeContext<Fault<ICreateUserSubscription>> context)
        {
            // compensating action
            _logger.LogInformation($"We don't have a user with this email - notify me so that i can cancel the subscription");

            // await ChargeBee.Models.Subscription
            //     .Cancel(context.Message.Message.SubscriptionCreate.Content.Subscription.Id)
            //     .CreditOptionForCurrentTermCharges(CreditOptionForCurrentTermChargesEnum.Prorate)
            //     .EndOfTerm(false)
            //     .RequestAsync();

            await _emailSender.SendEmailAsync(
                "us@highstreet.ly",
                "FAULT: CreateUserSubscriptionFaultConsumer",
                $"There was an issue creating a user. Check the RMQ logs: {JsonConvert.SerializeObject(context.Message.Message)} {JsonConvert.SerializeObject(context.Message.Exceptions)}");
        }
    }
}