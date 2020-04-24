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

    Rigidbody playerBallRB;

    bool doWhack = false;

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
        if (Input.GetKeyUp(KeyCode.Space)) {
            doWhack = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerBallRB == null) {
            return;
        }

        if (doWhack) {
             doWhack = false;    
             Vector3 forceVec = new Vector3(0, 0, 1f);

             playerBallRB.AddForce(forceVec, ForceMode.Impulse);
        }     
    }
}
