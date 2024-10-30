using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.Extensions
{
    public static class PhysicsExtensions
    {
        public static void ClampVelocity(this Rigidbody rigidbody, float maxMagnitude)
        {
            if (rigidbody.velocity.sqrMagnitude > maxMagnitude * maxMagnitude)
                rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxMagnitude);
        }

        public static float GetSquaredSpeed(this Rigidbody rigidbody)
        {
            return rigidbody.velocity.sqrMagnitude;
        }
    }
}
