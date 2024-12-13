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
        [SerializeField] private Vector3 respawnRange;
        [SerializeField] private float height = 0;

        public int AreaIndex => areaIndex;
        public Vector3 Size => rangeSize;
        public Vector3 Center => rangeCenter + transform.position;

        public Vector3 GetRandomSpawnPosition()
        {
            float x = Random.Range(Center.x - respawnRange.x / 2, Center.x + respawnRange.x / 2);
            float z = Random.Range(Center.z - respawnRange.z / 2, Center.z + respawnRange.z / 2);
            Vector3 position = new Vector3(x, Center.y + height, z);
            return position;
        }

        [Button][HorizontalGroup(GroupID = "Update")]
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
                float x = Mathf.Clamp(respawnRange.x, 0, rangeSize.x);
                float y = Mathf.Clamp(respawnRange.y, 0, rangeSize.y);
                float z = Mathf.Clamp(respawnRange.z, 0, rangeSize.z);
                respawnRange = new Vector3(x, y, z);
            }
        }

        [Button][HorizontalGroup(GroupID = "Update")]
        private void UpdateName() => gameObject.name = $"Respawn Area {areaIndex}";

        [Button][HorizontalGroup(GroupID = "Respawn Index")]
        private void IncreaseIndex() => areaIndex++;

        [Button][HorizontalGroup(GroupID = "Respawn Index")]
        private void DecreaseIndex() => areaIndex--;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawRespawnRange();
        }

        private void DrawRespawnRange()
        {
            Vector3 boxSize = range.size;
            Vector3 center = range.center;
            Vector3 scaledSize = new Vector3
                (
                    boxSize.x * transform.localScale.x,
                    boxSize.y * transform.localScale.y,
                    boxSize.z * transform.localScale.z
                );

            Gizmos.color = new Color(0, 1, 0, 0.4f);
            Gizmos.DrawCube(transform.position + center, scaledSize);
            Gizmos.color = new Color(0.5f, 1, 0, 0.6f);
            Gizmos.DrawCube(transform.position + center, respawnRange);
        }
#endif
    }
}
