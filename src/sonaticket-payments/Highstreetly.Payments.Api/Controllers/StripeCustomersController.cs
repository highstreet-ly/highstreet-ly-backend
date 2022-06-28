using Highstreetly.Payments.ViewModels.Payments;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Payments.Api.Controllers
{
    // [HttpReadOnly]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // public class StripeCustomersController : JsonApiController<StripeCustomer, string>
    // {
    //     public StripeCustomersController(
    //         IJsonApiOptions options,
    //         IResourceService<StripeCustomer, string> resourceService,
    //         ILoggerFactory loggerFactory)
    //         : base(options, loggerFactory, resourceService)
    //     {
    //     }
    // }
}