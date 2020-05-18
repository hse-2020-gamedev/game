using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    private IServer _server;
    private Status _status;
    private PlayerBall[] _playerBalls;

    private abstract class Status
    {
        private Status() { }

        public class WaitingEvents : Status { }
        
        public class BallIsRolling : Status
        {
            public BallIsRolling(Trajectory traj)
            {
                Traj = traj;
            }

            public Trajectory Traj;
            public int CurrentFrame;
        }
        
        
        // Aiming,
        // Filling,
        // DoWhack,
        // BallIsRolling,
        // Finished,
        // WaitingOtherPlayers
    }
    
    // Start is called before the first frame update
    internal void Start()
    {
        _server = new LocalServer();
        Physics.autoSimulation = false;
        _status = new Status.WaitingEvents();
        // TODO: make deterministic.
        _playerBalls = GameObject.FindObjectsOfType<PlayerBall>();
    }

    // Update is called once per frame
    internal void Update()
    {
        if (_status is Status.WaitingEvents)
        {
            Debug.Log("Waiting for events");
            var ev = _server.NextEvent();
            if (ev is Event.PlayTrajectory playTrajectoryEvent)
            {
                Debug.Log("Received event");
                _status = new Status.BallIsRolling(playTrajectoryEvent.Trajectory);
            }
            else
            {
                throw new ArgumentException("Not implemented");
            }
        }
        else if (_status is Status.BallIsRolling rolling)
        {
            var trajectory = rolling.Traj;
            if (rolling.CurrentFrame >= trajectory.Frames.Length)
            {
                _status = new Status.WaitingEvents();
            }
            else
            {
                var frame = trajectory.Frames[rolling.CurrentFrame];
                // Assert.Equals(_playerBalls.Length, frame.BallPositions.Length);
                for (int i = 0; i < _playerBalls.Length; ++i)
                {
                    _playerBalls[i].gameObject.transform.position = frame.BallPositions[i];
                }
                ++rolling.CurrentFrame;
            }
        }
    }
}
