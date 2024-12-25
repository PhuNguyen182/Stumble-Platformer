using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

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
                if (damageable is ICharacterMovement characterMovement && characterMovement.IsStunning)
                    NormalPush(collision, hitNormal, hitImpulse);

                else
                    AttackCharacter(damageable, hitNormal, hitImpulse);
            }

            else
                NormalPush(collision, hitNormal, hitImpulse);
        }

        private void AttackCharacter(IDamageable damageable, Vector3 hitNormal, float hitImpulse)
        {
            damageable.TakePhysicalAttack(new PhysicalDamage
            {
                AttackForce = hitImpulse + attactForce,
                ForceDirection = -hitNormal,
                StunDuration = stunDuration
            });

            platformAnimator.SetTrigger(_pushHash);
        }

        private void NormalPush(Collision collision, Vector3 hitNormal, float hitImpulse)
        {
            if (collision.collider.attachedRigidbody != null)
            {
                collision.collider.attachedRigidbody.velocity = -hitNormal * (hitImpulse + pushForce);
                platformAnimator.SetTrigger(_pushHash);
            }
        }

        public override void ExitDamage(Collision collision) { }

        public override void ObstacleAction() { }
    }
}
