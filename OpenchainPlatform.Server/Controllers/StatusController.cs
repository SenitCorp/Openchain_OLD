using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Server.Controllers
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
