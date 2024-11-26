using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class PushPillarObstacle : BaseObstacle
    {
        [SerializeField] private float pushForce = 6;
        [SerializeField] private Animator platformAnimator;

        private readonly int _pushHash = Animator.StringToHash("Push");

        public override void DamageCharacter(Collision collision)
        {
            Vector3 hitNormal = collision.GetContact(0).normal;
            float hitImpulse = collision.GetContact(0).impulse.magnitude;

            if (collision.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(new DamageData
                {
                    AttackForce = hitImpulse + attactForce,
                    ForceDirection = -hitNormal,
                    StunDuration = stunDuration
                });

                platformAnimator.SetTrigger(_pushHash);
            }

            else
            {
                if (collision.collider.attachedRigidbody != null)
                {
                    collision.collider.attachedRigidbody.velocity = -hitNormal * (hitImpulse + pushForce);
                    platformAnimator.SetTrigger(_pushHash);
                }
            }
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            
        }
    }
}
