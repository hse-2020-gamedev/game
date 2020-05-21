using UnityEngine;
using System.Collections;

public class ChangeBallLayer : MonoBehaviour {
    // [Andrei]: I am not sure what this code is supposed to do, but it used to cause crashes.

    // public int LayerOnEnter; // BallInHole
    // public int LayerOnExit;  // BallOnTable
    //
    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "Player")
    //     {
    //         other.gameObject.layer = LayerOnEnter;
    //     }
    // }
    //
    // void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject.tag == "Player")
    //     {
    //         other.gameObject.layer = LayerOnExit;
    //     }
    // }
}