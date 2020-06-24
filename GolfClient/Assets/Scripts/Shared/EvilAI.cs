using System;
using System.Linq;
using UnityEngine;
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
        private readonly Vector3 _metricValue;

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
            if (xCompare != 0)
                return xCompare;

            var yCompare = _metricValue.y.CompareTo(other._metricValue.y);
            if (yCompare != 0)
                return yCompare;

            return _metricValue.z.CompareTo(other._metricValue.z);
        }
    }

    private const float MinForce = 0.0f;
    private const float MaxForce = 1f;
    private const float LinearSearchStep = 0.1f;
    private const int BinarySearchSteps = 5;

    private const float MinAngle = -10f;
    private const float MaxAngle = 10f;
    private const float AngleStep = 5f;

    private volatile bool _makeTurnAfterLoading = false;

    private readonly GameLogic _gameLogic;
    private readonly int _playerId;
    private volatile PlayerBall[] _playerBalls;

    public Scene SimulationScene;
    public EvilAI(GameLogic gameLogic, int playerId)
    {
        _gameLogic = gameLogic;
        _playerId = playerId;

        _gameLogic.SceneLoader.LoadSimulationScene(OnSceneLoaded);
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

        int[] playerIds = Enumerable.Range(0, _gameLogic.NumberOfPlayers).ToArray(); // shuffling players
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            int tmp = playerIds[i];
            int j = Random.Range(i, _gameLogic.NumberOfPlayers);
            playerIds[i] = playerIds[j];
            playerIds[j] = tmp;
        }

        HitInfo bestHit = new HitInfo(this, _playerId, 0, 0);
        for (int playerIndex = 0; playerIndex < _gameLogic.NumberOfPlayers; ++playerIndex)
        {
            int playerId = playerIds[playerIndex];
            var ballPosition = _gameLogic.PlayerBalls[playerId].transform.position;
            var targetPosition = _gameLogic.PlayerBalls[playerId].GoalHint;

            float goalDirection = ballPosition.xz().LookAt(targetPosition.xz()) - (playerId == _playerId ? 0 : 180);
            float minDirectionDelta = playerId == _playerId ? MinAngle : 0;
            float maxDirectionDelta = playerId == _playerId ? MaxAngle : 0;
            for (float angle = goalDirection + minDirectionDelta; angle <= goalDirection + maxDirectionDelta; angle += AngleStep)
            {
                HitInfo bestForceHit = new HitInfo(this, playerId, angle, MinForce);
                for (float force = MinForce; force <= MaxForce; force += LinearSearchStep)
                {
                    HitInfo newForceHit = new HitInfo(this, playerId, angle, force);
                    if (newForceHit.CompareTo(bestForceHit) < 0 && Random.value < 0.9)
                        bestForceHit = newForceHit;
                }

                float forceL = Math.Max(MinForce, bestForceHit.HitForce - LinearSearchStep);
                float forceR = Math.Min(MaxForce, bestForceHit.HitForce + LinearSearchStep);
                for (int step = 0; step < BinarySearchSteps; ++step)
                {
                    float forceMl = (forceL * 2 + forceR) / 3;
                    HitInfo hitMl = new HitInfo(this, playerId, angle, forceMl);

                    float forceMr = (forceL + 2 * forceR) / 3;
                    HitInfo hitMr = new HitInfo(this, playerId, angle, forceMr);

                    int comparisonResult = hitMl.CompareTo(hitMr) * (playerId == _playerId ? -1 : 1);
                    if (comparisonResult <= 0)
                        forceL = forceMl;
                    if (comparisonResult >= 0)
                        forceR = forceMr;
                }

                HitInfo newHit = new HitInfo(this, playerId, angle, (forceL + forceR) / 2);
                if (newHit.CompareTo(bestHit) < 0 && (playerId == _playerId || Random.value < 0.9))
                    bestHit = newHit;
            }
        }

        _gameLogic.HitBall(bestHit.PlayerId, bestHit.HitAngle, bestHit.HitForce);
    }


    public Vector3 EmulateHit(int ballOwnerId, float angle, float forceFrac)
    {
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            _playerBalls[i].Body.transform.position = _gameLogic.PlayerBalls[i].Body.transform.position;
            _playerBalls[i].Body.position = _gameLogic.PlayerBalls[i].Body.position;
        }

        var ballBody = _playerBalls[ballOwnerId].Body;
        var forceVec = Vector3.forward * (GameLogic.MaxStrokeForce * forceFrac);
        ballBody.AddForce(Quaternion.Euler(0, angle, 0) * forceVec, ForceMode.Impulse);
        var physicsScene = SimulationScene.GetPhysicsScene();
        for (int nSteps = 0; nSteps < 300 && _playerBalls.Any(ball => !ball.Body.IsSleeping()); ++nSteps)
            physicsScene.Simulate(GameLogic.FrameDeltaTime);

        if (_playerBalls.Any(ball => !ball.Body.IsSleeping())) // most likely, out of bounds
            return new Vector3(_gameLogic.NumberOfPlayers, -1, float.NaN);

        var waypointIndex = _playerBalls[_playerId].WaypointIndex;
        var distance = Vector3.Distance(_playerBalls[_playerId].Body.position, _playerBalls[_playerId].GoalHint);
        var place = 0;

        for (int i = 0; i < _gameLogic.NumberOfPlayers; ++i)
        {
            if (i == _playerId)
                continue;

            var rivalWaypointIndex = _playerBalls[i].WaypointIndex;
            var rivalDistance = Vector3.Distance(_playerBalls[i].Body.position, _playerBalls[i].GoalHint);
            if (rivalWaypointIndex == waypointIndex && rivalDistance < distance || rivalWaypointIndex > waypointIndex)
                ++place;
        }

        return new Vector3(place, -waypointIndex, distance);
    }
}