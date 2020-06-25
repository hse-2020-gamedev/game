using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
                // RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            _serverProcess = Process.Start(processSettings);

            var portString = _serverProcess.StandardError.ReadLine();
            if (portString == null)
            {
                throw new ApplicationException("Did not receive port number from child process.");
            }
            _logger.LogWarning($"FOOBAR PortString {portString}");

            _serverPort = Int32.Parse(portString);
            _logger.LogInformation($"Child process is listening on port {_serverPort}");

            new Thread(() => PrintErrors(_serverProcess.StandardError)) {IsBackground = true}.Start();
        }

        public Task<string?> Hello()
        {
            // _serverProcess.StandardInput.WriteLine("Hello from your parent");
            // return _serverProcess.StandardOutput.ReadLineAsync();
            return Task.FromResult("Hello")!;
        }

        public Task<IPEndPoint> StartGame(GameSettings settings, Guid[] playerCookies)
        {
            settings.Write(_serverProcess.StandardInput);
            _serverProcess.StandardInput.WriteLine(playerCookies.Length);
            foreach (var cookie in playerCookies)
            {
                _serverProcess.StandardInput.WriteLine(cookie);
            }
            return Task.FromResult(new IPEndPoint(IPAddress.Loopback, _serverPort));
        }

        private void PrintErrors(TextReader reader)
        {
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    Console.Error.WriteLine(line);
                    Console.Error.Flush();
                }
            }
        }
    }
}
