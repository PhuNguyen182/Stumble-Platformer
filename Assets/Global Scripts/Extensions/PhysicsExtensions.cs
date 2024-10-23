using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.Extensions
{
    public static class PhysicsExtensions
    {
        public static void ClampVelocity(this Rigidbody rigidbody, float maxLength)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxLength);
        }

        public static float GetSquaredSpeed(this Rigidbody rigidbody)
        {
            return rigidbody.velocity.sqrMagnitude;
        }
    }
}
