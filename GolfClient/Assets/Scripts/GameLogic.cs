using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic
{
    public int CurrentPlayer;
    private Scene _scene;
    private PlayerBall[] _playerBalls;
    private GameObject _gameObject;
    
    public GameLogic(Scene scene)
    {
        _scene = scene;
        _gameObject = _scene.GetRootGameObjects()[0];
    }

    // Trajectory HitBall(int playerId, Vector2 direction, float force)
    // {
    //     if (playerId != CurrentPlayer)
    //     {
    //         throw new ArgumentException("FOOBAR");
    //     }
    //     // TODO: check MaxForce
    //     
    // }

}
