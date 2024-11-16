using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    [RequireComponent(typeof(BoxCollider))]
    public class DeadZone : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private DeadZoneEnvironment deadZoneEnvironment;
        [SerializeField] private BoxCollider deadZoneCollider;
        [SerializeField] private bool drawGizmo = true;

        public static GamePlayMode GamePlayMode;

        public void PlayDeathEffect(Vector3 position)
        {
            // Add offset to position to spawn effect at new position
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmo)
                return;

            Vector3 boxSize = deadZoneCollider.size;
            Vector3 scaledSize = new Vector3
                (
                    boxSize.x * transform.localScale.x,
                    boxSize.y * transform.localScale.y,
                    boxSize.z * transform.localScale.z
                );

            Gizmos.color = new Color(1, 0, 0, 0.45f);
            Gizmos.DrawCube(transform.position, scaledSize);
        }

        private void OnValidate()
        {
            deadZoneCollider ??= GetComponent<BoxCollider>();
        }
#endif
    }
}
