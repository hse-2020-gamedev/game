
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = System.Random;

public interface IGameAI
{
    void MakeTurn();
}

public class DummyAI : IGameAI
{
    private volatile bool _makeTurnAfterLoading = false;
    private GameLogic _gameLogic;
    private int _playerId;
    public Scene SimulationScene;
    private volatile PlayerBall[] _playerBalls;

    public DummyAI(GameLogic gameLogic, int playerId)
    {
        _gameLogic = gameLogic;
        _playerId = playerId;

        _gameLogic.SceneLoader.LoadSimulationScene(OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene)
    {
        SimulationScene = scene;
        var rootGameObject = SimulationScene.GetRootGameObjects()[0];

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
        {
            MakeTurn();
        }
    }

    public void MakeTurn()
    {
        if (_playerBalls == null)
        {
            _makeTurnAfterLoading = true;
            return;
        }
        
        var ballPosition = _gameLogic.PlayerBalls[_playerId].transform.position;
        var targetPosition = _gameLogic.TargetHole.transform.position;

        float targetHoleDirection = ballPosition.xz().LookAt(_gameLogic.PlayerBalls[_playerId].GoalHint.xz());
        float distanceToTarget = Vector3.Distance(ballPosition, _gameLogic.PlayerBalls[_playerId].GoalHint);
        float distanceToHole = Vector3.Distance(ballPosition, targetPosition);

        if (distanceToHole < 0.1f) 
        { 
            Debug.Log("DummyAI: UPDATE! " + distanceToTarget + " -- really slight shot");
            _gameLogic.HitBall(_playerId, targetHoleDirection, 0.01f);
            return;
        }
        bool closeEnough = distanceToTarget < 4;
        float minAngle = targetHoleDirection - 60;
        float maxAngle = targetHoleDirection + 60;
        float angleStep = closeEnough ? 10f : 30f;
        float minForce = closeEnough ? 0.05f : 0.3f;
        float maxForce = 0.9f;
        float forceStep = closeEnough ? 0.05f : 0.1f;
        float bestAngle = minAngle;
        float bestForce = minForce;
        Tuple<int, float> bestOption = EmulateHit(_playerId, minAngle, minForce);
        Random random = new Random();
        for (float angle = minAngle; angle <= maxAngle; angle += angleStep)
        {
            for (float force = minForce; force <= maxForce; force += forceStep)
            {
                if (Math.Abs(angle - minAngle) < 1e-6 && Math.Abs(force - minForce) < 1e-6) continue;
                if (random.NextDouble() > 0.75) continue;
                Tuple<int, float> option = EmulateHit(_playerId, angle, force);
                if (option.Item1 > bestOption.Item1 || 
                    (option.Item1 == bestOption.Item1 && option.Item2 < bestOption.Item2))
                {
                    bestOption = option;
                    bestAngle = angle;
                    bestForce = force;
                }
            }
        }
        Debug.Log("DummyAI: UPDATE! " + distanceToTarget + " --> " + bestOption.Item2 + ", checkpoint = " + bestOption.Item1 + ", angle = " + bestAngle + ", F = " + bestForce);
        _gameLogic.HitBall(_playerId, bestAngle, bestForce);
    }
    
    
    public Tuple<int, float> EmulateHit(int playerId, float angle, float forceFrac)
    {
        
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            _playerBalls[i].Body.transform.position = _gameLogic.PlayerBalls[i].Body.transform.position;
            _playerBalls[i].Body.position = _gameLogic.PlayerBalls[i].Body.position;
            
        }
        var ballBody = _playerBalls[playerId].Body;
        var forceVec = Vector3.forward * (GameLogic.MaxStrokeForce * forceFrac);
        ballBody.AddForce(Quaternion.Euler(0, angle, 0) * forceVec, ForceMode.Impulse);
        var physicsScene = SimulationScene.GetPhysicsScene();
        int nSteps = 0;
        int SleepFrame = 0;
        while (SleepFrame < 50 && nSteps < 500) {
            if (_playerBalls.All(ball => ball.Body.IsSleeping())) {
                SleepFrame += 1;
            } else {
                SleepFrame = 0;
            }
            physicsScene.Simulate(GameLogic.FrameDeltaTime);
            nSteps++;
        }
        //var position = _gameLogic.TargetHole.transform.position;
        var goalHintPosition = _playerBalls[_playerId].GoalHint;
        return new Tuple<int, float>(_playerBalls[_playerId].WaypointIndex, Vector3.Distance(ballBody.position, goalHintPosition));
        //(_playerBalls[_playerId].WaypointIndex, Vector3.Distance(ballBody.position, goalHintPosition));
    }
}
