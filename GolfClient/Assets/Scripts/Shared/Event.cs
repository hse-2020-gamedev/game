public abstract class Event
{
    private Event() {}

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

    public sealed class TurnOfPlayer : Event
    {
        public TurnOfPlayer(int playerId)
        {
            this.playerId = playerId;
        }

        public int playerId;
    }

    public sealed class Finish : Event
    {
        public string Message;
        public Finish(string msg) {
            this.Message = msg;
        }
    }
}
