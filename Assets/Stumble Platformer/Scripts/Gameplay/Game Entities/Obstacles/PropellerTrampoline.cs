using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PropellerTrampoline : BaseObstacle
    {
        public override void DamageCharacter(Collision collision)
        {
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
