﻿using System;
using System.Net;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Orleans;

namespace FrontendServer.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LobbyController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IClusterClient _client;
        
        public LobbyController(ILogger<WeatherForecastController> logger, IClusterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpPost]
        [Route("SearchGame")]
        public async Task<Guid> SearchGame(string levelName)
        {
            _logger.LogInformation($"SearchGame in level '{levelName}'.");
            var lobbyGrain = this._client.GetGrain<ILobby>(0);
            Guid cookie = await lobbyGrain.SearchGame(levelName);
            return cookie;
        }
        
        [HttpPost]
        [Route("CheckStatus")]
        public async Task<IActionResult> CheckStatus(Guid cookie)
        {
            try
            {
                _logger.LogDebug($"CheckStatus with cookie '{cookie}'.");
                var lobbyGrain = this._client.GetGrain<ILobby>(0);
                return Ok(await lobbyGrain.CheckStatus(cookie));
            }
            catch (PlayerNotFoundException e)
            {
                return NotFound(); 
            }
        }
        
        [HttpPost]
        [Route("StopSearching")]
        public async Task<IActionResult> StopSearching(Guid cookie)
        {
            try
            {
                _logger.LogInformation($"StopSearching with cookie '{cookie}'.");
                var lobbyGrain = this._client.GetGrain<ILobby>(0);
                await lobbyGrain.StopSearching(cookie);
                return Ok();
            }
            catch (PlayerNotFoundException)
            {
                return NotFound();
            }
        }
    }
}