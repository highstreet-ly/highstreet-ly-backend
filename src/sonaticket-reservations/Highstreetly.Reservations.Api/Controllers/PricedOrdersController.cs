using System;
using Highstreetly.Reservations.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PricedOrdersController : JsonApiController<PricedOrder, Guid>
    {
        public PricedOrdersController(
            IJsonApiOptions options,
            IResourceService<PricedOrder, Guid> resourceService,
            ILoggerFactory loggerFactory)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}