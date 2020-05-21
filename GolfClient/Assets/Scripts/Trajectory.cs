using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trajectory
{
    public Trajectory(Frame[] frames, float deltaTime)
    {
        this.Frames.AddRange(frames);
        this.DeltaTime = deltaTime;
    }

    public Trajectory(float deltaTime)
    {
        this.DeltaTime = deltaTime;
    }

    public void AddFrame(Frame frame)
    {
        Frames.Add(frame);
    }

    public List<Frame> Frames = new List<Frame>();
    public float DeltaTime;
}


public class Frame
{
    public Frame(BallStatus[] ballStatuses)
    {
        BallStatuses = ballStatuses;
    }

    public static Frame Extract(PlayerBall[] playerBalls)
    {
        return new Frame(playerBalls.Select(BallStatus.Extract).ToArray());
    }

    public BallStatus[] BallStatuses;
}

public class BallStatus
{
    public Vector3 Position;
    public Quaternion Rotation;

    public BallStatus(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public static BallStatus Extract(PlayerBall playerBall)
    {
        var transform = playerBall.transform;
        return new BallStatus(transform.position, transform.rotation);
    }
}

