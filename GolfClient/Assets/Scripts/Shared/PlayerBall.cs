using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Body = GetComponent<Rigidbody>(); 
        Body.sleepThreshold = 0.2f; // Default is 0.005
    }

    public Rigidbody Body { get; private set; }
    
    public int getLayerId() {
        return 0;//layer;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
