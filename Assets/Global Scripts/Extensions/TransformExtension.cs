using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.Extensions
{
    public static class TransformExtension
    {
        public static bool IsCloseTo(this Transform transform, Transform targetPoint, float minDistance)
        {
            return Vector3.SqrMagnitude(targetPoint.position - transform.position) <= minDistance * minDistance;
        }

        public static bool IsCloseTo(this Transform transform, Vector3 targetPosition, float minDistance)
        {
            return Vector3.SqrMagnitude(targetPosition - transform.position) <= minDistance * minDistance;
        }

        public static float GetDistance(this Transform transform, Vector3 targetPosition)
        {
            return Vector3.Magnitude(targetPosition - transform.position);
        }

        public static float GetSquaredDistance(this Transform transform, Vector3 targetPosition)
        {
            return Vector3.SqrMagnitude(targetPosition - transform.position);
        }

        public static void SetTRS(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }

        public static void SetTRS(this Transform transform, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Quaternion quaternion = Quaternion.Euler(rotation);
            transform.SetTRS(position, quaternion, scale);
        }

        public static void SetTRSP(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            transform.SetTRS(position, rotation, scale);
            transform.SetParent(parent);
        }

        public static void SetTRSP(this Transform transform, Vector3 position, Vector3 rotation, Vector3 scale, Transform parent = null)
        {
            transform.SetTRS(position, rotation, scale);
            transform.SetParent(parent);
        }

        public static bool TryGetChildComponent<T>(this Transform transform, int childIndex, out T component) where T : Component
        {
            if(transform.childCount == 0 || childIndex >= transform.childCount)
            {
                component = default;
                return false;
            }

            return transform.GetChild(childIndex).TryGetComponent<T>(out component);
        }
    }
}
