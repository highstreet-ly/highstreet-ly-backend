using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : JsonApiController<Order, Guid>
    {
        public OrdersController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<Order, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}