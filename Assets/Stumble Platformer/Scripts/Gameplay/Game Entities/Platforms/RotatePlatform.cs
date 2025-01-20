using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class RotatePlatform : BasePlatform
    {
        [SerializeField] private LayerMask playerMask;

        [Space(10)]
        [SerializeField] private bool checkLocalPosition;
        [SerializeField] private bool followLocalPosition;
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Transform parent;
        [SerializeField] private Rigidbody pivot;

        [Header("Rotation")]
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private RotateAxis rotateAxis;

        private Vector3 _rotateAxis;
        private Quaternion _rotation;

        protected override void OnAwake()
        {
            base.OnAwake();
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };
        }

        public override void SetPlatformActive(bool active)
        {
            base.SetPlatformActive(active);
            IsActive = active;
        }

        public override void OnPlatformCollide(Collision collision)
        {
            
        }

        public override void OnPlatformStay(Collision collision)
        {
            
        }

        public override void OnPlatformExit(Collision collision)
        {
            
        }

        public override void PlatformAction()
        {
            if (followLocalPosition)
                platformBody.position = parent.TransformPoint(localPosition);

            if (rotateSpeed != 0)
            {
                _rotation = Quaternion.Euler(_rotateAxis * rotateSpeed * Time.fixedDeltaTime);
                _rotation = platformBody.rotation * _rotation;
                
                if (usePhysics)
                    platformBody.MoveRotation(_rotation);
                else
                    platformBody.transform.rotation = _rotation;
            }
        }

        private void OnPlatformTriggerEnter(Collider collider)
        {
        }

        private void OnPlatformTriggerStay(Collider collider)
        {
        }

        private void OnPlatformTriggerExit(Collider collider)
        {
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (checkLocalPosition)
            {
                checkLocalPosition = false;
                localPosition = transform.localPosition;
            }
        }
#endif
    }
}
