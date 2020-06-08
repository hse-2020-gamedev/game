public abstract class Event
{
    private Event() {}

    public class PlayTrajectory : Event
    {
        public PlayTrajectory(Trajectory trajectory, int ballToFollow)
        {
            BallToFollow = ballToFollow;
            Trajectory = trajectory;
        }

        public int BallToFollow;
        public Trajectory Trajectory;
    }

    public class TurnOfPlayer : Event
    {
        public TurnOfPlayer(int playerId)
        {
            this.playerId = playerId;
        }

        public int playerId;
    }

    public class Finish : Event
    {
        public string Message;
    }
    
}
