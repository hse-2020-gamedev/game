using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicalWorld
{
    public static float MaxForce = 10;

    public int CurrentPlayer = 0;

    private Player[] _players;
    private Scene _scene;
    private Vector3?[] _deferredImpulse;

    public void HitBall(int player, Vector2 direction, float force)
    {
        if (player != CurrentPlayer)
        {
            throw new ArgumentException(
                $"Wrong move sequence. CurrentPlayer is {CurrentPlayer}, player {player} attempted to move.");
        }

        if (force < 0 || force > MaxForce)
        {
            throw new ArgumentException(
                $"Invalid force value: {force}. Must be between {0} and {MaxForce}.");
        }

        // TODO: Use normal vector of the surface.
        var impulse = direction.normalized * force;
        _deferredImpulse[player] = new Vector3(impulse.x, 0, impulse.y);
    }

    // Should be called from the Start method of the scene.
    public void Start()
    {
        Physics.autoSimulation = false;
    }
    
    // Should be called from the Update method of the scene.
    public void Update(float deltaTime)
    {
        for (int i = 0; i < _deferredImpulse.Length; ++i)
        {
            if (_deferredImpulse[i] != null)
            {
                _players[i].Ball.AddForce(_deferredImpulse[i].Value);
                _deferredImpulse[i] = null;
            }
        }
        _scene.GetPhysicsScene().Simulate(deltaTime);
    }
}
