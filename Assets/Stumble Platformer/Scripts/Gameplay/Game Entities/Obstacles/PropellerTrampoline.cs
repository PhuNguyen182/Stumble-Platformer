using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Effectors;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PropellerTrampoline : BaseObstacle
    {
        [SerializeField] private float force;
        [SerializeField] private LayerMask interactibleLayer;
        [SerializeField] private VolumeEffector3D volumeEffector;

        public override void DamageCharacter(Collision collision)
        {
            if (!collision.HasLayer(interactibleLayer))
                return;

            if (collision.collider.TryGetComponent(out ICharacterMovement characterMovement))
                characterMovement.OnGrounded();
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (volumeEffector)
                volumeEffector.forceMagnitude = force;
        }
#endif
    }
}
