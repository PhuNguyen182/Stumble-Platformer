using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    public class RotatePlatform : BasePlatform
    {
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

        public override void OnPlatformExit(Collision collision)
        {
            
        }

        public override void PlatformAction()
        {
            Quaternion rotation = Quaternion.Euler(_rotateAxis * rotateSpeed * Time.fixedDeltaTime);
            platformBody.MoveRotation(platformBody.rotation * rotation);
        }
    }
}
