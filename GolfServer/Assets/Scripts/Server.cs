using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
	private struct Game
	{
		public Guid[] Cookies;
		public GameLogic Logic;
	}

	private Thread tcpListenerThread;
	private Thread parentListenerThread;

	private ConcurrentDictionary<Guid, Game> _ongoingGames = new ConcurrentDictionary<Guid, Game>();

	// Use this for initialization
	void Start()
	{
		// Start TcpServer background thread
		tcpListenerThread = new Thread(ListenForIncomingRequests) {IsBackground = true};
		tcpListenerThread.Start();
	}

	/// <summary>
	/// Runs in background TcpServerThread; Handles incoming TcpClient requests
	/// </summary>
	private void ListenForIncomingRequests()
	{
		try
		{
			// Create listener on localhost with OS-assigned port.
			// var tcpListener = new TcpListener(IPAddress.Any, 0);
			var tcpListener = new TcpListener(IPAddress.Any, 6015);
			tcpListener.Start();

			Debug.Log($"Server is listening on {tcpListener.LocalEndpoint}");

			Console.Out.WriteLine(((IPEndPoint) tcpListener.LocalEndpoint).Port);
			Console.Out.Flush();

			parentListenerThread = new Thread(ListenParentProcess) {IsBackground = true};
			parentListenerThread.Start();

			// Byte[] bytes = new Byte[1024];
			while (true)
			{
				var connectedClient = tcpListener.AcceptTcpClient();
				Debug.Log($"Incoming connection {connectedClient.Client.RemoteEndPoint}");
				var clientThread = new Thread(() => ListenClient(connectedClient)) {IsBackground = true};
				clientThread.Start();
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}

	private void ListenParentProcess()
	{
		while (true)
		{
			Debug.Log("Waiting for messages from parent");

			var gameSetting = GameSettings.Read(Console.In);
			Debug.Log("Received GameSettings from parent");

			var playerCount = int.Parse(Console.In.ReadLine()
			                            ?? throw new NullReferenceException("Number of players is not provided"));
			var game = new Game
			{
				Cookies = new Guid[playerCount],
				Logic = new GameLogic(gameSetting)
			};

			for (int i = 0; i < playerCount; i++)
			{
				game.Cookies[i] = Guid.Parse(Console.In.ReadLine()
				                              ?? throw new NullReferenceException($"Cookie {i} is not provided"));
				_ongoingGames[game.Cookies[i]] = game;
			}

			Debug.Log($"Starting game on level {gameSetting.SceneName} with {playerCount} players: {game.Cookies}");
		}
	}

	private void ListenClient(TcpClient connectedClient)
	{
		using (var client = connectedClient)
		{
			var clientId = client.Client.RemoteEndPoint.ToString();
			using (NetworkStream stream = client.GetStream()) {
				Debug.Log($"Waiting for cookie from client {clientId}");
				IFormatter formatter = new BinaryFormatter();

				var cookie = (Guid) formatter.Deserialize(stream);
				if (!_ongoingGames.ContainsKey(cookie))
				{
					Debug.LogError($"Unknown cookie: {cookie}");
					return;
				}

				Debug.Log($"Client {clientId} connected with cookie {cookie}");
				var game = _ongoingGames[cookie];
				while (true)
				{
					var command = (ClientToServerMessage) formatter.Deserialize(stream);
					lock (game.Logic)
					{
						switch (command)
						{
							case ClientToServerMessage.Heartbeat _:
								game.Logic.NextMove();
								break;
							case ClientToServerMessage.HitBall hitBallCmd:
								// TODO: check player id
								game.Logic.HitBall(hitBallCmd.PlayerId, hitBallCmd.Angle, hitBallCmd.Force);
								break;
						}
					}
				}
			}
		}
	}

// /// <summary>
	// /// Send message to client using socket connection.
	// /// </summary>
	// private void SendMessage() {
	// 	if (connectedTcpClient == null) {
	// 		return;
	// 	}
	//
	// 	try {
	// 		// Get a stream object for writing.
	// 		NetworkStream stream = connectedTcpClient.GetStream();
	// 		if (stream.CanWrite) {
	// 			string serverMessage = "This is a message from your server.";
	// 			// Convert string message to byte array.
	// 			byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
	// 			// Write byte array to socketConnection stream.
	// 			stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
	// 			Debug.Log("Server sent his message - should be received by client");
	// 		}
	// 	}
	// 	catch (SocketException socketException) {
	// 		Debug.Log("Socket exception: " + socketException);
	// 	}
	// }
}
