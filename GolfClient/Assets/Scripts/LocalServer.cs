
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalServer : IServer
{
    private GameLogic _gameLogic;
    
    public LocalServer(GameSettings gameSettings)
    {
        _gameLogic = new GameLogic(gameSettings);
    }
    
    public void HitBall(int playerId, float angle, float force)
    {
        Debug.Log($"Hit ball {playerId} with angle {angle} and force {force}");
        _gameLogic.HitBall(playerId, angle, force);
    }

    public void LeaveGame()
    {
        throw new NotImplementedException();
    }

    public void NextMove()
    {
        _gameLogic.NextMove();
    }
    
    public Event NextEvent()
    {
        // _gameLogic.Update();
        // return null;
        _gameLogic.Events.TryDequeue(out var result);
        return result;
    }

    // public static float MaxForce = 10;
    //
    // public int CurrentPlayer = 0;
    //
    // private Player[] _players;
    // private Scene _scene;
    // private Vector3?[] _deferredImpulse;
    //
    // public Task<Trajectory> HitBall(Vector2 direction, float force)
    // {
    //     // TODO: compute in separate thread
    //     var impulse2d = direction * force;
    //     // TODO: use normal vector of the surface?
    //     _players[CurrentPlayer].Ball.AddForce(impulse2d.x, 0, impulse2d.y);
    //     
    // }
    //
    // public Task LeaveGame()
    // {
    //     // throw new System.NotImplementedException();
    // }
}
