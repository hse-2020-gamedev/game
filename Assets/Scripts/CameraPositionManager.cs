﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionManager : MonoBehaviour {
    StrokeManager StrokeManager;
   
    void SetCameraOverview() {
        GameObject mainCam = GameObject.Find("Main Camera");
        GameObject levelPlane = GameObject.Find("level0").transform.Find("Plane").gameObject;
        var PlaneBounds = levelPlane.GetComponent<Renderer>().bounds;
        var max_len = Math.Max(PlaneBounds.size[0], PlaneBounds.size[2])/2;
        var FieldOfView = (Camera.main.fieldOfView/2)*Mathf.PI/180;
        var ycam = max_len*Mathf.Cos(FieldOfView)/Mathf.Sin(FieldOfView) * 1.1f;
        mainCam.transform.position = new Vector3((PlaneBounds.min[0] + PlaneBounds.max[0])/2, ycam, (PlaneBounds.min[2] + PlaneBounds.max[2])/2);
        mainCam.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    void SetThirdPersonCamera() {
         GameObject mainCam = GameObject.Find("Main Camera");
         var rot = mainCam.transform.eulerAngles.y;
         mainCam.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);  
         Transform playerBallTransform = StrokeManager.GetPlayerBall().transform;
         mainCam.transform.position = playerBallTransform.position + new Vector3(0.1f, 0.2f, -0.5f); 
    }

    // Start is called before the first frame update
    void Start() {
         StrokeManager = GameObject.FindObjectOfType<StrokeManager>();
         SetCameraOverview();
    }

    // Update is called once per frame
    void Update() {
        SetThirdPersonCamera();
        
    }
}
