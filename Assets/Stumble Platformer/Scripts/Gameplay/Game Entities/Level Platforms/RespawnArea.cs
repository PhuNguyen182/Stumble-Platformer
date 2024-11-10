using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    [RequireComponent(typeof(BoxCollider))]
    public class RespawnArea : MonoBehaviour
    {
        [SerializeField] private BoxCollider range;
        [SerializeField] private Vector3 rangeCenter;
        [SerializeField] private Vector3 rangeSize;
        [SerializeField] private bool updateRange;

        public Vector3 Size => rangeSize;
        public Vector3 Center => rangeCenter + transform.position;

#if UNITY_EDITOR
        private void OnValidate()
        {
            range ??= GetComponent<BoxCollider>();
            range.isTrigger = true;

            if (updateRange)
            {
                updateRange = false;

                if (range != null)
                {
                    rangeCenter = range.center;
                    rangeSize = range.size;
                }
            }
        }
#endif
    }
}
