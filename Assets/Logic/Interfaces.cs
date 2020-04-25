using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface SurfaceI
{
    // mostly need for AI
    float HeightAt(Vector2 xy);

    // from 0 to 1
    float SpeedReductionCoefAt(Vector2 xy);
}

public interface StateI
{
    Vector3 MyBall();
    Vector3 EnemyBall();
    Vector3 GatePosition();
    SurfaceI Field();
}


public interface BallI
{
    Vector3 Position { get; }
    Vector3 Velocity { get; set; }
}


public enum PlayerState
{
    WAIT,
    ACTIVE,
    READY
}

public interface PlayerI
{
    void Turn(StateI state);

    PlayerState GetState();

    Vector3 GetResult();
}


