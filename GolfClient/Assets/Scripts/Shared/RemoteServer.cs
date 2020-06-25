using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
    private Thread _connectThread;
    private Thread _incomingMessagesThread;
    private Thread _outgoingMessagesThread;
    private ConcurrentQueue<Event> _incomingEvents = new ConcurrentQueue<Event>();
    private BlockingCollection<ClientToServerMessage> _outgoingMessages = new BlockingCollection<ClientToServerMessage>();
    private Guid _cookie;

    public RemoteServer(GameSettings settings, IPEndPoint frontendEP)
    {
        _connectThread = new Thread(() => ConnectToServer(settings, frontendEP).Wait()) {IsBackground = true};
        _connectThread.Start();
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

    private async Task ConnectToServer(GameSettings settings, IPEndPoint frontendEP)
    {
        Debug.Log($"Connecting to frontend server {frontendEP}");
        var httpClient = new HttpClient();

        Debug.Log($"Post http://{frontendEP}/Lobby/SearchGame?settingsString={settings}");
        var cookieHttpResp = await httpClient.PostAsync(
            $"http://{frontendEP}/Lobby/SearchGame?settingsString={settings}",
            null);

        cookieHttpResp.EnsureSuccessStatusCode();
        var cookieString = (await cookieHttpResp.Content.ReadAsStringAsync());
        _cookie = Guid.Parse(cookieString);
        Debug.Log($"Received cookie {_cookie}");

        while (true)
        {
            Debug.Log($"Post http://{frontendEP}/Lobby/CheckStatus?cookie={_cookie}");
            var statusHttpResp = await httpClient.PostAsync(
                $"http://{frontendEP}/Lobby/CheckStatus?cookie={_cookie}",
                null);
            statusHttpResp.EnsureSuccessStatusCode();
            if (statusHttpResp.StatusCode == HttpStatusCode.OK)
            {
                var serverEP = ParseIPEndPoint(await statusHttpResp.Content.ReadAsStringAsync());
                _tcpClient = new TcpClient(serverEP.Address.ToString(), serverEP.Port);
                _tcpClientStream = _tcpClient.GetStream();
                break;
            }
            Thread.Sleep(1000);
        }

        _incomingMessagesThread = new Thread(HandleIncomingMessages) {IsBackground = true};
        _incomingMessagesThread.Start();

        _outgoingMessagesThread = new Thread(HandleOutgoingMessages) {IsBackground = true};
        _outgoingMessagesThread.Start();
    }

    // From https://stackoverflow.com/questions/2727609/best-way-to-create-ipendpoint-from-string
    private static IPEndPoint ParseIPEndPoint(string endPoint)
    {
        string[] ep = endPoint.Split(':');
        if(ep.Length != 2) throw new FormatException("Invalid endpoint format");

        if(!IPAddress.TryParse(ep[0], out var ip))
        {
            throw new FormatException("Invalid ip-adress");
        }

        if(!int.TryParse(ep[1], out var port))
        {
            throw new FormatException("Invalid port");
        }
        return new IPEndPoint(ip, port);
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
        formatter.Serialize(_tcpClientStream, _cookie);
        while (true)
        {
            var message = _outgoingMessages.Take();
            formatter.Serialize(_tcpClientStream, message);
            _tcpClientStream.Flush();
        }
    }
}
