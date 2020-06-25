using System;
using System.Net;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface ILobby : Orleans.IGrainWithIntegerKey
    {
        Task<Guid> SearchGame(GameSettings settings);
        Task<IPEndPoint?> CheckStatus(Guid cookie);

        Task StopSearching(Guid cookie);
        // Task<string?> ReadyForStart(Guid cookie);
    }

    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string? message) : base(message)
        { }
    }
}
