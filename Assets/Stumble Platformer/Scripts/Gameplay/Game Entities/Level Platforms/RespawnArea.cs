using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    [RequireComponent(typeof(BoxCollider))]
    public class RespawnArea : MonoBehaviour
    {
        [SerializeField] private int areaIndex;
        [SerializeField] private BoxCollider range;
        [SerializeField] private Vector3 rangeCenter;
        [SerializeField] private Vector3 rangeSize;
        [SerializeField] private float height = 0;
        [SerializeField] private bool updateRange;

        public int AreaIndex => areaIndex;
        public Vector3 Size => rangeSize;
        public Vector3 Center => rangeCenter + transform.position;

        public Vector3 GetRandomSpawnPosition()
        {
            float x = Random.Range(Center.x - rangeSize.x / 2, Center.x + rangeSize.x / 2);
            float z = Random.Range(Center.z - rangeSize.z / 2, Center.z + rangeSize.z / 2);
            Vector3 position = new Vector3(x, Center.y + height, z);
            return position;
        }

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
