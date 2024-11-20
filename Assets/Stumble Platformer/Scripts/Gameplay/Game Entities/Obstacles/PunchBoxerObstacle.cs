using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PunchBoxerObstacle : BaseObstacle
    {
        [SerializeField] private float attackThreshold;
        [SerializeField] private float attackTrigger = 0.05f; // Use blend key to check
        [SerializeField] private SkinnedMeshRenderer punchBoxerSkin;

        private bool _canAttack;

        public override void DamageCharacter(Collision collision)
        {
            if (!collision.transform.TryGetComponent(out ICharacterMovement characterMovement))
                return;

            if (!collision.collider.TryGetComponent(out IDamageable damageable))
                return;

            if (collision.impulse.sqrMagnitude < attackThreshold * attackThreshold)
                return;

            if (_canAttack)
            {
                Vector3 forceDirection = collision.GetContact(0).normal;

                if (forceDirection.y < 0)
                {
                    forceDirection.y = 0;
                    forceDirection.Normalize();
                }

                damageable.TakeDamage(new DamageData
                {
                    AttackForce = -attactForce,
                    ForceDirection = forceDirection,
                    StunDuration = 1.5f,
                });
            }
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            _canAttack = punchBoxerSkin.GetBlendShapeWeight(0) < attackTrigger;
        }
    }
}
