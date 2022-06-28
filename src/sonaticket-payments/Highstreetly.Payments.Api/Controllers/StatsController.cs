using System;
using Highstreetly.Payments.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Controllers
{
    [HttpReadOnly]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StatsController : JsonApiController<Stats, Guid>
    {
        public StatsController(
            IJsonApiOptions options,
            IResourceService<Stats, Guid> resourceService,
            ILoggerFactory loggerFactory)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}