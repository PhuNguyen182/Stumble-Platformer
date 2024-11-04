using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class SwingObstacle : BaseObstacle
    {
        [SerializeField] private RotateAxis rotateAxis;
        [SerializeField] private Transform maceObject;
        
        [Header("Pendulum Settings")]
        [SerializeField][Range(0f, 1f)] private float startTime = 0.5f;
        [SerializeField] private float swingSpeed = 3f;
        [SerializeField] private float swingAngle = 60;
        [SerializeField] private float maceSpinningSpeed = 12f;

        private float _startTime;
        private Quaternion _startAngle, _endAngle;

        private void OnEnable()
        {
            _startTime = startTime;
            _startAngle = CalculatePendulumAngle(swingAngle);
            _endAngle = CalculatePendulumAngle(-swingAngle);
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
            _startTime += Time.fixedDeltaTime;
            obstacleBody.rotation = Quaternion.Lerp(_startAngle, _endAngle, (Mathf.Sin(swingSpeed * _startTime + Mathf.PI / 2) + 1) / 2);

            if (maceObject != null)
                maceObject.Rotate(Vector3.up * maceSpinningSpeed);
        }

        private Quaternion CalculatePendulumAngle(float angle)
        {
            Quaternion pendulumRotation = obstacleBody.rotation;

            float targetAngle = rotateAxis switch
            {
                RotateAxis.X => pendulumRotation.eulerAngles.x + angle,
                RotateAxis.Y => pendulumRotation.eulerAngles.y + angle,
                RotateAxis.Z => pendulumRotation.eulerAngles.z + angle,
                _ => 0
            };

            if (targetAngle > 180)
                targetAngle -= 360;

            else if (targetAngle < -180)
                targetAngle += 360;

            Vector3 newRotation = rotateAxis switch
            {
                RotateAxis.X => new Vector3(targetAngle, pendulumRotation.eulerAngles.y, pendulumRotation.eulerAngles.z),
                RotateAxis.Y => new Vector3(pendulumRotation.eulerAngles.x, targetAngle, pendulumRotation.eulerAngles.z),
                RotateAxis.Z => new Vector3(pendulumRotation.eulerAngles.x, pendulumRotation.eulerAngles.y, targetAngle),
                _ => Vector3.zero
            };

            pendulumRotation = Quaternion.Euler(newRotation);
            return pendulumRotation;
        }
    }
}
