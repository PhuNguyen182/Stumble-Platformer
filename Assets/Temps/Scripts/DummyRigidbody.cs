using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRigidbody : MonoBehaviour
{
    public Rigidbody body;
    public Vector3 velocity;

    private void Update()
    {
        velocity = body.velocity;
    }
}
