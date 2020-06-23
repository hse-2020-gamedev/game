using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GrainInterfaces;

namespace Grains
{
    public class LobbyGrain : Orleans.Grain, ILobby
    {
        private struct FormedGame
        {
            // public Dictionary<Guid, bool> PlayerApprove;
            public Guid[] PlayerCookies;
            public string LevelName;

            public FormedGame(Guid playerCookie1, Guid playerCookie2, string levelName)
            {
                // PlayerApprove = new Dictionary<Guid, bool>();
                // PlayerApprove[playerCookie1] = false;
                // PlayerApprove[playerCookie2] = false;
                PlayerCookies = new[] {playerCookie1, playerCookie2};
                LevelName = levelName;
            }
        }

        private const int GameServerManagerCount = 2;
        
        private readonly ILogger _logger;
        private readonly Dictionary<string, Guid> _waitingPlayersOnLevel = new Dictionary<string, Guid>();
        private readonly Dictionary<Guid, FormedGame> _formedGames = new Dictionary<Guid, FormedGame>();
        private readonly Dictionary<Guid, string> _levelNameByCookie = new Dictionary<Guid, string>();
        private readonly Random _random = new Random();
        
        public LobbyGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
        }

        public async Task<Guid> SearchGame(string levelName)
        {
            var gameServerManagerGrain = GrainFactory.GetGrain<IGameServerManager>(0);
            _logger.LogInformation(await gameServerManagerGrain.Hello());
            
            // TODO: Guids are not really secure, replace with real random.
            // TODO: See https://security.stackexchange.com/questions/890/are-guids-safe-for-one-time-tokens
            var cookie = Guid.NewGuid();
            // TODO: Check that "levelName" exists.
            _levelNameByCookie[cookie] = levelName;

            if (!_waitingPlayersOnLevel.ContainsKey(levelName))
            {
                _waitingPlayersOnLevel.Add(levelName, cookie);
            }
            else
            {
                // TODO: Use timers to kick out unresponsive players.
                var otherCookie = _waitingPlayersOnLevel[levelName];
                var players = new FormedGame(cookie, otherCookie, levelName);
                _formedGames[cookie] = players;
                _formedGames[otherCookie] = players;
                _waitingPlayersOnLevel.Remove(levelName);
            }

            // return Task.FromResult(cookie);
            return cookie;
        }

        public Task<string?> CheckStatus(Guid cookie)
        {
            if (!_levelNameByCookie.ContainsKey(cookie))
            {
                throw new PlayerNotFoundException($"Unknown player cookie: {cookie}");
            }

            if (!_formedGames.ContainsKey(cookie))
            {
                return Task.FromResult<string?>(null);
            }

            var formedGame = _formedGames[cookie];

            var gameSettings = new GameSettings();
            gameSettings.PlayerTypes = new[] {PlayerType.Human, PlayerType.Human};
            gameSettings.SceneName = formedGame.LevelName;

            var gameServerManager = GrainFactory.GetGrain<IGameServerManager>(_random.Next(GameServerManagerCount));
            return gameServerManager.StartGame(gameSettings, formedGame.PlayerCookies)!;
        }

        public Task StopSearching(Guid cookie)
        {
            if (!_levelNameByCookie.ContainsKey(cookie))
            {
                throw new PlayerNotFoundException($"Unknown player cookie: {cookie}");
            }

            if (_waitingPlayersOnLevel[_levelNameByCookie[cookie]] == cookie)
            {
                _waitingPlayersOnLevel.Remove(_levelNameByCookie[cookie]);
            }
            _formedGames.Remove(cookie);

            return Task.CompletedTask;
        }

        // public async Task<string?> ReadyForStart(Guid cookie)
        // {
        //     _formedGames[cookie].PlayerApprove[cookie] = true;
        //     if (_formedGames[cookie].PlayerApprove.Values.All(_ => _))
        //     {
        //         // Start game
        //         return "Start game";
        //     }
        //
        //     return null;
        // }

        // public async Task<bool?> CheckStatus(Guid cookie)
        // {
        //     if (_playerReady.ContainsKey(cookie))
        //     {
        //         return null;
        //     }
        // }
        //
        // Task<IPAddress> ApproveStartGame(string cookie);
        //
        // Task<string> Se(string greeting)
        // {
        //     _logger.LogInformation($"\n SayHello message received: greeting = '{greeting}'");
        //     return Task.FromResult($"\n Client said: '{greeting}', so HelloGrain says: Hello!");
        // }
    }
}