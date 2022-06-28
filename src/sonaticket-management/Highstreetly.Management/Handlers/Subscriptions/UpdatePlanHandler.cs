using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargeBee.Api;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Infrastructure.Email;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class UpdatePlanHandler : IConsumer<IUpdatePlan>
    {
        private readonly ILogger<UpdatePlanHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;
        private readonly IEmailSender _emailSender;

        public UpdatePlanHandler(
            ILogger<UpdatePlanHandler> logger,
            ManagementDbContext managementDbContext,
            IConfiguration config, IEmailSender emailSender)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
            _emailSender = emailSender;
            ApiConfig.Configure(config["ChargeBeeSite"], config["ChargeBeeKey"]);
        }

        public async Task Consume(ConsumeContext<IUpdatePlan> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var planUpdated = await ChargeBee
                .Models
                .Plan
                .Retrieve(context.Message.PlanUpdate.Content.Plan.Id)
                .RequestAsync();   

            var existingPlanQuery =
                _managementDbContext
                    .Plans
                    .Include(x => x.PlanAddOns)
                    .ThenInclude(x => x.AddOn)
                    .Where(x => x.IntegrationId == planUpdated.Plan.Id);

            var incomingAddOnIds = new List<string>();

            if (planUpdated.Plan.EventBasedAddons !=null)
            {
                incomingAddOnIds.AddRange(planUpdated.Plan.EventBasedAddons.Select(x => x.Id()));
            }

            if (planUpdated.Plan.ApplicableAddons != null)
            {
                incomingAddOnIds.AddRange(planUpdated.Plan.ApplicableAddons.Select(x => x.Id()));
            }

            if (planUpdated.Plan.AttachedAddons != null)
            {
                incomingAddOnIds.AddRange(planUpdated.Plan.AttachedAddons.Select(x => x.Id()));
            }

            if (existingPlanQuery.Any())
            {
                // existing plan, can have addons added / removed.
                // we assume the addon exists in hs
                var existingPlan = existingPlanQuery.First();
                var incomingAddOns = incomingAddOnIds;
                
                var savedAddOns = existingPlan
                    .PlanAddOns
                    .Select(x => x.AddOn.IntegrationId);

                var deletedDiff = savedAddOns.Except(incomingAddOns);
                var addedDiff = incomingAddOns.Except(savedAddOns);

                var toDelete = existingPlan
                    .PlanAddOns
                    .Select(x => x.AddOn)
                    .Where(x => deletedDiff.Contains(x.IntegrationId))
                    .ToList();

                // Deleted addons
                foreach (var deleteAddOn in toDelete)
                {
                    _managementDbContext.AddOns.Remove(deleteAddOn);
                }

                await _managementDbContext.SaveChangesAsync();

                // Added addons
                var addOnsToCreate = incomingAddOnIds.Where(x => addedDiff.Contains(x)).ToList();

                var createdAddOns = addOnsToCreate
                    .Select(x => new PlanAddOn
                    {
                        Plan = existingPlan,
                        AddOn = _managementDbContext.AddOns.FirstOrDefault(ao => ao.IntegrationId == x)
                    });

                if (createdAddOns.Any(x=>x.AddOn == null))
                {
                    var addOnIds = addOnsToCreate;

                    var existingAddons = _managementDbContext.AddOns.Where(x => addOnIds.Contains(x.IntegrationId)).Select(x=>x.IntegrationId);

                    var missingAddons = addOnIds.Except(existingAddons);

                    await _emailSender.SendEmailAsync(
                        "us@highstreet.ly",
                        "FAULT: UpdatePlanHandler",
                        $"Missing addons when updating plan: {JsonConvert.SerializeObject(context.Message)} {string.Join(",", missingAddons)}");

                }

                await _managementDbContext.PlanAddOns.AddRangeAsync(createdAddOns);

                existingPlan.Name = planUpdated.Plan.Name;
                existingPlan.Description = planUpdated.Plan.Description;
                existingPlan.Price = planUpdated.Plan.Price;
                existingPlan.PricingModel = planUpdated.Plan.PricingModel.ToString();
                existingPlan.Status = planUpdated.Plan.Status.ToString();
                existingPlan.ChargeModel = planUpdated.Plan.ChargeModel.ToString();
                existingPlan.CurrencyCode = planUpdated.Plan.CurrencyCode;
                existingPlan.EnabledInHostedPages = planUpdated.Plan.EnabledInHostedPages;
                existingPlan.EnabledInPortal = planUpdated.Plan.EnabledInPortal;
                existingPlan.FreeQuantity = planUpdated.Plan.FreeQuantity;
                existingPlan.Period = planUpdated.Plan.Period;
                existingPlan.PeriodUnit = planUpdated.Plan.PeriodUnit.ToString();
                existingPlan.Taxable = planUpdated.Plan.Taxable;

                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}