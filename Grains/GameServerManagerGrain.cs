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
        private readonly int _serverPort;

        public GameServerManagerGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
            // _tcpListener = new TcpListener(IPAddress.Any, 0);
            // _tcpListener.Start();

            var golfServerExecutable = Path.Combine(
                Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                Path.Combine("HSEGolf", "GolfServer.exe"));
            _logger.LogInformation($"Starting '{golfServerExecutable}'");

            var processSettings = new ProcessStartInfo(golfServerExecutable)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            _serverProcess = Process.Start(processSettings);

            var portString = _serverProcess.StandardOutput.ReadLine();
            if (portString == null)
            {
                throw new ApplicationException("Did not receive port number from child process.");
            }

            _serverPort = Int32.Parse(portString);
            _logger.LogInformation($"Child process is listening on port {_serverPort}");
        }

        public Task<string?> Hello()
        {
            // _serverProcess.StandardInput.WriteLine("Hello from your parent");
            // return _serverProcess.StandardOutput.ReadLineAsync();
            return Task.FromResult("Hello")!;
        }

        public Task<EndPoint> StartGame(GameSettings settings, Guid[] playerCookies)
        {
            settings.Write(_serverProcess.StandardInput);
            foreach (var cookie in playerCookies)
            {
                _serverProcess.StandardInput.WriteLine(cookie);
            }
            return Task.FromResult<EndPoint>(new IPEndPoint(IPAddress.Loopback, _serverPort));
        }
    }
}
