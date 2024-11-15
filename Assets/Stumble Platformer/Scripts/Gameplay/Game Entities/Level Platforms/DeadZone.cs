using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class DeadZone : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private DeadZoneEnvironment deadZoneEnvironment;

        public DeadZoneEnvironment Environment => deadZoneEnvironment;

        public void PlayDeathEffect(Vector3 position)
        {
            // Add offset to position to spawn effect at new position
        }
    }
}
