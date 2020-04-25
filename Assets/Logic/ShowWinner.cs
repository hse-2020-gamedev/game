using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinner : MonoBehaviour
{
    public UnityEngine.UI.Text text;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetWinner(string name)
    {
        text.text = $"{name} win";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
