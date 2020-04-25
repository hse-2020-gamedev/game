using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Ball : BallI
{
    GameObject go;
    public Ball(GameObject go)
    {
        this.go = go;
    }

    public Vector3 Position => go.GetComponent<Rigidbody>().position;

    public Vector3 Velocity { 
        get => go.GetComponent<Rigidbody>().velocity;
        set => go.GetComponent<Rigidbody>().velocity = value;
    }
}

public class Loop : MonoBehaviour
{
    public PlayerI Player0;
    public BallI Ball0;

    public PlayerScript Player;
    public PlayerI Player1;
    public BallI Ball1;

    public GameObject gate;
    public SurfaceI surface;
    private int turn = 0;

    private float minSpeed = 0.04F;

    public GameObject winModal;

    public bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        Player0 = new DummyPlayer();
        Ball0 = new Ball(GameObject.Find("EnemyBall"));

        Player1 = Player;
        Ball1 = new Ball(GameObject.Find("PlayerBall"));

    }

    bool shouldSimulate()
    {
        var ball0Stays = Ball0.Velocity.magnitude > minSpeed;
        var ball1Stays = Ball1.Velocity.magnitude > minSpeed;
        return ball0Stays || ball1Stays;
    }

    public (PlayerI, BallI) nextPlayer()
    {
        turn = (turn + 1) % 2;
        if (turn == 0)
        {
            return (Player0, Ball0);
        }
        else
        {
            return (Player1, Ball1);
        }
    }

    StateI getState()
    {
        if (turn == 0) {
            return new DummyState(Ball0, Ball1, gate.transform.position);
        }
        else
        {
            return new DummyState(Ball1, Ball0, gate.transform.position);
        }
    }

    void correction()
    {
        if (Ball0.Velocity.magnitude < minSpeed)
        {
            Ball0.Velocity = Vector3.zero;
        }
        if (Ball1.Velocity.magnitude < minSpeed)
        {
            Ball1.Velocity = Vector3.zero;
        }
    }

    PlayerI currentPlayer;
    BallI currentBall;

    void makeTurn()
    {
        if (currentPlayer == null)
        {
            (currentPlayer, currentBall) = nextPlayer();
        }

        if (currentPlayer.GetState() == PlayerState.WAIT)
        {
            currentPlayer.Turn(getState());
        }
        else if (currentPlayer.GetState() == PlayerState.ACTIVE)
        {
            return;
        }
        else if (currentPlayer.GetState() == PlayerState.READY)
        {
            currentBall.Velocity = currentPlayer.GetResult();
            (currentPlayer, currentBall) = nextPlayer();
        }
    }

    void simulate()
    {
        // pass
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            winModal.SetActive(true);
            winModal.GetComponent<ShowWinner>().SetWinner(turn == 0 ? "player 1" : "player 2");
            enabled = false;
        }
        else
        {
            correction();
            if (shouldSimulate())
            {
                simulate();
            }
            else
            {
                makeTurn();
            }
        }
    }
}
