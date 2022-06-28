using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PlanController : JsonApiController<Plan, Guid>
    {
        public PlanController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<Plan, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}