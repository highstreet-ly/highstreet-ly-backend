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
    public class TicketDetailsController : JsonApiController<OrderTicketDetails, Guid>
    {
        public TicketDetailsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<OrderTicketDetails, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}