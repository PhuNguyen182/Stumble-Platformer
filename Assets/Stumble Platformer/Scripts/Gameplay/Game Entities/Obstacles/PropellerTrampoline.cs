using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using GlobalScripts.Extensions;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PropellerTrampoline : BaseObstacle
    {
        [SerializeField] private LayerMask interactibleLayer;

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
    }
}
