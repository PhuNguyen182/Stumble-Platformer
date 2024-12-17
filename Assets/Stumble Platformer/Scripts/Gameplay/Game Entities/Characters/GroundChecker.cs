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
        private Collider[] _groundCollider;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _groundCollider = new Collider[10];
        }

        private void FixedUpdate()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _groundCollider, groundMask) > 0;

            if (IsGrounded)
                InspectGround();
            else
                playerPhysics.ResetBodyValue();
        }

        private void InspectGround()
        {
            for (int i = 0; i < _groundCollider.Length; i++)
            {
                if (_groundCollider[i].TryGetComponent(out WalkableSurface walkableSurface))
                {
                    playerPhysics.SetLinearDrag(walkableSurface.LinearDrag);
                    playerPhysics.SetAngularDrag(walkableSurface.AngularDrag);
                    playerPhysics.SetJumpRestriction(walkableSurface.JumpRestriction);
                }
            }
        }

        private void OnDestroy()
        {
            Array.Clear(_groundCollider, 0, _groundCollider.Length);
        }
    }
}
