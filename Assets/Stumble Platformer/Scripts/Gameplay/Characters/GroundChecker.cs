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

        public bool IsGrounded { get; private set; }
        public Vector3 GroundVelocity { get; private set; }

        private void Update()
        {
            _checkPosition = transform.position + groundOffset;
            IsGrounded = Physics.CheckSphere(_checkPosition, checkGroundRadius, groundMask);
        }
    }
}
