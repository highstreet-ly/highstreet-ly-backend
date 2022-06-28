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
    public class OrderTicketsController : JsonApiController<OrderTicket, Guid>
    {
        public OrderTicketsController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<OrderTicket, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}