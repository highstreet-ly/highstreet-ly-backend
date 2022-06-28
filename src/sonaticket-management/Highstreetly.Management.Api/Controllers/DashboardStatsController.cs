using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpReadOnly]
    public class DashboardStatsController : JsonApiController<DashboardStat, Guid>
    {
        public DashboardStatsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<DashboardStat, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}