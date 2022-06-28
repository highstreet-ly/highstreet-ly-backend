using Microsoft.AspNetCore.Mvc;

namespace Highstreetly.Signalr.Controllers
{
    [Route("/healthz")]
    [Route("/")]
    public class HealthController : Controller
    {
       
        public IActionResult GetAction(){
            return Ok();
        }
    }
}