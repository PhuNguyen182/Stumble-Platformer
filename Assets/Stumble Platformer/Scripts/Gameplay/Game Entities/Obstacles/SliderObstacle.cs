using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CommonMovement;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class SliderObstacle : BaseObstacle
    {
        [SerializeField] private OscillatePosition oscillatePosition;

        [Header("Movement")]
        [SerializeField] private AnimationCurve easeCurve;
        [SerializeField] private Vector3 moveAxis = Vector3.up;
        [SerializeField] private bool useRotationAsDirection;
        [SerializeField] private float moveDistance = 2f;
        [SerializeField] private float duration = 2f;

        [Header("Delay")]
        [SerializeField] private bool useRandomDelay = false;
        [SerializeField] private float maxRandomDelay = 1f;

        public override void SetObstacleActive(bool active)
        {
            base.SetObstacleActive(active);
            oscillatePosition.IsActive = active;
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
                    StunDuration = stunDuration,
                });
            }
        }

        public override void ExitDamage(Collision collision) { }

        public override void ObstacleAction() { }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ModifySlider();
        }

        private void ModifySlider()
        {
            if (oscillatePosition != null)
            {
                oscillatePosition.easeCurve = easeCurve;
                oscillatePosition.moveAxis = moveAxis;
                oscillatePosition.moveDistance = moveDistance;
                oscillatePosition.duration = duration;
                oscillatePosition.useRotationAsDirection = useRotationAsDirection;
                oscillatePosition.useRandomDelay = useRandomDelay;
                oscillatePosition.maxRandomDelay = maxRandomDelay;
            }
        }
#endif
    }
}
