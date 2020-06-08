using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class CameraPositionManager {
    private PlayerBall[] _playerBalls;
    private int followedBall;
    private Vector3 followedBallPos;
   
    void SetCameraOverview() {
        GameObject mainCam = GameObject.Find("Main Camera");
        GameObject levelPlane = GameObject.Find("level0").transform.Find("Wall").gameObject;
        var PlaneBounds = levelPlane.GetComponent<Renderer>().bounds;
        var max_len = Math.Max(PlaneBounds.size[0], PlaneBounds.size[2])/2;
        var FieldOfView = (Camera.main.fieldOfView/2)*Mathf.PI/180;
        var ycam = max_len*Mathf.Cos(FieldOfView)/Mathf.Sin(FieldOfView) * 1.1f;
        mainCam.transform.position = new Vector3((PlaneBounds.min[0] + PlaneBounds.max[0])/2, ycam, (PlaneBounds.min[2] + PlaneBounds.max[2])/2);
        mainCam.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    public CameraPositionManager(PlayerBall[] playerBalls) {
        _playerBalls = playerBalls; 
        SetCameraOverview();
    }

    void ThirdPersonView(int playerId, float strokeAngle)
    {
        GameObject mainCam = GameObject.Find("Main Camera");
        var rot = mainCam.transform.eulerAngles.y;
        rot = strokeAngle;
        mainCam.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Transform playerBallTransform = _playerBalls[playerId].transform;
        mainCam.transform.position = playerBallTransform.position + new Vector3(-Mathf.Sin(rot * Mathf.PI / 180) * 0.5f, 0.2f, -Mathf.Cos(rot * Mathf.PI / 180) * 0.5f);
        mainCam.transform.eulerAngles = new Vector2(0, rot);
    }

    public void ViewBall(int playerId, float fixedAngle)
    {
        ThirdPersonView(playerId, fixedAngle);
    }

    public void FollowBall(int playerId)
    { // use ball transform
        if (followedBall != playerId)
        {
            followedBall = playerId;
            followedBallPos = _playerBalls[playerId].transform.position;
            ThirdPersonView(playerId, 0);
        }
        else
        {
            ThirdPersonView(playerId, Vector3.Angle(_playerBalls[playerId].transform.position - followedBallPos, Vector3.forward));
            followedBallPos = _playerBalls[playerId].transform.position;
        }
    }
}
