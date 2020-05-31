using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoopManager : MonoBehaviour
{
    private IServer _server;
    private Status _status;
    private CameraPositionManager _cameraPositionManager;
    private PlayerBall[] _playerBalls;
    private int[] _localPlayerIds;
    private GameObject _scorePanel;
    private GameObject _finalText;
    public float ForceImageFillAmount;
    private float strokeAngle;
    
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
            public LocalPlayerMoving(int playerId, PlayerBall playerBall, float strokeAngle)
            {
                PlayerId = playerId;
                Manager = new StrokeManager(playerBall, strokeAngle);
            }

            public readonly StrokeManager Manager;   
            public readonly int PlayerId;
        }

        public class Finished : Status 
        {
            public Finished(string message) {
                msg = message;
            }
            public readonly string msg;

        }
        // WaitingOtherPlayers
    }
    
    // Start is called before the first frame update
    internal void Start()
    {
        _playerBalls = FindObjectsOfType<PlayerBall>();
        _scorePanel = GameObject.Find("/RootObject/Canvas/ScorePanel");
        _finalText = GameObject.Find("/RootObject/Canvas/ScorePanel/Text");
        Debug.Log(_scorePanel);
        _cameraPositionManager = new CameraPositionManager(_playerBalls[0]);
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
                    _status = new Status.LocalPlayerMoving(movingPlayerId, _playerBalls[movingPlayerId], strokeAngle);
                }
                else
                {
                    // Waiting for remote player to make turn.
                    _status = new Status.WaitingEvents();
                }
            } else if (ev is Event.Finish finishEvent) {
                _status = new Status.Finished(finishEvent.Message);
            } else {
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
            _cameraPositionManager.UpdateCameraPosition(moving.Manager.StrokeAngle);
            strokeAngle = moving.Manager.StrokeAngle;
            ForceImageFillAmount = moving.Manager.StrokeForcePerc;
            if (moving.Manager.Done)
            {
                _server.HitBall(moving.PlayerId, moving.Manager.StrokeAngle, moving.Manager.StrokeForcePerc);
                _status = new Status.WaitingEvents();
            }
        } else if (_status is Status.Finished finished) {
            _finalText.GetComponent<Text>().text = finished.msg;
            _scorePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
