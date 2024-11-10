using System;
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
        private Collider[] _groundCollider;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _groundCollider = new Collider[3];
        }

        private void FixedUpdate()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _groundCollider, groundMask) > 0;
        }

        private void OnDestroy()
        {
            Array.Clear(_groundCollider, 0, _groundCollider.Length);
        }
    }
}
