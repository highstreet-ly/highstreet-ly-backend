using System.Linq;
using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands.Subscriptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Handlers.Subscriptions
{
    public class DeletePlanHandler : IConsumer<IDeletePlan>
    {
        private readonly ILogger<DeletePlanHandler> _logger;
        private readonly ManagementDbContext _managementDbContext;

        public DeletePlanHandler(ILogger<DeletePlanHandler> logger, ManagementDbContext managementDbContext)
        {
            _logger = logger;
            _managementDbContext = managementDbContext;
        }

        public async Task Consume(ConsumeContext<IDeletePlan> context)
        {
            _logger.LogInformation($"Starting Consume {context.Message.GetType().Name}");

            var toDelete = _managementDbContext.Plans.Where(x => x.IntegrationId == context.Message.PlanDelete.Content.Plan.Id);

            if (toDelete.Any())
            {
                var d = toDelete.First();
                d.Deleted = true;
                await _managementDbContext.SaveChangesAsync();
            }
        }
    }
}