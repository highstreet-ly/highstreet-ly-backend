using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using Highstreetly.Management.Resources;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class CreateAddOnHandler : IConsumer<ICreateAddOn>
    {
        private readonly ILogger<CreateAddOnHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public CreateAddOnHandler(ILogger<CreateAddOnHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<ICreateAddOn> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var incomingAddon = context.Message.AddOnCreate.Content.Addon;

            if (_managementDbContext.AddOns.Any(x => x.IntegrationId == incomingAddon.Id))
            {
                _logger.LogInformation($"Add-on already exists - skipping. {incomingAddon.Id}:{incomingAddon.Name}");
                return;
            }

            await _managementDbContext.AddOns.AddAsync(new AddOn
            {
                IntegrationId = incomingAddon.Id,
                Name = incomingAddon.Name,
                Price = incomingAddon.Price,
                PricingModel = incomingAddon.PricingModel,
                Status = incomingAddon.Status,
                ChargeType = incomingAddon.ChargeType,
                CurrencyCode = incomingAddon.CurrencyCode,
                Period = incomingAddon.Period,
                PeriodUnit = incomingAddon.PeriodUnit,
                EnabledInPortal = incomingAddon.EnabledInPortal,
                IsShippable = incomingAddon.IsShippable,
                ShowDescriptionInInvoices = incomingAddon.ShowDescriptionInInvoices,
                ShowDescriptionInQuotes = incomingAddon.ShowDescriptionInQuotes,
            });

            await _managementDbContext.SaveChangesAsync();
        }
    }
}
