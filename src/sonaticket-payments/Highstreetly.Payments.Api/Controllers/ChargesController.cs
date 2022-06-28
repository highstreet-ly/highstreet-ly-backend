using System;
using Highstreetly.Payments.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChargesController : JsonApiController<Charge, Guid>
    {
        public ChargesController(
            IJsonApiOptions options,
            IResourceService<Charge, Guid> resourceService,
            ILoggerFactory loggerFactory)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}