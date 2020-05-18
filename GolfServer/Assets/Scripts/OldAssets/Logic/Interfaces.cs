using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISurface
{
    // mostly need for AI
    float HeightAt(Vector2 xy);

    // from 0 to 1
    float SpeedReductionCoefAt(Vector2 xy);
}

public interface IState
{
    Vector3 MyBall();
    Vector3 EnemyBall();
    Vector3 GatePosition();
    ISurface Field();
}

public interface IBall
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

public interface IPlayer
{
    void Turn(IState state);

    PlayerState GetState();

    Vector3 GetResult();
}


