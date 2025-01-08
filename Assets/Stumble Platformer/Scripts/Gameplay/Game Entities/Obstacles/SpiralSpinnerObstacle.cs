using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class SpiralSpinnerObstacle : BaseObstacle
    {
        [Header("Spin Spiral")]
        [SerializeField] private Vector3 spiralCenter;
        [SerializeField] private float phase = 0;
        [SerializeField] private float spiralRadiusTime = 6f;
        [SerializeField] private float spiralRotateSpeed = 10f;
        [SerializeField] private float minSpiralRadius = 1f;
        [SerializeField] private float maxSpiralRadius = 6f;

        [Header("Spin Self")]
        [SerializeField] private float selfRotateSpeed = 10f;
        [SerializeField] private RotateAxis rotateAxis;

        private float _spiralTime = 0;
        private Vector3 _rotateAxis;
        private Vector3 _originalPosition;

        public override void OnAwake()
        {
            base.OnAwake();
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };

            _originalPosition = transform.position;
        }

        public override void DamageCharacter(Collision collision)
        {
            if (!collision.transform.TryGetComponent(out ICharacterMovement characterMovement))
                return;

            if (collision.collider.TryGetComponent(out IDamageable damageable))
            {
                Vector3 forceDirection = collision.GetContact(0).normal;

                if (forceDirection.y < 0)
                {
                    forceDirection.y = 0;
                    forceDirection.Normalize();

                }
                damageable.TakePhysicalAttack(new PhysicalDamage
                {
                    AttackForce = -attactForce,
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
            SpinSelf();
            SpiralMove();
        }

        private void SpiralMove()
        {
            _spiralTime += Time.deltaTime;
            float radiusInterpolation = 0.5f * (Mathf.Sin(_spiralTime * spiralRadiusTime) + 1);
            float radius = Mathf.Lerp(minSpiralRadius, maxSpiralRadius, radiusInterpolation);

            float x = radius * Mathf.Cos(_spiralTime * spiralRotateSpeed + Mathf.Deg2Rad * phase);
            float z = radius * Mathf.Sin(_spiralTime * spiralRotateSpeed + Mathf.Deg2Rad * phase);

            Vector3 position = new Vector3(x, 0, z) + spiralCenter + _originalPosition;
            obstacleBody.position = position;
        }

        private void SpinSelf()
        {
            Quaternion rotation = Quaternion.Euler(_rotateAxis * selfRotateSpeed * Time.fixedDeltaTime);
            obstacleBody.MoveRotation(obstacleBody.rotation * rotation);
        }
    }
}
