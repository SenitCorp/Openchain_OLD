using Microsoft.AspNetCore.Mvc;

namespace OpenchainPlatform.API.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        [HttpGet, Route("")]
        public IActionResult GetStatus()
        {
            return Ok();
        }
    }
}
