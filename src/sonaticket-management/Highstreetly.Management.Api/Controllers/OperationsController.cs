using JsonApiDotNetCore.AtomicOperations;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Middleware;
using JsonApiDotNetCore.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class OperationsController : JsonApiOperationsController
    {
        public OperationsController(IJsonApiOptions options, ILoggerFactory loggerFactory,
            IOperationsProcessor processor, IJsonApiRequest request,
            ITargetedFields targetedFields)
            : base(options, loggerFactory, processor, request, targetedFields)
        {
        }
    }
}