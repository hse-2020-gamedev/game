using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeManager
{
    public StrokeManager(PlayerBall playerBall)
    {
        _strokeAngleIndicator = new StrokeAngleIndicator(playerBall);
        // TODO: perhaps, it would be better to start looking in some sensible direction.
        StrokeAngle = 0;
        _strokeAngleIndicator.SetAngle(StrokeAngle);

        // this._strokeForceUI = new StrokeForceUI();
    }

    public float StrokeAngle { get; private set; }
    
    public float StrokeForce { get; private set; }

    private readonly StrokeAngleIndicator _strokeAngleIndicator;
    // private StrokeForceUI _strokeForceUI;

    // float strokeForceFillSpeed = 0.5f;
    // int fillDirection = 1;

    private enum StrokeStatusEnum {
        Aiming,
        // Filling,
        // DoWhack,
        Done
    };

    private StrokeStatusEnum _strokeStatus;

    public bool Done => _strokeStatus == StrokeStatusEnum.Done;

    // Update is called once per visual frame -- use this for inputs
    public void Update()
    {
        if (_strokeStatus == StrokeStatusEnum.Aiming)
        {
            StrokeAngle += Input.GetAxis("Horizontal") * 100f * Time.deltaTime;
            _strokeAngleIndicator.SetAngle(StrokeAngle);
            if (Input.GetButtonUp("Fire"))
            {
                _strokeAngleIndicator.Destroy();
                StrokeForce = 0.5f;
                // StrokeStatus = StrokeStatusEnum.FILLING;
                _strokeStatus = StrokeStatusEnum.Done;
                return;
            }
        }
        
        
    
        // if(StrokeStatus == StrokeStatusEnum.FILLING)
        // {
        //     StrokeForce += (strokeForceFillSpeed * fillDir) * Time.deltaTime;
        //     if(StrokeForce > MaxStrokeForce)
        //     {
        //         StrokeForce = MaxStrokeForce;
        //         fillDir = -1;
        //     }
        //     else if (StrokeForce < 0)
        //     {
        //         StrokeForce = 0;
        //         fillDir = 1;
        //     }
        //
        //     if (Input.GetButtonUp("Fire"))
        //     {
        //         StrokeStatus = StrokeStatusEnum.DO_WHACK;
        //     }
        //
        // }
    
    
    }
    
    // void FixedUpdate()
    // {
    //     if (playerBallRB == null)
    //     {
    //         // Might not be an error -- maybe the ball fell out of bounds, got deleted,
    //         // and hasn't respawned yet.
    //         return;
    //     }
    //
    //     if(StrokeStatus == StrokeStatusEnum.BALL_IS_ROLLING)
    //     {
    //         CheckRollingStatus();
    //         return;
    //     }
    //
    //     if (StrokeStatus != StrokeStatusEnum.DO_WHACK)
    //     {
    //         return;
    //     }
    //
    //     // Whackadaball
    //
    //     Debug.Log("Whacking it!");
    //
    //     Vector3 forceVec = new Vector3(0, 0, StrokeForce);
    //
    //     playerBallRB.AddForce(Quaternion.Euler(0, StrokeAngle, 0) * forceVec, ForceMode.Impulse);
    //
    //     StrokeForce = 0;
    //     fillDir = 1;
    //     StrokeCount++;
    //
    //     StrokeStatus = StrokeStatusEnum.BALL_IS_ROLLING;
    // }
}
