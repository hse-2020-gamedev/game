using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeAngleIndicator
{
    private PlayerBall _playerBall;
    private GameObject _object;
    
    public StrokeAngleIndicator(PlayerBall playerBall)
    {
        _playerBall = playerBall;
        var prefab = (GameObject)Resources.Load("StrokeAngleIndicator", typeof(GameObject));
        Debug.Assert(prefab != null);
        _object = GameObject.Instantiate(prefab);
    }

    public void SetAngle(float strokeAngle)
    {
        Transform playerBallTransform = _playerBall.transform;
        _object.transform.position = playerBallTransform.position;
        _object.transform.rotation = Quaternion.Euler(0, strokeAngle, 0);
    }

    public void Destroy()
    {
        GameObject.Destroy(_object);
    }
}
