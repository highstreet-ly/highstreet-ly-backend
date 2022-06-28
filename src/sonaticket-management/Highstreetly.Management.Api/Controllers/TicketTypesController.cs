using System;
using System.Threading;
using System.Threading.Tasks;
using Highstreetly.Management.Resources;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Management.Api.Controllers
{
    public class TicketTypesController : JsonApiController<TicketType, Guid>
    {
        public TicketTypesController(
            IJsonApiOptions options,
            ILoggerFactory loggerFactory,
            IResourceService<TicketType, Guid> resourceService)
            : base(options, loggerFactory, resourceService)
        {
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var result = await base.GetAsync(cancellationToken);

            return result;
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await base.GetAsync(id, cancellationToken);
            return result;
        }
    }
}