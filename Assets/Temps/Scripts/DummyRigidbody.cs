using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRigidbody : MonoBehaviour
{
    public Rigidbody body;
    public Vector3 relativePoint;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public float speed;

    private void Update()
    {
        velocity = body.velocity;
        speed = velocity.magnitude;
        angularVelocity = body.angularVelocity;
    }
}
