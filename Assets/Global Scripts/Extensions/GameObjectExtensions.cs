using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasLayer(this GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask.value & (1 << gameObject.layer)) > 0;
        }

        public static bool HasLayer(this Collider collider, LayerMask layerMask)
        {
            return collider != null ? HasLayer(collider.gameObject, layerMask) : false;
        }

        public static bool HasLayer(this Collision collision, LayerMask layerMask)
        {
            return collision != null ? HasLayer(collision.gameObject, layerMask) : false;
        }

        public static bool HasLayer(this Collider2D collider, LayerMask layerMask)
        {
            return collider != null ? HasLayer(collider.gameObject, layerMask) : false;
        }

        public static bool HasLayer(this Collision2D collision, LayerMask layerMask)
        {
            return collision != null ? HasLayer(collision.gameObject, layerMask) : false;
        }
    }
}
