using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // public readonly ConcurrentQueue<UnityAction<Scene>> SceneLoadSubscribers;
    public readonly ConcurrentQueue<Event> Events;

    public readonly BlockingCollection<Event> Events1 = new BlockingCollection<Event>();
    public readonly BlockingCollection<Event> Events2 = new BlockingCollection<Event>();
    private bool _newMove = false;

    public GameLogic(GameSettings gameSettings)
    {
        Events = new ConcurrentQueue<Event>();

        GameSettings = gameSettings;

        // SceneLoadSubscribers = new ConcurrentQueue<UnityAction<Scene>>();
        // SceneManager.sceneLoaded += OnSceneLoaded;

        // Start scene loading
        var sceneAsync = SceneManager.LoadSceneAsync(
            gameSettings.SceneName,
            new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        sceneAsync.completed += _ => OnSimulationSceneLoaded(scene);
        // SceneLoadSubscribers.Enqueue(OnSimulationSceneLoaded);

        // Prepare AI
        CurrentPlayer = 0;
        PlayerTypes = gameSettings.PlayerTypes;
        AIs = new IGameAI[NumberOfPlayers];
    }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     bool success = SceneLoadSubscribers.TryDequeue(out var callback);
    //     Assert.IsTrue(success, "No scene loader subscribed?");
    //
    //     callback(scene);
    // }

    private void OnSimulationSceneLoaded(Scene scene)
    {
        Debug.Log("GameLogic got scene");

        SimulationScene = scene;
        MakeSceneInvisible(scene);

        var rootGameObjects = scene.GetRootGameObjects();
        var rootGameObject = rootGameObjects[0];

        TargetHole = rootGameObject.GetComponentsInChildren<Transform>()
            .First(child => child.gameObject.name == "TargetHole").gameObject;

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

        _newMove = true;
        NextMove();
    }

    public static void MakeSceneInvisible(Scene scene)
    {
        var rootGameObject = scene.GetRootGameObjects()[0];

        // Disable GameLoopManager in the physics scene.
        // rootGameObject.GetComponentsInChildren<Transform>()
        //     .First(child => child.gameObject.name == "GameLoopManager").gameObject.SetActive(false);

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

        lock (this)
        {
            CurrentPlayer = (CurrentPlayer + 1) % NumberOfPlayers;
            _newMove = true;
            EnqueueEvent(new Event.PlayTrajectory(trajectory, playerId));
        }
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
        if (_newMove)
        {
            _newMove = false;

            if (PlayerBalls[0].getLayerId() != 0) {
                EnqueueEvent(new Event.Finish("Player 0 win!"));
                CurrentPlayer = -1;
                return;
            }

            if (PlayerBalls[1].getLayerId() != 0) {
                EnqueueEvent(new Event.Finish("Player 1 win!"));
                CurrentPlayer = -1;
                return;
            }

            EnqueueEvent(new Event.TurnOfPlayer(CurrentPlayer));
            if (PlayerTypes[CurrentPlayer] != PlayerType.Human)
            {
                AIs[CurrentPlayer].MakeTurn();
            }
        }
    }

    private void EnqueueEvent(Event ev)
    {
        Events.Enqueue(ev);
        Events1.Add(ev);
        Events2.Add(ev);
    }
}
