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
    public class RefundsController : JsonApiController<Refund, Guid>
    {
        public RefundsController(
            IJsonApiOptions options,
            IResourceService<Refund, Guid> resourceService,
            ILoggerFactory loggerFactory)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}