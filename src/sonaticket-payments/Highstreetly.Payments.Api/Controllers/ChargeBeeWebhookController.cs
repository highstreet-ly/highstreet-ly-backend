using System.Text.Json;
using System.Threading.Tasks;
using Baseline;
using Highstreetly.Infrastructure.ChargeBee;
using Highstreetly.Infrastructure.ChargeBee.AddOnCreated;
using Highstreetly.Infrastructure.ChargeBee.AddOnDeleted;
using Highstreetly.Infrastructure.ChargeBee.AddOnUpdated;
using Highstreetly.Infrastructure.ChargeBee.PlanCreated;
using Highstreetly.Infrastructure.ChargeBee.PlanDeleted;
using Highstreetly.Infrastructure.ChargeBee.PlanUpdated;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionCancelled;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionCreated;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionDeleted;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionPaused;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionReactivated;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionRenewed;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionResumed;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionStarted;
using Highstreetly.Infrastructure.ChargeBee.SubscriptionTrialEndReminder;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Controllers
{
    [Route("api/v1/chargebee/webhook")]
    [ApiController]
    public class ChargeBeeWebhookController : Controller
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IBusClient _busClient;

        public ChargeBeeWebhookController(
            ILogger<WebhookController> logger,
            IBusClient busClient)
        {
            _logger = logger;
            _busClient = busClient;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            _logger.LogInformation("POSTING into chargebee webhook");
            var json = await HttpContext.Request.Body.ReadAllTextAsync();

            var submission = JsonSerializer.Deserialize<ChargeBeeEvent>(json);

            _logger.LogInformation($"submission.EventType: {submission.EventType}");

            switch (submission?.EventType)
            {
                case "subscription_created":
                    var subCreated = JsonSerializer.Deserialize<SubscriptionCreate>(json);
                    await _busClient.Send<ICreateUserSubscription>(new CreateUserSubscription()
                    {
                        SubscriptionCreate = subCreated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });

                    break;
                case "subscription_started":
                    var subStarted = JsonSerializer.Deserialize<SubscriptionStart>(json);
                    await _busClient.Send<ICreateUserSubscription>(new StartUserSubscription()
                    {
                        SubscriptionStart = subStarted,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_cancelled":
                    var subCancelled = JsonSerializer.Deserialize<SubscriptionCancel>(json);
                    await _busClient.Send<ICancelUserSubscription>(new CancelUserSubscription()
                    {
                        SubscriptionCancel = subCancelled,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_reactivated":
                    var subReactivated = JsonSerializer.Deserialize<SubscriptionReactivate>(json);
                    await _busClient.Send<IReactivateUserSubscription>(new ReactivateUserSubscription()
                    {
                        SubscriptionReactivate = subReactivated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_renewed":
                    var subRenewed = JsonSerializer.Deserialize<SubscriptionRenew>(json);
                    await _busClient.Send<IRenewedUserSubscription>(new RenewedUserSubscription()
                    {
                        SubscriptionRenew = subRenewed,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_deleted":
                    var subDeleted = JsonSerializer.Deserialize<SubscriptionDelete>(json);
                    await _busClient.Send<IDeleteUserSubscription>(new DeleteUserSubscription()
                    {
                        SubscriptionDelete = subDeleted,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_paused":
                    var subPaused = JsonSerializer.Deserialize<SubscriptionPause>(json);
                    await _busClient.Send<IPauseUserSubscription>(new PauseUserSubscription()
                    {
                        SubscriptionPause = subPaused,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_resumed":
                    var subResumed = JsonSerializer.Deserialize<SubscriptionResume>(json);
                    await _busClient.Send<IResumeUserSubscription>(new ResumeUserSubscription()
                    {
                        SubscriptionResume = subResumed,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "subscription_trial_end_reminder":
                    var subTrialEndReminder = JsonSerializer.Deserialize<SubscriptionTrialEndReminder>(json);
                    await _busClient.Send<ITrialEndReminderUserSubscription>(new TrialEndReminderUserSubscription()
                    {
                        SubscriptionTrialEndReminder = subTrialEndReminder,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "addon_created":
                    var addOnCreated = JsonSerializer.Deserialize<AddOnCreate>(json);
                    await _busClient.Send<ICreateAddOn>(new CreateAddOn()
                    {
                        AddOnCreate = addOnCreated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "addon_updated":
                    var addOnUpdated = JsonSerializer.Deserialize<AddOnUpdate>(json);
                    await _busClient.Send<IUpdateAddOn>(new UpdateAddOn()
                    {
                        AddOnUpdate = addOnUpdated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "addon_deleted":
                    var addOnDeleted = JsonSerializer.Deserialize<AddOnDelete>(json);
                    await _busClient.Send<IDeleteAddOn>(new DeleteAddOn()
                    {
                        AddOnDelete = addOnDeleted,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "plan_created":
                    var planCreated = JsonSerializer.Deserialize<PlanCreate>(json);
                    await _busClient.Send<ICreatePlan>(new CreatePlan()
                    {
                        PlanCreate = planCreated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "plan_updated":
                    var planUpdated = JsonSerializer.Deserialize<PlanUpdate>(json);
                    await _busClient.Send<IUpdatePlan>(new UpdatePlan()
                    {
                        PlanUpdate = planUpdated,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
                case "plan_deleted":
                    var planDeleted = JsonSerializer.Deserialize<PlanDelete>(json);
                    await _busClient.Send<IDeletePlan>(new DeletePlan()
                    {
                        PlanDelete = planDeleted,
                        Id = NewId.NextGuid(),
                        CorrelationId = NewId.NextGuid()
                    });
                    break;
            }

            return new EmptyResult();
        }
    }
}