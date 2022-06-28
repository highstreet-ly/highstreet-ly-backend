using System;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Reservations.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DraftOrdersController : JsonApiController<DraftOrder, Guid>
    {
        public DraftOrdersController(
            IJsonApiOptions options, 
            ILoggerFactory loggerFactory,
            IResourceService<DraftOrder, Guid> resourceService) 
            : base(options, loggerFactory, resourceService)
        {
        }


        [AllowAnonymous]
        public override Task<IActionResult> PostAsync(DraftOrder resource, CancellationToken cancellationToken)
        {
            return base.PostAsync(resource, cancellationToken);
        }
    }
}