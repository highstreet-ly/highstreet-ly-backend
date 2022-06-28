using System;
using Highstreetly.Permissions.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Permissions.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : JsonApiController<Role, Guid>
    {
        public RolesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<Role, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        { }
    }
}