using System;
using System.Net;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IGameServerManager : Orleans.IGrainWithIntegerKey
    {
        Task<string?> Hello();
        Task<string> StartGame(GameSettings settings, Guid[] playerCookies);
    }
}