using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public LoopScript Loop;

    private void OnTriggerEnter(Collider other)
    {
        Loop.isGameOver = true;
    }
}
