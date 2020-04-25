using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeAngleIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StrokeManager = GameObject.FindObjectOfType<StrokeManager>();
    }

    StrokeManager StrokeManager;
    
    // Update is called once per frame
    void Update()
    {
        Transform playerBallTransform = StrokeManager.GetPlayerBall().transform;
        this.transform.position = playerBallTransform.position;
        this.transform.rotation = Quaternion.Euler(0, StrokeManager.StrokeAngle, 0);
    }
}
