using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionSelectorScript : MonoBehaviour
{
    public float DeltaRot = 2;
    public GameObject Arrow;
    private Vector3 unit = new Vector3(0, 0, 1);

    public Vector3 GetDirection()
    {
        return gameObject.transform.rotation * unit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Rotate(0, -DeltaRot, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Rotate(0, DeltaRot, 0);
        }
    }
}
