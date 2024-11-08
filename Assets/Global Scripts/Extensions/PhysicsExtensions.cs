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

        public static Vector3 GetFlatVelocity(this Rigidbody rigidbody)
        {
            return rigidbody.velocity.x * Vector3.right + rigidbody.velocity.z * Vector3.forward;
        }

        public static float GetSquaredFlatSpeed(this Rigidbody rigidbody)
        {
            Vector3 flatVelocity = rigidbody.velocity.x * Vector3.right + rigidbody.velocity.z * Vector3.forward;
            return flatVelocity.sqrMagnitude;
        }

        public static float GetSquaredSpeed(this Rigidbody rigidbody)
        {
            return rigidbody.velocity.sqrMagnitude;
        }
    }
}
