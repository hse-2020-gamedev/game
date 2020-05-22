using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
   
    float strokeForceFillSpeed = 0.5f;
    int fillDirection = 1;
    public float MaxStrokeForce = 1.0f;
    
    public float StrokeForcePerc { get { return StrokeForce / MaxStrokeForce; } }

    private enum StrokeStatusEnum {
        Aiming,
        Filling,
        Done
    };

    private StrokeStatusEnum _strokeStatus;

    public bool Done => _strokeStatus == StrokeStatusEnum.Done;

    // Update is called once per visual frame -- use this for inputs
    public void Update() {
        if (_strokeStatus == StrokeStatusEnum.Aiming) {
            StrokeAngle += Input.GetAxis("Horizontal") * 100f * Time.deltaTime;
            _strokeAngleIndicator.SetAngle(StrokeAngle);
            if (Input.GetButtonUp("Fire")) {
                _strokeAngleIndicator.Destroy();
                _strokeStatus = StrokeStatusEnum.Filling;
                return;
            }
        }
        
        
        if (_strokeStatus == StrokeStatusEnum.Filling) {
             StrokeForce += (strokeForceFillSpeed * fillDirection) * Time.deltaTime;
             if(StrokeForce > MaxStrokeForce) {
                 StrokeForce = MaxStrokeForce;
                 fillDirection = -1;
             } else if (StrokeForce < 0) {
                 StrokeForce = 0;
                 fillDirection = 1;
             }
            
             if (Input.GetButtonUp("Fire")) {
                 _strokeAngleIndicator.Destroy();
                 _strokeStatus = StrokeStatusEnum.Done;             
             }
        
        }
    
    
    }
}
