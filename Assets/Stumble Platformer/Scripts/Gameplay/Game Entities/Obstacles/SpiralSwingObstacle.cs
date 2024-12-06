using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class SpiralSwingObstacle : BaseObstacle
    {
        [SerializeField] private float swingSpeed = 3f;
        [SerializeField] private float rotateSpeed = 3f;
        [SerializeField][Range(15, 90)] private float swingAngle = 60;
        [SerializeField][Range(0, 1)] private float minSwingPercent = 0.4f;

        private float _startTime;
        private float _damageAttack;

        public override void DamageCharacter(Collision collision)
        {
            if (!collision.transform.TryGetComponent(out ICharacterMovement characterMovement))
                return;

            if (collision.collider.TryGetComponent(out IDamageable damageable))
            {
                Vector3 forceDirection = collision.GetContact(0).normal;

                if (forceDirection.y > 0)
                {
                    forceDirection.y = 0;
                    forceDirection.Normalize();
                }

                damageable.TakeDamage(new DamageData
                {
                    AttackForce = -attactForce * _damageAttack,
                    ForceDirection = forceDirection,
                    StunDuration = stunDuration,
                });
            }
        }

        public override void ExitDamage(Collision collision)
        {
            
        }

        public override void ObstacleAction()
        {
            _startTime += Time.fixedDeltaTime;
            float interpolate = (Mathf.Sin(swingSpeed * _startTime + Mathf.PI / 2) + 1) / 2;

            _damageAttack = Mathf.Lerp(minSwingPercent, 1, interpolate);
            float angle = Mathf.Lerp(swingAngle * minSwingPercent, swingAngle, interpolate);
            obstacleBody.rotation = Quaternion.Euler(0, _startTime * rotateSpeed, angle);
        }
    }
}
