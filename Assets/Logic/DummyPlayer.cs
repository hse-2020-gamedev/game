﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : PlayerI
{
    Vector3 result;
    PlayerState state = PlayerState.WAIT;

    public void Turn(StateI gameState)
    {
        result = 3 * Vector3.Normalize(gameState.GatePosition() - gameState.MyBall());
        state = PlayerState.READY;
    }

    public PlayerState GetState() => state;

    public Vector3 GetResult()
    {
        state = PlayerState.WAIT;
        return result;
    }
}
