using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;
public class EvilAI : IGameAI
{
    private class HitInfo : IComparable<HitInfo>
    {
        public readonly int PlayerId;
        public readonly float HitAngle;
        public readonly float HitForce;
        private readonly Vector2 _metricValue;

        public HitInfo()
        {
            this.PlayerId = -1;
            this.HitAngle = float.NaN;
            this.HitForce = float.NaN;
            this._metricValue = Vector2.positiveInfinity;
        }
        public HitInfo(EvilAI parent, int playerId, float hitAngle, float hitForce)
        {
            this.PlayerId = playerId;
            this.HitAngle = hitAngle;
            this.HitForce = hitForce;
            this._metricValue = parent.EmulateHit(playerId, hitAngle, this.HitForce);
        }

        public int CompareTo(HitInfo other)
        {
            var xCompare = _metricValue.x.CompareTo(other._metricValue.x);
            return xCompare == 0 ? _metricValue.y.CompareTo(other._metricValue.y) : xCompare;
        }
    }

    private const float MinAngle = -0.5f;
    private const float MaxAngle = 0.5f;
    private const float AngleStep = 0.1f;
    private const float MinForce = 0.04f;
    private const float MaxForce = 1f;
    private const float ForceStep = 0.04f;

    private volatile bool _makeTurnAfterLoading = false;

    private readonly GameLogic _gameLogic;
    private readonly int _playerId;
    private volatile PlayerBall[] _playerBalls;

    public Scene SimulationScene;
    public EvilAI(GameLogic gameLogic, int playerId)
    {
        _gameLogic = gameLogic;
        _playerId = playerId;

        SceneManager.LoadSceneAsync(
            _gameLogic.GameSettings.SceneName, 
            new LoadSceneParameters(
                LoadSceneMode.Additive, 
                LocalPhysicsMode.Physics3D
            )
        );

        _gameLogic.SceneLoadSubscribers.Enqueue(OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene)
    {
        Debug.Log("EvilAI got scene");

        SimulationScene = scene;

        var rootGameObjects = SimulationScene.GetRootGameObjects();
        var rootGameObject = rootGameObjects[0];

        // Disable GameLoopManager in the physics scene.
        rootGameObject.GetComponentInChildren<GameLoopManager>().gameObject.SetActive(false);

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
        _playerBalls = rootGameObject.GetComponentsInChildren<PlayerBall>();

        if (_makeTurnAfterLoading)
            MakeTurn();
    }

    public void MakeTurn()
    {
        if (_playerBalls == null)
        {
            _makeTurnAfterLoading = true;
            return;
        }

        int[] playerIds = Enumerable.Range(0, _gameLogic.NumberOfPlayers).ToArray();
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            int tmp = playerIds[i];
            int j = Random.Range(i, _gameLogic.NumberOfPlayers);
            playerIds[i] = playerIds[j];
            playerIds[j] = tmp;
        }

        HitInfo bestHit = new HitInfo();
        for (int playerId = 0; playerId < _gameLogic.NumberOfPlayers; ++playerId) {
            var ballPosition = _gameLogic.PlayerBalls[_playerId].transform.position;
            var targetPosition = _gameLogic.TargetHole.transform.position;

            float targetHoleDirection = ballPosition.xz().LookAt(targetPosition.xz()) + (playerId == _playerId ? 0 : 1);

            for (float angle = targetHoleDirection + MinAngle; angle <= targetHoleDirection + MaxAngle; angle += AngleStep)
                for (float force = MinForce; force <= MaxForce; force += ForceStep)
                {
                    HitInfo newHit = new HitInfo(this, playerId, angle, force);
                    if (newHit.CompareTo(bestHit) < 0 && Random.value < 0.5)
                        bestHit = newHit;
                }
        }

        _gameLogic.HitBall(bestHit.PlayerId, bestHit.HitAngle, bestHit.HitForce);
    }


    public Vector2 EmulateHit(int ballOwnerId, float angle, float forceFrac)
    {
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            _playerBalls[i].Body.transform.position = _gameLogic.PlayerBalls[i].Body.transform.position;
            _playerBalls[i].Body.position = _gameLogic.PlayerBalls[i].Body.position;

        }
        var targetPosition= _gameLogic.TargetHole.transform.position;

        var ballBody = _playerBalls[ballOwnerId].Body;
        var forceVec = Vector3.forward * (GameLogic.MaxStrokeForce * forceFrac);
        ballBody.AddForce(Quaternion.Euler(0, angle, 0) * forceVec, ForceMode.Impulse);
        var physicsScene = SimulationScene.GetPhysicsScene();
        for (int nSteps = 0; nSteps < 300; ++nSteps)
        {
            if (_playerBalls.All(ball => ball.Body.IsSleeping()))
                break;

            physicsScene.Simulate(GameLogic.FrameDeltaTime);
        }

        var distance = Vector3.Distance(_playerBalls[_playerId].Body.position, targetPosition);
        var place = 0;

        for (int i = 0; i < _gameLogic.NumberOfPlayers; ++i)
        {
            if (i == _playerId)
                continue;

            if (Vector3.Distance(_playerBalls[i].Body.position, targetPosition) < distance)
                ++place;
        }

        return new Vector2(place, -distance);
    }
}