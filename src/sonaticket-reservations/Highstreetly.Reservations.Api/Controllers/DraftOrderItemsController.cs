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
    public class DraftOrderItemsController : JsonApiController<DraftOrderItem, Guid>
    {
        public DraftOrderItemsController(
            IJsonApiOptions options, 
            ILoggerFactory loggerFactory,
            IResourceService<DraftOrderItem, Guid> resourceService) 
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}