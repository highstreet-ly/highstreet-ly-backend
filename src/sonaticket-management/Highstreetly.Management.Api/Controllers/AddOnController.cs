using System;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AddOnController : JsonApiController<AddOn, Guid>
    {
        public AddOnController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<AddOn, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}