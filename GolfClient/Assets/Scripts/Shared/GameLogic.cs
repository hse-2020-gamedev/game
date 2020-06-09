using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum PlayerType
{
    Human,
    DummyAI,
    EvilAI
}

public class GameSettings
{
    public string SceneName;
    public PlayerType[] PlayerTypes;
}

public class GameLogic
{
    public readonly static float FrameDeltaTime = 1 / 60f;
    public readonly static float MaxStrokeForce = 10f;

    public int CurrentPlayer { get; private set; }
    public readonly PlayerType[] PlayerTypes;
    public readonly IGameAI[] AIs;
    public int NumberOfPlayers => PlayerTypes.Length;

    public Scene SimulationScene { get; private set; }
    public readonly GameSettings GameSettings;
    public PlayerBall[] PlayerBalls { get; private set; }
    public GameObject TargetHole { get; private set; }

    public class GameState
    {
        private GameState() { }

        public sealed class WaitingForHumanPlayer : GameState
        {
            public readonly int PlayerId;

            private WaitingForHumanPlayer(int playerId)
            {
                PlayerId = playerId;
            }
        }
    }

    public readonly ConcurrentQueue<UnityAction<Scene>> SceneLoadSubscribers;
    public readonly ConcurrentQueue<Event> Events;
    
    public GameLogic(GameSettings gameSettings)
    {
        Events = new ConcurrentQueue<Event>();

        GameSettings = gameSettings;

        SceneLoadSubscribers = new ConcurrentQueue<UnityAction<Scene>>();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Start scene loading
        SceneManager.LoadSceneAsync(gameSettings.SceneName, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
        SceneLoadSubscribers.Enqueue(OnSimulationSceneLoaded);

        // Prepare AI
        CurrentPlayer = 0;
        PlayerTypes = gameSettings.PlayerTypes;
        AIs = new IGameAI[NumberOfPlayers];
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool success = SceneLoadSubscribers.TryDequeue(out var callback);
        Assert.IsTrue(success);

        callback(scene);
    }

    private void OnSimulationSceneLoaded(Scene scene)
    {
        Debug.Log("GameLogic got scene");

        SimulationScene = scene;

        var rootGameObjects = SimulationScene.GetRootGameObjects();
        var rootGameObject = rootGameObjects[0];
        
        // Disable GameLoopManager in the physics scene.
        rootGameObject.GetComponentInChildren<GameLoopManager>().gameObject.SetActive(false);

        TargetHole = rootGameObject.GetComponentsInChildren<Transform>()
            .First(child => child.gameObject.name == "TargetHole").gameObject;

        // Disable cameras in the physics scene.
        foreach (var camera in rootGameObject.GetComponentsInChildren<Camera>())
        {
            camera.gameObject.SetActive(false);
        }

        // Disable light in the physics scene.
        foreach (var light in rootGameObject.GetComponentsInChildren<Light>())
        {
            light.gameObject.SetActive(false);
        }

        // Make all objects in the physics scene invisible.
        foreach (var renderer in rootGameObject.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        // Find player balls.
        PlayerBalls = rootGameObject.GetComponentsInChildren<PlayerBall>();
        foreach (var playerBall in PlayerBalls)
            playerBall.Body.sleepThreshold = 0.5f;

        if (PlayerBalls.Length != NumberOfPlayers)
        {
            throw new ArgumentException($"Inconsistent number of players: {PlayerBalls.Length} vs {NumberOfPlayers}");
        }

        // Initialize AIs.
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            switch (PlayerTypes[i])
            {
                case PlayerType.Human:
                    AIs[i] = null;
                    Debug.Log(i + " is HUMAN");
                    break;
                case PlayerType.DummyAI:
                    AIs[i] = new DummyAI(this, i);
                    Debug.Log(i + " is DUMMYAI");
                    break;
                case PlayerType.EvilAI:
                    AIs[i] = new EvilAI(this, i);
                    Debug.Log(i + " is EVILAI");
                    break;
                default:
                    throw new ArgumentException($"Unknown player type: {PlayerTypes[i]}");
            }
        }

        NextMove();
    }

    public void HitBall(int playerId, float angle, float forceFrac)
    {
        //if (playerId != CurrentPlayer) throw new ArgumentException($"Wrong player ID: {playerId}. Expected: {CurrentPlayer}");
        if (forceFrac < 0 || forceFrac > 1) throw new ArgumentException($"Invalid force fraction value: {forceFrac}");

        var ballBody = PlayerBalls[playerId].Body;  
        var forceVec = Vector3.forward * (MaxStrokeForce * forceFrac);
        ballBody.AddForce(Quaternion.Euler(0, angle, 0) * forceVec, ForceMode.Impulse);
        var trajectory = new Trajectory(FrameDeltaTime);

        var physicsScene = SimulationScene.GetPhysicsScene();

        for (int nSteps = 0; !AllBallsSleeping(); ++nSteps) {
            physicsScene.Simulate(FrameDeltaTime);
            trajectory.AddFrame(Frame.Extract(PlayerBalls));
            if (nSteps > 3000)
            {
                Debug.Log($"Ball0Position: {PlayerBalls[0].transform.position}, " +
                          $"Ball0Velocity: {PlayerBalls[0].Body.velocity}, " +
                          $"Ball0Sleeping: {PlayerBalls[0].Body.IsSleeping()}");
                Debug.Log($"Ball1Position: {PlayerBalls[1].transform.position}, " +
                          $"Ball1Velocity: {PlayerBalls[1].Body.velocity}, " +
                          $"Ball1Sleeping: {PlayerBalls[1].Body.IsSleeping()}");
                throw new ApplicationException("Simulation is too long.");
            }
        }
        
        Events.Enqueue(new Event.PlayTrajectory(trajectory, playerId));
        
        CurrentPlayer = (CurrentPlayer + 1) % NumberOfPlayers;
    }

    private bool AllBallsSleeping()
    {
        return PlayerBalls.All(ball => ball.Body.IsSleeping());
    }

    private bool IsCurrentPlayerAI()
    {
        return PlayerTypes[CurrentPlayer] != PlayerType.Human;
    }

    public void NextMove()
    {
        if (PlayerBalls[0].getLayerId() != 0) {
            Events.Enqueue(new Event.Finish("Player 0 win!"));
        }
        
        if (PlayerBalls[1].getLayerId() != 0) {
            Events.Enqueue(new Event.Finish("Player 1 win!"));
        }
        
        Events.Enqueue(new Event.TurnOfPlayer(CurrentPlayer));
        if (PlayerTypes[CurrentPlayer] != PlayerType.Human)
        {
            AIs[CurrentPlayer].MakeTurn();
        }
    }
}
