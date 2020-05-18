using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameLoopManager LoopManager;

    private void OnTriggerEnter(Collider other)
    {
        LoopManager.isGameOver = true;
    }
}
