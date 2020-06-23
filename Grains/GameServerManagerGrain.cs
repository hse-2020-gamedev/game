using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GrainInterfaces;

namespace Grains
{
    public class GameServerManagerGrain : Orleans.Grain, IGameServerManager
    {
        private readonly ILogger _logger;
        // private readonly TcpListener _tcpListener;
        private readonly Process _serverProcess;
        
        public GameServerManagerGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
            // _tcpListener = new TcpListener(IPAddress.Any, 0);
            // _tcpListener.Start();
            
            var golfServerPath = Path.Combine(
                Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
                "GolfServer.exe");
            _logger.LogInformation($"Starting '{golfServerPath}'");
            _serverProcess = Process.Start(golfServerPath);
        }

        public Task<string?> Hello()
        {
            _serverProcess.StandardInput.WriteLine("Hello from your parent");
            return _serverProcess.StandardOutput.ReadLineAsync();
        }

        public Task<string> StartGame(GameSettings settings, Guid[] playerCookies)
        {
            // return Task.FromResult(_tcpListener.LocalEndpoint);
            return Task.FromResult("Hello from game server manager");
        }
    }
}
