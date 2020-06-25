using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

[Serializable]
public abstract class ClientToServerMessage
{
    private ClientToServerMessage()
    { }

    [Serializable]
    public sealed class Heartbeat : ClientToServerMessage
    { }

    // [Serializable]
    // public sealed class Exit : ClientToServerMessage
    // { }

    [Serializable]
    public sealed class HitBall : ClientToServerMessage
    {
        public int PlayerId;
        public float Angle;
        public float Force;
    }
}

public class RemoteServer : IServerHandle
{
    private TcpClient _tcpClient;
    private NetworkStream _tcpClientStream;
    private Thread _incomingMessagesThread;
    private Thread _outgoingMessagesThread;
    private ConcurrentQueue<Event> _incomingEvents = new ConcurrentQueue<Event>();
    private BlockingCollection<ClientToServerMessage> _outgoingMessages = new BlockingCollection<ClientToServerMessage>();

    public RemoteServer(IPEndPoint serverEP)
    {
        _tcpClient = new TcpClient(serverEP.Address.ToString(), serverEP.Port);
        _tcpClientStream = _tcpClient.GetStream();

        _incomingMessagesThread = new Thread(HandleIncomingMessages) {IsBackground = true};
        _incomingMessagesThread.Start();

        _outgoingMessagesThread = new Thread(HandleOutgoingMessages) {IsBackground = true};
        _outgoingMessagesThread.Start();
    }

    public void HitBall(int playerId, float angle, float force)
    {
        _outgoingMessages.Add(new ClientToServerMessage.HitBall
        {
            PlayerId = playerId,
            Angle = angle,
            Force = force
        });
    }

    public void LeaveGame()
    {
        throw new NotImplementedException();
    }

    public void NextMove()
    {
        _outgoingMessages.Add(new ClientToServerMessage.Heartbeat());
    }

    public Event NextEvent()
    {
        _incomingEvents.TryDequeue(out var result);
        return result;
    }

    private void HandleIncomingMessages()
    {
        var formatter = new BinaryFormatter();
        while (true)
        {
            var ev = (Event) formatter.Deserialize(_tcpClientStream);
            _incomingEvents.Enqueue(ev);
        }
    }

    private void HandleOutgoingMessages()
    {
        var formatter = new BinaryFormatter();
        while (true)
        {
            var message = _outgoingMessages.Take();
            formatter.Serialize(_tcpClientStream, message);
        }
    }
}
