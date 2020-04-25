using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject loop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        loop.GetComponent<Loop>().isGameOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
