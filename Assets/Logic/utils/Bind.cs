using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bind : MonoBehaviour
{
    public GameObject go;

    void Update()
    {
        gameObject.transform.position = go.transform.position;
    }
}
