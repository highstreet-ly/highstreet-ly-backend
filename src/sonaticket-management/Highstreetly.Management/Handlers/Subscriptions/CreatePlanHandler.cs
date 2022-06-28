using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreatePlanHandler : IConsumer<ICreatePlan>
    {
        private readonly ILogger<CreatePlanHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public CreatePlanHandler(ILogger<CreatePlanHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<ICreatePlan> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");


            var incomingPlan = context.Message.PlanCreate.Content.Plan;

            if (_managementDbContext.Plans.Any(x=>x.IntegrationId == incomingPlan.Id))
            {
                _logger.LogInformation($"Plan already exists - skipping. {incomingPlan.Id}:{incomingPlan.Name}");
                return;
            }

            var createdPlan = new Plan
            {
                Description = incomingPlan.Description,
                Name = incomingPlan.Name,
                IntegrationId = incomingPlan.Id,
                Price = incomingPlan.Price,
                PricingModel = incomingPlan.PricingModel,
                Status = incomingPlan.Status,
                ChargeModel = incomingPlan.ChargeModel,
                CurrencyCode = incomingPlan.CurrencyCode,
                EnabledInHostedPages = incomingPlan.EnabledInHostedPages,
                EnabledInPortal = incomingPlan.EnabledInPortal,
                FreeQuantity = incomingPlan.FreeQuantity,
                Period = incomingPlan.Period,
                PeriodUnit = incomingPlan.PeriodUnit,
                ShowDescriptionInInvoices = incomingPlan.ShowDescriptionInInvoices,
                ShowDescriptionInQuotes = incomingPlan.ShowDescriptionInQuotes,
                Taxable = incomingPlan.Taxable,
            };

            await _managementDbContext.Plans.AddAsync(createdPlan);

            await _managementDbContext.SaveChangesAsync();
        }
    }
}