using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;

namespace FrontendServer.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IClusterClient _client;
        
        public HelloController(ILogger<WeatherForecastController> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<String> Get()
        {
            var friend = this._client.GetGrain<IHello>(0);
            var response = await friend.SayHello("Good morning, my friend!");
            _logger.Log(LogLevel.Warning, "FOOBAR");
            return response;
        }
    }
}