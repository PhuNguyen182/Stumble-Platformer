using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask moveableGroundMask;
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private float checkGroundRadius;
        [SerializeField] private PlayerPhysics playerPhysics;

        private Vector3 _checkPosition;
        private Vector3 _checkVelocityPosition;
        private Vector3 _tempGroundVelocity;

        private Collider[] _groundCollider;
        private Collider[] _platformCollider;

        public bool IsGrounded { get; private set; }
        public bool IsPlatformGround { get; private set; }
        public Vector3 GroundVelocity { get; private set; }

        private void Awake()
        {
            _groundCollider = new Collider[6];
            _platformCollider = new Collider[6];
        }

        public void CheckGround()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _groundCollider, groundMask) > 0;
            IsPlatformGround = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _platformCollider, moveableGroundMask) > 0;

            if (IsGrounded)
                InspectGround();
            else
            {
                GroundVelocity = Vector3.zero;
                playerPhysics.ResetBodyValue();
            }
        }

        private void InspectGround()
        {
            for (int i = 0; i < _groundCollider.Length; i++)
            {
                if (_groundCollider[i])
                {
                    if (_groundCollider[i].TryGetComponent(out WalkableSurface walkableSurface))
                    {
                        playerPhysics.SetLinearDrag(walkableSurface.LinearDrag);
                        playerPhysics.SetJumpRestriction(walkableSurface.JumpRestriction);
                        playerPhysics.SetAngularDrag(walkableSurface.AngularDrag);
                    }
                }
            }

            if (!IsPlatformGround)
            {
                GroundVelocity = Vector3.zero;
                Array.Clear(_platformCollider, 0, _platformCollider.Length);
            }

            else
            {
                for (int i = 0; i < _platformCollider.Length; i++)
                {
                    if (!_platformCollider[i])
                        continue;

                    if (!_platformCollider[i].attachedRigidbody)
                        continue;

                    _tempGroundVelocity = _platformCollider[i].attachedRigidbody.GetPointVelocity(transform.position);

                    if (_tempGroundVelocity == Vector3.zero)
                        continue;

                    GroundVelocity = _tempGroundVelocity;
                }
            }
        }

        private void OnDestroy()
        {
            Array.Clear(_groundCollider, 0, _groundCollider.Length);
            Array.Clear(_platformCollider, 0, _platformCollider.Length);
        }
    }
}
