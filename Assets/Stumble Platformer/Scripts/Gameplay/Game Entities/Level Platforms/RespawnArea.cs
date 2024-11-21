using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

        [Button]
        private void UpdateRange()
        {
            if (range != null)
            {
                rangeCenter = range.center;
                rangeSize = range.size;
                Vector3 scaledSize = new Vector3
                (
                    rangeSize.x * transform.localScale.x,
                    rangeSize.y * transform.localScale.y,
                    rangeSize.z * transform.localScale.z
                );

                rangeSize = scaledSize;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 boxSize = range.size;
            Vector3 center = range.center;
            Vector3 scaledSize = new Vector3
                (
                    boxSize.x * transform.localScale.x,
                    boxSize.y * transform.localScale.y,
                    boxSize.z * transform.localScale.z
                );

            Gizmos.color = new Color(0, 1, 0, 0.45f);
            Gizmos.DrawCube(transform.position + center, scaledSize);
        }
#endif
    }
}
