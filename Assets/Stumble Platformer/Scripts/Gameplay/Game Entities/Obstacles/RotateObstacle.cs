using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    public class RotateObstacle : BaseObstacle
    {
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float invertTimer = 6f;
        [SerializeField] private float nextInversionTimer = 3f;
        [SerializeField] private RotateAxis rotateAxis;
        [SerializeField] private RotateMode rotateMode;

        private float _rotateSpeed = 0;
        private float _invertTimer = 0;
        private float _rotateWeight = 1;
        private float _nextInvertTime = 0;
        private int _nextCount = 0;
        private Vector3 _rotateAxis;

        public override void OnAwake()
        {
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };
            IsActive = true;
            _rotateSpeed = rotateSpeed;
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
            switch (rotateMode)
            {
                case RotateMode.SwitchWise:
                    RotateSwitchWise();
                    break;
                case RotateMode.InvertGradually:
                    InvertGradually();
                    break;
            }

            RotateContinous();
        }

        private void RotateContinous()
        {
            Quaternion rotation = Quaternion.Euler(_rotateAxis * _rotateSpeed * _rotateWeight * Time.fixedDeltaTime);
            obstacleBody.MoveRotation(obstacleBody.rotation * rotation);
        }

        private void RotateSwitchWise()
        {
            _invertTimer += Time.deltaTime;
            if (_invertTimer > invertTimer)
            {
                _invertTimer = 0;
                _rotateSpeed *= -1;
            }
        }

        private void InvertGradually()
        {
            if (_invertTimer < invertTimer)
            {
                _rotateWeight = 1;
                _invertTimer += Time.deltaTime;
            }

            if(_invertTimer > invertTimer)
            {
                _nextInvertTime += Time.deltaTime;
                _rotateWeight = _nextCount == 0 ? Mathf.Lerp(1f, 0f, _nextInvertTime / nextInversionTimer)
                                                : Mathf.Lerp(0f, 1f, _nextInvertTime / nextInversionTimer);

                if(_nextInvertTime > nextInversionTimer)
                {
                    _nextCount += 1;
                    _nextInvertTime = 0;
                    _rotateSpeed *= -1;

                    if (_nextCount == 2)
                    {
                        _nextCount = 0;
                        _rotateSpeed *= -1;
                        _invertTimer = 0;
                    }
                }
            }
        }
    }
}
