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
    public class LogEntryController : JsonApiController<LogEntry, int>
    {
        public LogEntryController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<LogEntry, int> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }
    }
}