public abstract class Event
{
    private Event() {}

    public class PlayTrajectory : Event
    {
        public PlayTrajectory(Trajectory trajectory)
        {
            Trajectory = trajectory;
        }

        public Trajectory Trajectory;
    }

    public class TurnOfPlayer : Event
    {
        public int playerId;
    }

    public class Finish : Event
    {
        public string Message;
    }
    
}
