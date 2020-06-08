using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Body.sleepThreshold = 1f; // Default is 0.005
    }

    public Rigidbody Body => GetComponent<Rigidbody>();
    
    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
