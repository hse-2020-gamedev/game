
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalServer : MonoBehaviour, IServer
{
    private Scene _scene;

    public void HitBall(int playerId, Vector2 direction, float force)
    {
        
    }

    public void LeaveGame()
    {
        throw new System.NotImplementedException();
    }

    public Event NextEvent()
    {
        var frames = new Frame[1000];
        for (int i = 0; i < frames.Length; ++i)
        {
            frames[i] = new Frame(new Vector3[2]);
            frames[i].BallPositions[0] = new Vector3(0.001f * i, 0, 0);
            frames[i].BallPositions[1] = new Vector3(-0.001f * i, 0, 0.2f);
        }
        return new Event.PlayTrajectory(new Trajectory(frames, 1 / 60f));
    }

    // public static float MaxForce = 10;
    //
    // public int CurrentPlayer = 0;
    //
    // private Player[] _players;
    // private Scene _scene;
    // private Vector3?[] _deferredImpulse;
    //
    // public Task<Trajectory> HitBall(Vector2 direction, float force)
    // {
    //     // TODO: compute in separate thread
    //     var impulse2d = direction * force;
    //     // TODO: use normal vector of the surface?
    //     _players[CurrentPlayer].Ball.AddForce(impulse2d.x, 0, impulse2d.y);
    //     
    // }
    //
    // public Task LeaveGame()
    // {
    //     // throw new System.NotImplementedException();
    // }
}
