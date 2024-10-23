using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Characters
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private float checkGroundRadius;

        private Vector3 _checkPosition;
        private Collider[] _groundResult;

        public bool IsGrounded { get; private set; }
        public Vector3 GroundVelocity { get; private set; }

        private void Awake()
        {
            _groundResult = new Collider[20];
        }

        private void Update()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.CheckSphere(_checkPosition, checkGroundRadius, groundMask);

            if (IsGrounded)
                InspectGround();
            else
                GroundVelocity = Vector3.zero;
        }

        private void InspectGround()
        {
            int resultCount = Physics.OverlapSphereNonAlloc(_checkPosition, checkGroundRadius, _groundResult, groundMask);

            if (resultCount <= 0)
            {
                GroundVelocity = Vector3.zero;
                return;
            }

            if (_groundResult[0].TryGetComponent<Rigidbody>(out var groundBody))
            {
                GroundVelocity = groundBody.velocity;
            }
        }

        private void OnDestroy()
        {
            Array.Clear(_groundResult, 0, _groundResult.Length);
        }
    }
}
