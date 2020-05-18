using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory
{
    public Trajectory(Frame[] frames, float deltaTime)
    {
        Frames = frames;
        this.deltaTime = deltaTime;
    }

    public Frame[] Frames;
    public float deltaTime;
}




