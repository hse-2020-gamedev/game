using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager2 : MonoBehaviour
{
    private IServer _server;
    
    public GameLoopManager2()
    {
        // TODO: real constructor
        _server = new LocalServer();
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
