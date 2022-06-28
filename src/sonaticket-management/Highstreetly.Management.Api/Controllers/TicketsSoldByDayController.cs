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
    public class TicketsSoldByDayController : JsonApiController<TicketsSoldByDay, Guid>
    {
        public TicketsSoldByDayController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<TicketsSoldByDay, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}