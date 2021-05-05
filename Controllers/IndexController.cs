using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Controllers
{
    [ApiController]
    [Route("API/Index")]
    public class IndexController : ControllerBase
    {

        [HttpGet]
        public string Welcome()
        {
            return "Home Page Noleggio Automezzi.";

        }
    }
}
