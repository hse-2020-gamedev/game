using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyState : IState
{
    private readonly IBall enemyBall;
    private readonly IBall myBall;
    private readonly Vector3 gate;
    private readonly ISurface surface;

    public DummyState(IBall myBall, IBall enemyBall, Vector3 gate)
    {
        this.myBall = myBall;
        this.enemyBall = enemyBall;
        this.gate = gate;
        this.surface = new DummySurface();
    }

    public Vector3 EnemyBall()
    {
        return enemyBall.Position;
    }

    public ISurface Field()
    {
        return surface;
    }

    public Vector3 GatePosition()
    {
        return gate;
    }

    public Vector3 MyBall()
    {
        return myBall.Position;
    }
}
