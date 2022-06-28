using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class UpdateAddOnHandler : IConsumer<IUpdateAddOn>
    {
        private readonly ILogger<UpdateAddOnHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public UpdateAddOnHandler(ILogger<UpdateAddOnHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IUpdateAddOn> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var incomingAddon = context.Message.AddOnUpdate.Content.Addon;

            var addOnUpdate = _managementDbContext
                .AddOns
                .First(x => x.IntegrationId == incomingAddon.Id);

            addOnUpdate.Name = incomingAddon.Name;
            addOnUpdate.Description = incomingAddon.Description;
            addOnUpdate.Price = incomingAddon.Price;
            addOnUpdate.Status = incomingAddon.Status;
            addOnUpdate.ChargeType = incomingAddon.ChargeType;
            addOnUpdate.CurrencyCode = incomingAddon.CurrencyCode;
            addOnUpdate.Period = incomingAddon.Period;
            addOnUpdate.PeriodUnit = incomingAddon.PeriodUnit;
            addOnUpdate.EnabledInPortal = incomingAddon.EnabledInPortal;
            addOnUpdate.ShowDescriptionInQuotes = incomingAddon.ShowDescriptionInQuotes;
            addOnUpdate.ShowDescriptionInInvoices = incomingAddon.ShowDescriptionInInvoices;

            await _managementDbContext.SaveChangesAsync();
        }
    }
}