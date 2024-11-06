using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class RotatePlatform : BasePlatform
    {
        [SerializeField] private bool checkLocalPosition;
        [SerializeField] private bool followLocalPosition;
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Transform parent;

        [Header("Rotation")]
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private RotateAxis rotateAxis;

        private Vector3 _rotateAxis;

        protected override void OnAwake()
        {
            _rotateAxis = rotateAxis switch
            {
                RotateAxis.X => Vector3.right,
                RotateAxis.Y => Vector3.up,
                RotateAxis.Z => Vector3.forward,
                _ => Vector3.zero
            };
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
                Quaternion rotation = Quaternion.Euler(_rotateAxis * rotateSpeed * Time.fixedDeltaTime);
                platformBody.MoveRotation(platformBody.rotation * rotation);
            }
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
