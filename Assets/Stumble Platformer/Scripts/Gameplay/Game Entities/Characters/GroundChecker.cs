using GlobalScripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask moveableGroundMask;
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private float checkGroundRadius;

        private Vector3 _checkPosition;
        private Collider[] _groundCollider = new Collider[20];

        public bool IsGrounded { get; private set; }
        public bool HasMoveableGround { get; private set; }
        public Vector3 GroundVelocity { get; private set; }
        public Vector3 FlatGroundVelocity { get; private set; }

        private void FixedUpdate()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _groundCollider, groundMask) > 0;

            GroundVelocity = IsGrounded ? GetGroundVelocity() : Vector3.zero;
            FlatGroundVelocity = new(GroundVelocity.x, 0, GroundVelocity.z);
            HasMoveableGround = IsGrounded && IsMoveableGround();
        }

        private Vector3 GetGroundVelocity()
        {
            return _groundCollider[0].attachedRigidbody != null ? _groundCollider[0].attachedRigidbody.velocity : Vector3.zero;
        }

        private bool IsMoveableGround() => _groundCollider[0].HasLayer(moveableGroundMask);
    }
}
