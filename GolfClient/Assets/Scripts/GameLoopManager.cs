using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    private IServer _server;
    private Status _status;
    private PlayerBall[] _playerBalls;
    private int[] _localPlayerIds;
    
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

            public readonly Trajectory Traj;
            public int CurrentFrame;
        }

        public class LocalPlayerMoving : Status
        {
            public LocalPlayerMoving(int playerId, PlayerBall playerBall)
            {
                PlayerId = playerId;
                Manager = new StrokeManager(playerBall);
            }

            public readonly StrokeManager Manager;
            public readonly int PlayerId;
        }

        // Finished,
        // WaitingOtherPlayers
    }
    
    // Start is called before the first frame update
    internal void Start()
    {
        _playerBalls = FindObjectsOfType<PlayerBall>();
        var gameSettings = new GameSettings();
        gameSettings.SceneName = SceneManager.GetActiveScene().name;
        gameSettings.PlayerTypes = new[] {PlayerType.Human, PlayerType.DummyAI};
        _server = new LocalServer(gameSettings);
        _localPlayerIds = new[] { 0 };
        Physics.autoSimulation = false;
        _status = new Status.WaitingEvents();
        // TODO: make deterministic.
    }

    // Update is called once per frame
    internal void Update()
    {
        if (_status is Status.WaitingEvents)
        {
            Debug.Log("Waiting for events");
            var ev = _server.NextEvent();
            if (ev == null)
            {
                Debug.Log("No events yet.");
                return;
            }
        
            Debug.Log("Received event");
            if (ev is Event.PlayTrajectory playTrajectoryEvent)
            {
                _status = new Status.BallIsRolling(playTrajectoryEvent.Trajectory);
            }
            else if (ev is Event.TurnOfPlayer turnOfPlayerEvent)
            {
                int movingPlayerId = turnOfPlayerEvent.playerId;
                if (_localPlayerIds.Contains(movingPlayerId))
                {
                    _status = new Status.LocalPlayerMoving(movingPlayerId, _playerBalls[movingPlayerId]);
                }
                else
                {
                    // Waiting for remote player to make turn.
                    _status = new Status.WaitingEvents();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else if (_status is Status.BallIsRolling rolling)
        {
            var trajectory = rolling.Traj;
            if (rolling.CurrentFrame >= trajectory.Frames.Count)
            {
                _status = new Status.WaitingEvents();
            }
            else
            {
                var frame = trajectory.Frames[rolling.CurrentFrame];
                Debug.Assert(_playerBalls.Length == frame.BallStatuses.Length);
                for (int i = 0; i < _playerBalls.Length; ++i)
                {
                    var ballTransform = _playerBalls[i].gameObject.transform;
                    var ballStatus = frame.BallStatuses[i];
                    ballTransform.position = ballStatus.Position;
                    ballTransform.rotation = ballStatus.Rotation;
                }
                ++rolling.CurrentFrame;
            }
        }
        else if (_status is Status.LocalPlayerMoving moving)
        {
            moving.Manager.Update();
            if (moving.Manager.Done)
            {
                _server.HitBall(moving.PlayerId, moving.Manager.StrokeAngle, moving.Manager.StrokeForce);
                _status = new Status.WaitingEvents();
            }
        }
    }
}
