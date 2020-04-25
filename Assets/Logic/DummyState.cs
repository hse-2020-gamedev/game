using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyState : StateI
{
    private readonly BallI enemyBall;
    private readonly BallI myBall;
    private readonly Vector3 gate;
    private readonly SurfaceI surface;

    public DummyState(BallI myBall, BallI enemyBall, Vector3 gate)
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

    public SurfaceI Field()
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
