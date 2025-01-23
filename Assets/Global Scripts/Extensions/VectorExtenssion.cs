using UnityEngine;

namespace GlobalScripts.Extensions
{
    public static class VectorExtenssion
    {
        public static Vector3 GetFlatVector(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }
    }
}
