using System;

[Serializable]
public abstract class Event
{
    private Event() {}

    [Serializable]
    public sealed class PlayTrajectory : Event
    {
        public PlayTrajectory(Trajectory trajectory, int ballToFollow)
        {
            BallToFollow = ballToFollow;
            Trajectory = trajectory;
        }

        public int BallToFollow;
        public Trajectory Trajectory;
    }

    [Serializable]
    public sealed class LocalPlayerId : Event
    {
        public LocalPlayerId(int playerId)
        {
            this.playerId = playerId;
        }

        public int playerId;
    }

    [Serializable]
    public sealed class TurnOfPlayer : Event
    {
        public TurnOfPlayer(int playerId)
        {
            this.playerId = playerId;
        }

        public int playerId;
    }

    [Serializable]
    public sealed class Finish : Event
    {
        public string Message;
        public Finish(string msg) {
            this.Message = msg;
        }
    }
}
