
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

        var sceneAsync = SceneManager.LoadSceneAsync(
            _gameLogic.GameSettings.SceneName, 
            new LoadSceneParameters(
                LoadSceneMode.Additive, 
                LocalPhysicsMode.Physics3D
            )
        );
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        sceneAsync.completed += _ => OnSceneLoaded(scene);
    }

    private void OnSceneLoaded(Scene scene)
    {
        Debug.Log("DummyAI got scene");

        SimulationScene = scene;
        var rootGameObject = SimulationScene.GetRootGameObjects()[0];

        GameLogic.MakeSceneInvisible(scene);

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

        float targetHoleDirection = ballPosition.xz().LookAt(targetPosition.xz());
        Debug.Log("ANGLE = " + targetHoleDirection);

        bool closeEnough = Vector3.Distance(ballPosition, targetPosition) < 4;
        float minAngle = targetHoleDirection - 90;
        float maxAngle = targetHoleDirection + 90;
        float angleStep = closeEnough ? 10f : 30f;
        float minForce = closeEnough ? 0.05f : 0.3f;
        float maxForce = 0.9f;
        float forceStep = closeEnough ? 0.05f : 0.1f;
        float bestAngle = minAngle;
        float bestForce = minForce;
        float bestDistance = EmulateHit(_playerId, minAngle, minForce);
        Random random = new Random();
        for (float angle = minAngle; angle <= maxAngle; angle += angleStep)
        {
            for (float force = minForce; force <= maxForce; force += forceStep)
            {
                if (Math.Abs(angle - minAngle) < 1e-6 && Math.Abs(force - minForce) < 1e-6) continue;
                float distance = EmulateHit(_playerId, angle, force);
                if (distance < bestDistance && random.NextDouble() < 0.75)
                {
                    Debug.Log("UPDATE! " + bestDistance + " " + bestAngle + " " + bestForce);
                    bestDistance = distance;
                    bestAngle = angle;
                    bestForce = force;
                }
            }
        }
        _gameLogic.HitBall(_playerId, bestAngle, bestForce);
    }
    
    
    public float EmulateHit(int playerId, float angle, float forceFrac)
    {
        
        for (int i = 0; i < _gameLogic.NumberOfPlayers; i++)
        {
            _playerBalls[i].Body.transform.position = _gameLogic.PlayerBalls[i].Body.transform.position;
            _playerBalls[i].Body.position = _gameLogic.PlayerBalls[i].Body.position;
            
        }
        var ballBody = _playerBalls[playerId].Body;
        Debug.Log("STARTED EMULATE HIT");
        Debug.Log("ballBody.position = " + ballBody.position);
        Debug.Log("ballBody.transform.position = " + ballBody.transform.position);
        Debug.Log("ballBody = " + ballBody);;
        var forceVec = Vector3.forward * (GameLogic.MaxStrokeForce * forceFrac);
        ballBody.AddForce(Quaternion.Euler(0, angle, 0) * forceVec, ForceMode.Impulse);
        var physicsScene = SimulationScene.GetPhysicsScene();
        int nSteps = 0;
        int SleepFrame = 0;
        while (SleepFrame < 50 && nSteps < 3000) {
            if (_playerBalls.All(ball => ball.Body.IsSleeping())) {
                SleepFrame += 1;
            } else {
                SleepFrame = 0;
            }
            physicsScene.Simulate(GameLogic.FrameDeltaTime);
            nSteps++;
        }
        Debug.Log("nSteps =" + nSteps);
        Debug.Log("DONE EMULATE HIT");
        Debug.Log("ballBody.position = " + ballBody.position);
        Debug.Log("ballBody.transform.position = " + ballBody.transform.position);
        Debug.Log("ballBody = " + ballBody);
        var position = _gameLogic.TargetHole.transform.position;
        Debug.Log("distance = " + Vector3.Distance(ballBody.position, position));
        return Vector3.Distance(ballBody.position, position);
    }
}
