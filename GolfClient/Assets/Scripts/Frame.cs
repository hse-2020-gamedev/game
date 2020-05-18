using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame
{
    public Frame(Vector3[] ballPositions)
    {
        BallPositions = ballPositions;
    }

    public Vector3[] BallPositions;
}
