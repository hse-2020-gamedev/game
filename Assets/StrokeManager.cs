using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindPlayerBall();
    }

    public float StrokeAngle {get; protected set;}
    
    public float StrokeForce { get; protected set; }
    public float StrokeForcePerc { get { return StrokeForce / MaxStrokeForce; } }
    float MaxStrokeForce = 10f;
    
    float strokeForceFillSpeed = 5f;
    int fillDir = 1;
    
    public enum StrokeModeEnum {
        AIMING,
        FILLING,
        DO_WHACK,
        BALL_IS_ROLLING
    };

    public StrokeModeEnum StrokeMode { get; protected set; }

    Rigidbody playerBallRB;
                         

    private void FindPlayerBall() {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) {
            Debug.LogError("Could't find the ball");
        }

        playerBallRB = go.GetComponent<Rigidbody>();

        if (playerBallRB == null) {
            Debug.LogError("Ball doesn't have a RB!?");
        }
    }

    private void Update() {
        if (StrokeMode == StrokeModeEnum.AIMING)
        {
            StrokeAngle += Input.GetAxis("Horizontal") * 100f * Time.deltaTime;

            if (Input.GetButtonUp("Fire"))
            {
                StrokeMode = StrokeModeEnum.FILLING;
                return;
            }
        }

        if(StrokeMode == StrokeModeEnum.FILLING)
        {
            StrokeForce += (strokeForceFillSpeed * fillDir) * Time.deltaTime;
            if(StrokeForce > MaxStrokeForce)
            {
                StrokeForce = MaxStrokeForce;
                fillDir = -1;
            }
            else if (StrokeForce < 0)
            {
                StrokeForce = 0;
                fillDir = 1;
            }

            if (Input.GetButtonUp("Fire"))
            {
                StrokeMode = StrokeModeEnum.DO_WHACK;
            }

        }
    }

    void CheckRollingStatus()
    {
        // Is the ball still rolling?
        if(playerBallRB.IsSleeping())
        {
            StrokeMode = StrokeModeEnum.AIMING;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerBallRB == null) {
            return;
        }

        if(StrokeMode == StrokeModeEnum.BALL_IS_ROLLING)
        {
            CheckRollingStatus();
            return;
        }

        if (StrokeMode != StrokeModeEnum.DO_WHACK)
        {
            return;
        }

        Vector3 forceVec = new Vector3(0, 0, -StrokeForce);
        playerBallRB.AddForce(Quaternion.Euler(0, StrokeAngle, 0) * forceVec, ForceMode.Impulse);

        StrokeForce = 0;
        fillDir = 1;

        StrokeMode = StrokeModeEnum.BALL_IS_ROLLING;   
    }
}
