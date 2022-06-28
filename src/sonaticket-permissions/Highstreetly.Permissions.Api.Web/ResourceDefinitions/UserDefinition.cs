using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChargeBee.Api;
using ChargeBee.Models.Enums;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Queries.Expressions;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Web.ResourceDefinitions
{
    public class UserDefinition : JsonApiResourceDefinition<User, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDefinition(
            IResourceGraph resourceGraph,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            ILogger<UserDefinition> logger)
            : base(resourceGraph)
        {
            _httpContextAccessor = httpContextAccessor;
            ApiConfig.Configure(config["ChargeBeeSite"], config["ChargeBeeKey"]);
        }

        public override async Task OnWriteSucceededAsync(User resource, OperationKind operationKind, CancellationToken cancellationToken)
        {
            await base.OnWriteSucceededAsync(resource, operationKind, cancellationToken);
            await ProcessCommands(resource);
        }

        public async Task ProcessCommands(User resource)
        {
            var commands = _httpContextAccessor.HttpContext.Request.Headers["Command-Type"].ToList();
            foreach (var command in commands)
            {
                switch (command)
                {
                    case "SetDefaultUserSubscription":
                        // create the charge bee sub:
                        var now = ApiUtil.ConvertToTimestamp(DateTime.UtcNow);
                        await ChargeBee.Models.Subscription.Create()
                             .PlanId("starter-plan")
                             .StartDate(now ?? default)
                             .AutoCollection(AutoCollectionEnum.Off)
                             .TrialEnd(0)
                             .CustomerFirstName(resource.Email)
                             .CustomerEmail(resource.Email)
                             .RequestAsync();
                        break;
                }
            }
        }
    }
}
