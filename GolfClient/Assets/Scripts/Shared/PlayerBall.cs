using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerBall : MonoBehaviour
{
    private Collider _currentWaypoint;
    public Vector3 GoalHint => GetWaypointGoal(_currentWaypoint);
    public int WaypointIndex => GetWaypointIndex(_currentWaypoint);

    // Start is called before the first frame update
    public void Start()
    {
        Body.sleepThreshold = 1f; // Default is 0.005
    }

    public Rigidbody Body => GetComponent<Rigidbody>();
    
    public int getLayerId() {
        return gameObject.layer;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.name == "Waypoints")
            _currentWaypoint = other;
    }

    private Vector3 GetWaypointGoal(Collider col)
    {
        var child = col.transform.GetChild(0);
        Assert.AreEqual("WaypointGoal", child.name, "Got unexpected collider");
        return child.position;
    }

    private int GetWaypointIndex(Collider col)
    {
        return Int32.Parse(col.name);
    }
}
